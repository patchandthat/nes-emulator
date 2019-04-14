using NesEmulator.RomMappers;

namespace NesEmulator
{
    internal class Memory : IMemory
    {
        public byte Read(ushort address)
        {
            return 0x0;
        }

        public byte Peek(ushort address)
        {
            return 0x0;
        }

        public void Write(ushort address, byte value)
        {
        }

        public void Load(ROM rom)
        {
        }
    }
}