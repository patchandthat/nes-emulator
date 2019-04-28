using System.IO;

namespace NesEmulator.RomMappers
{
    internal abstract class ROM : IReadWrite
    {
        public static ROM Create(Stream stream)
        {
            // Parse. Determine mapper.

            return null;
        }

        public abstract byte Read(ushort address);
        public abstract byte Peek(ushort address);
        public abstract void Write(ushort address, byte value);
    }
}