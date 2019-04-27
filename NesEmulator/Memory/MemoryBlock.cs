using System;

namespace NesEmulator.Memory
{
    internal class MemoryBlock : IReadWrite
    {
        private readonly ushort _startAddress;
        private readonly byte[] _bytes;

        public MemoryBlock(ushort startAddress, ushort endAddress)
        {
            if (startAddress >= endAddress) throw new ArgumentException("startAddress must be less than endAddress");
            
            _startAddress = startAddress;
            
            _bytes = new byte[endAddress - startAddress];
        }

        public virtual byte Read(ushort address)
        {
            return _bytes[address - _startAddress];
        }

        public virtual byte Peek(ushort address)
        {
            return Read(address);
        }

        public virtual void Write(ushort address, byte value)
        {
            _bytes[address - _startAddress] = value;
        }
    }
}