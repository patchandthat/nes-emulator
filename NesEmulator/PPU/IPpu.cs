using System;

namespace NesEmulator.PPU
{
    internal interface IPpu : IReadWrite, IDisposable
    {
        void Load(IReadWriteChrBus rom);
    }
}