using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal enum InterruptType
    {
        Nmi = MemoryMap.NonMaskableInterruptVector,
        Reset = MemoryMap.ResetVector,
        Irq = MemoryMap.InterruptRequestVector
    }
}