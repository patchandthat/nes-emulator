using System;

namespace NesEmulator.Processor
{
    [Flags]
    internal enum InterruptType
    {
        None = 0x0,
        Nmi = 0x1,
        Reset = 0x2,
        Irq = 0x4,
        Brk = 0x8,
    }
}