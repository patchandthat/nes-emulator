using System;

namespace NesEmulator.Memory
{
    internal class MemoryMirrorDecorator : IReadWrite
    {
        private readonly IReadWrite _other;
        private readonly MemoryRange _sourceArea;
        private readonly MemoryRange _mirrorArea;

        public MemoryMirrorDecorator(IReadWrite other, MemoryRange sourceArea, MemoryRange mirrorArea)
        {
            _other = other ?? throw new ArgumentNullException(nameof(other));

            if (sourceArea.Intersects(mirrorArea) || sourceArea.Contains(mirrorArea) || mirrorArea.Contains(sourceArea))
                throw new ArgumentException("Memory ranges should not overlap");
            
            _sourceArea = sourceArea;
            _mirrorArea = mirrorArea;
        }

        public byte Read(ushort address)
        {
            ushort targetAddress = MapAddress(address);

            return _other.Read(targetAddress);
        }

        public byte Peek(ushort address)
        {
            ushort targetAddress = MapAddress(address);

            return _other.Peek(targetAddress);
        }

        public void Write(ushort address, byte value)
        {
            ushort targetAddress = MapAddress(address);
            
            _other.Write(targetAddress, value);
        }

        private ushort MapAddress(ushort address)
        {
            if (!_sourceArea.Contains(address) && !_mirrorArea.Contains(address))
                throw new ArgumentOutOfRangeException("address", $"Address {address} was not in the source ({_sourceArea}) or mirror ({_mirrorArea}) address space");

            if (_mirrorArea.Contains(address))
            {
                var temp = (address - _mirrorArea.Start) % _sourceArea.Length;
                return (ushort)(temp + _sourceArea.Start);
            }

            return address;
        }
    }
}