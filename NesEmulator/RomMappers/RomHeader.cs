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

            var leadingBytes = new byte[] {0x4E, 0x45, 0x53, 0x1A};
            for (int i = 0; i < leadingBytes.Length; i++)
            {
                if (data[i] != leadingBytes[i])
                    throw new ArgumentException("Not a valid iNES file, should begin 'NES<EOF>'");
            }

            NametableMirroring = (data[6] & 0x01) == 0 ? 
                NametableMirrorType.Horizontal : 
                NametableMirrorType.Vertical;

            HasBatteryPrgRam = (data[6] & 0b0000_0010) != 0;
            HasTrainer = (data[6] & 0b0000_0100) != 0;
            
            if ((data[6] & 0b0000_1000) != 0x00)
            {
                NametableMirroring = NametableMirrorType.None;
            }

            MapperNumber = (byte) ((data[6] >> 4) + (data[7] & 0xF0));
        }

        public int PrgBankSize { get; } = 16 * 1024; 
        public int ChrBankSize { get; } = 8 * 1024;

        public byte PrgRomBanks => _data[4];
        public byte ChrRomBanks => _data[5];
        
        public byte MapperNumber { get; }
        public NametableMirrorType NametableMirroring { get; }
        public bool HasBatteryPrgRam { get; }
        public bool HasTrainer { get; }
        public Version iNesVersion { get; }
        
        // Flags 8+ vary depending on iNes version
    }

    public enum NametableMirrorType
    {
        None,
        Horizontal,
        Vertical,
    }
}
