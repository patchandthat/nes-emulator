namespace NesEmulator.PPU
{
    internal interface IPpu : IReadWrite
    {
        void Load(IReadWriteChr rom);
    }
}