using System;

namespace NesEmulator
{
    internal class CPU
    {
        private IMemory _memory;

        public CPU(IMemory memory)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
        }

        public byte Accumulator { get; }

        public byte IndexX { get; }

        public byte IndexY { get; }

        public byte Status { get; }

        public ushort InstructionPointer { get; }

        public ushort StackPointer { get; }

        public void Run()
        {
            for (;;) Step();
            // ReSharper disable once FunctionNeverReturns
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
        NMI,
        Reset,
        IRQ
    }
}
