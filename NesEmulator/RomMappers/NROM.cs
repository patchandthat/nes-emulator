using System;

namespace NesEmulator.RomMappers
{
    internal class NROM : ROM
    {
        public NROM(RomHeader header, Memory<byte> content)
        {
        }

        public override byte Read(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public override byte Peek(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(ushort address, byte value)
        {
            throw new System.NotImplementedException();
        }
    }
}