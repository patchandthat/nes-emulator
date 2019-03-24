namespace NesEmulator
{
    internal enum InterruptType
    {
        Nmi = MemoryMap.NonMaskableInterruptVector,
        Reset = MemoryMap.ResetVector,
        Irq = MemoryMap.InterruptRequestVector,
    }
}