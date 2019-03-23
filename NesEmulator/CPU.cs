using System;

namespace NesEmulator
{
    internal class CPU
    {
        private readonly IMemory _memory;
        private readonly OpcodeDefinitions _opCodes;

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
            // IP = Low byte, high byte
            // Set interrupt flag low
        }

        public void Step()
        {

        }

        public void Interrupt(InterruptType type)
        {

        }
    }

    internal enum InterruptType
    {
        NMI = MemoryMap.NonMaskableInterruptVector,
        Reset = MemoryMap.ResetVector,
        IRQ = MemoryMap.InterruptRequestVector,
    }
}
