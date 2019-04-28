using System;

namespace NesEmulator.Memory
{
    internal class MemoryBlock : IReadWrite
    {
        private readonly byte[] _bytes;
        private readonly MemoryRange _range;

        public MemoryBlock(MemoryRange range)
        {
            _range = range;
            
            _bytes = new byte[range.Length];
        }

        public virtual byte Read(ushort address)
        {
            return _bytes[address - _range.Start];
        }

        public virtual byte Peek(ushort address)
        {
            return Read(address);
        }

        public virtual void Write(ushort address, byte value)
        {
            _bytes[address - _range.Start] = value;
        }
    }
}