namespace NesEmulator.PPU
{
    internal static  class PpuMemoryMap
    {
        // Overall map
        public const ushort PatternTables = 0x0000;
        public const ushort NameTables = 0x2000;
        public const ushort Palettes = 0x03F00;
        public const ushort Mirrors = 0x4000;
        
        // Pattern tables
        public const ushort PatternTable0 = 0x0000;
        public const ushort PatternTable1 = 0x1000;

        // Name tables
        public const ushort NameTable0 = 0x2000;
        public const ushort AttributeTable0 = 0x23C0;
        public const ushort NameTable1 = 0x2400;
        public const ushort AttributeTable1 = 0x27C0;
        public const ushort NameTable2 = 0x2800;
        public const ushort AttributeTable2 = 0x2BC0;
        public const ushort NameTable3 = 0x2C00;
        public const ushort AttributeTable3 = 0x2FC0;
        public const ushort NameTableMirrorStart = 0x3000;
        public const ushort NameTableMirrorEnd = 0x3EFF;

        // Palettes
        public const ushort ImagePalette = 0x3F00;
        public const ushort SpritePalette = 0x3F10;
        public const ushort PaletteMirrorStart = 0x3F20;
        public const ushort PaletteMirrorEnd = 0x3FFF;

        // Mirrors
        public const ushort MirrorStart = 0x4000;
        public const ushort Mirrorend = 0xFFFF;
    }
}