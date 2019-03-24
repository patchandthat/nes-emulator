using System;

namespace NesEmulator
{
    internal class CPU
    {
        private readonly IMemory _memory;
        private readonly OpcodeDefinitions _opCodes;

        private bool _isReset;

        public CPU(IMemory memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));

            _opCodes = new OpcodeDefinitions();
        }

        public byte Accumulator { get; private set; }

        public byte IndexX { get; private set; }

        public byte IndexY { get; private set; }

        public StatusFlags Status { get; private set; }

        public ushort InstructionPointer { get; private set; }

        public ushort StackPointer { get; private set; }

        public long ElapsedCycles { get; private set; }

        public bool IsPowerOn { get; private set; }

        public void Power()
        {
            IsPowerOn = !IsPowerOn;

            if (IsPowerOn)
            {
                _isReset = true;

                Accumulator = 0;
                IndexX = 0;
                IndexY = 0;
                StackPointer = MemoryMap.Stack - 3;
                ElapsedCycles = 0;
                Status = StatusFlags.InterruptDisable | StatusFlags.Bit4 | StatusFlags.Bit5;

                InstructionPointer = MemoryMap.ResetVector;

                _memory.Write(MemoryMap.ApuSoundChannelStatus, 0);
                _memory.Write(MemoryMap.ApuFrameCounter, 0);
                for (ushort i = MemoryMap.SquareWave1Volume; i <= MemoryMap.NoiseHighByte; i++)
                {
                    _memory.Write(i, 0);
                }
            }
        }

        private void ExecuteInterrupt()
        {
            byte low = _memory.Read(InstructionPointer);
            byte high = _memory.Read((ushort)(InstructionPointer+1));

            InstructionPointer = (ushort)((high << 8) + low);
        }

        public void Step()
        {
            if (ShouldHandleInterrupt())
            {
                /*
                 * A note on timing.
                 * In the general case the cpu will fetch the op and operand at
                 * the instruction pointer prior to executing an interrupt
                 *
                 * Then the interrupt cycle takes 7 cycles as values are pushed to the stack
                 * and the interrupt vector loaded
                 */
                ExecuteInterrupt();
                return;
            }

            int cyclePenalty = 0;
            byte opHex = _memory.Read(InstructionPointer);
            byte operand = _memory.Read((ushort) (InstructionPointer + 1));

            OpCode opcode = _opCodes[opHex];

            operand = ResolveOperandValue(opcode, operand, ref cyclePenalty);

            ExecuteOperation(opcode, operand);

            ElapsedCycles += opcode.Cycles + cyclePenalty;
            InstructionPointer += opcode.Bytes;
        }

        private byte ResolveOperandValue(OpCode opcode, byte operand, ref int cyclePenalty)
        {
            switch (opcode.AddressMode)
            {
                case AddressMode.Implicit:
                    break;
                case AddressMode.Accumulator:
                    break;
                case AddressMode.Immediate:
                    break;
                case AddressMode.ZeroPage:
                    operand = _memory.Read(operand);
                    break;
                case AddressMode.ZeroPageX:
                    operand = _memory.Read(
                        (byte) ((operand + IndexX) % 256));
                    break;
                case AddressMode.ZeroPageY:
                    operand = _memory.Read(
                        (byte) ((operand + IndexY) % 256));
                    break;
                case AddressMode.Relative:
                    break;
                case AddressMode.Absolute:
                {
                    var highByte = _memory.Read((ushort) (InstructionPointer + 2));
                    ushort address = (ushort) ((highByte << 8) + operand);
                    operand = _memory.Read(address);
                    break;
                }
                case AddressMode.AbsoluteX:
                {
                    cyclePenalty += ((IndexX + operand) > 0xFF) ? 1 : 0;
                    var highByte = _memory.Read((ushort) (InstructionPointer + 2));
                    ushort address = (ushort) ((highByte << 8) + operand);
                    operand = _memory.Read((ushort) (address + IndexX));
                    break;
                }
                case AddressMode.AbsoluteY:
                {
                    cyclePenalty += ((IndexY + operand) > 0xFF) ? 1 : 0;
                    var highByte = _memory.Read((ushort) (InstructionPointer + 2));
                    ushort address = (ushort) ((highByte << 8) + operand);
                    operand = _memory.Read((ushort) (address + IndexY));
                    break;
                }
                case AddressMode.Indirect:
                    break;
                case AddressMode.IndirectX:
                {
                    // Index applied during indirection
                    ushort address = (byte) (operand + IndexX);
                    byte low = _memory.Read(address);
                    byte high = _memory.Read((byte) ((address + 1) % 256));
                    address = (ushort) ((high << 8) + low);
                    operand = _memory.Read(address);
                    break;
                }
                case AddressMode.IndirectY:
                {
                    // Index applied after indirection
                    byte low = _memory.Read(operand);
                    byte high = _memory.Read((byte) ((operand + 1) % 256));
                    ushort address = (ushort) (((high << 8) + low) + IndexY);
                    cyclePenalty += address > 0x00FF ? 1 : 0;
                    operand = _memory.Read(address);
                    break;
                }
            }

            return operand;
        }

        private void ExecuteOperation(OpCode opcode, byte operand)
        {
            switch (opcode.Operation)
            {
                case Operation.LDA:
                    LoadRegister(operand, b => Accumulator = b);
                    break;
                case Operation.LDX:
                    LoadRegister(operand, b => IndexX = b);
                    break;
                case Operation.LDY:
                    LoadRegister(operand, b => IndexY = b);
                    break;
            }
        }

        private void LoadRegister(byte value, Action<byte> registerAction)
        {
            registerAction(value);

            if (value == 0x0)
            {
                Status |= (StatusFlags.Zero);
            }
            else
            {
                Status &= ~StatusFlags.Zero;
            }

            if (value >= 0x80)
            {
                Status |= StatusFlags.Negative;
            }
            else
            {
                Status &= ~StatusFlags.Negative;
            }
        }

        private bool ShouldHandleInterrupt()
        {
            if (_isReset)
            {
                _isReset = false;
                return true;
            }

            return false;
        }
    }
}
