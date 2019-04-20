using System;
using NesEmulator.Extensions;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        private readonly IMemory _memory;
        private readonly OpCodes _opCodes;

        private bool _isReset;

        public CPU(IMemory memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));

            _opCodes = new OpCodes();
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
                StackPointer = MemoryMap.Ram - 1;
                ElapsedCycles = 0;
                Status = StatusFlags.InterruptDisable | StatusFlags.Bit4 | StatusFlags.Bit5;

                InstructionPointer = MemoryMap.ResetVector;

                _memory.Write(MemoryMap.ApuSoundChannelStatus, 0);
                _memory.Write(MemoryMap.ApuFrameCounter, 0);
                for (var i = MemoryMap.SquareWave1Volume; i <= MemoryMap.NoiseHighByte; i++) _memory.Write(i, 0);
            }
        }

        private void ExecuteInterrupt()
        {
            var low = _memory.Read(InstructionPointer);
            var high = _memory.Read((ushort) (InstructionPointer + 1));

            InstructionPointer = (ushort) ((high << 8) + low);
        }

        public void Step()
        {
            if (ShouldHandleInterrupt())
            {
                /*
                 * The interrupt cycle takes 7 cycles as status flags & instruction pointer are pushed to the stack
                 * and the interrupt vector loaded
                 *
                 * Registers are not pushed to the stack, that is the responsibility of the code
                 */
                ExecuteInterrupt();
                return;
            }

            var operation = _memory.Read(InstructionPointer);
            var operand = _memory.Read(InstructionPointer.Plus(1));

            var opcode = _opCodes[operation];
            opcode.ExecutionStrategy.Execute(this, opcode, operand, _memory);
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

        private void SetFlagState(StatusFlags flags, bool state)
        {
            if (state)
                SetFlags(flags);
            else
                ClearFlags(flags);
        }

        private void SetFlags(StatusFlags flag)
        {
            Status |= flag;
        }

        private void ClearFlags(StatusFlags flag)
        {
            Status &= ~flag;
        }

        private void Push(byte value)
        {
            _memory.Write(StackPointer, value);
            if (--StackPointer < MemoryMap.Stack)
                StackPointer += 0x0100;
        }

        private byte Pop()
        {
            if (++StackPointer == MemoryMap.Ram)
                StackPointer = MemoryMap.Stack;
            return _memory.Read(StackPointer);;
        }

        #region UnitTestHelpers

        /// <summary>
        ///     Internal: for convenience of unit testing
        /// </summary>
        /// <param name="flags">The flags status to set</param>
        internal void ForceStatus(StatusFlags flags)
        {
            Status = flags;
        }

        /// <summary>
        ///     Internal: for convenience of unit testing
        /// </summary>
        /// <param name="value">The pointer value to set</param>
        internal void ForceStack(ushort value)
        {
            StackPointer = value;
        }

        #endregion
    }
}