using System;

namespace NesEmulator.RomMappers
{
    class RomHeader
    {
        private readonly byte[] _data;

        public RomHeader(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));

            if (data.Length != 16) throw new ArgumentException("Header should be 16 bytes", nameof(data));
        }
    }
}
