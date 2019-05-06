using System;
using NesEmulator.Memory;
using NesEmulator.Processor;

namespace NesEmulator.Extensions
{
    internal static class InterruptTypeExtensions
    {
        public static ushort ToVectorAddress(this InterruptType interrupt)
        {
            if (interrupt.HasFlag(InterruptType.Reset)) return MemoryMap.ResetVector;
            if (interrupt.HasFlag(InterruptType.Nmi)) return MemoryMap.NonMaskableInterruptVector;
            if (interrupt.HasFlag(InterruptType.Brk)) return MemoryMap.InterruptRequestVector;
            if (interrupt.HasFlag(InterruptType.Irq)) return MemoryMap.InterruptRequestVector;
            
            throw new ArgumentException("No interrupt, unable to resolve vector address");
        }
    }
}