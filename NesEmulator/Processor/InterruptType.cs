using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    [Flags]
    internal enum InterruptType
    {
        None,
        Nmi = 0x1,
        Reset = 0x2,
        Irq = 0x4,
        Brk = 0x8,
    }
}