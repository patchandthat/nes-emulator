using System.Collections;
using System.Collections.Generic;

namespace NesEmulator.PPU
{
    public sealed class NesPalette : IReadOnlyDictionary<byte, Color>
    {
        private static Dictionary<byte, Color> Colors;

        static NesPalette()
        {
            Colors = new Dictionary<byte, Color>
            {
                {0x00, new Color("#7C7C7C")},
                {0x01, new Color("#0000FC")},
                {0x02, new Color("#0000BC")},
                {0x03, new Color("#4428BC")},
                {0x04, new Color("#940084")},
                {0x05, new Color("#A80020")},
                {0x06, new Color("#A81000")},
                {0x07, new Color("#881400")},
                {0x08, new Color("#503000")},
                {0x09, new Color("#007800")},
                {0x0A, new Color("#006800")},
                {0x0B, new Color("#005800")},
                {0x0C, new Color("#004058")},
                {0x0D, new Color("#000000")},
                {0x0E, new Color("#000000")},
                {0x0F, new Color("#000000")},
                {0x10, new Color("#BCBCBC")},
                {0x11, new Color("#0078F8")},
                {0x12, new Color("#0058F8")},
                {0x13, new Color("#6844FC")},
                {0x14, new Color("#D800CC")},
                {0x15, new Color("#E40058")},
                {0x16, new Color("#F83800")},
                {0x17, new Color("#E45C10")},
                {0x18, new Color("#AC7C00")},
                {0x19, new Color("#00B800")},
                {0x1A, new Color("#00A800")},
                {0x1B, new Color("#00A844")},
                {0x1C, new Color("#008888")},
                {0x1D, new Color("#000000")},
                {0x1E, new Color("#000000")},
                {0x1F, new Color("#000000")},
                {0x20, new Color("#F8F8F8")},
                {0x21, new Color("#3CBCFC")},
                {0x22, new Color("#6888FC")},
                {0x23, new Color("#9878F8")},
                {0x24, new Color("#F878F8")},
                {0x25, new Color("#F85898")},
                {0x26, new Color("#F87858")},
                {0x27, new Color("#FCA044")},
                {0x28, new Color("#F8B800")},
                {0x29, new Color("#B8F818")},
                {0x2A, new Color("#58D854")},
                {0x2B, new Color("#58F898")},
                {0x2C, new Color("#00E8D8")},
                {0x2D, new Color("#787878")},
                {0x2E, new Color("#000000")},
                {0x2F, new Color("#000000")},
                {0x30, new Color("#FCFCFC")},
                {0x31, new Color("#A4E4FC")},
                {0x32, new Color("#B8B8F8")},
                {0x33, new Color("#D8B8F8")},
                {0x34, new Color("#F8B8F8")},
                {0x35, new Color("#F8A4C0")},
                {0x36, new Color("#F0D0B0")},
                {0x37, new Color("#FCE0A8")},
                {0x38, new Color("#F8D878")},
                {0x39, new Color("#D8F878")},
                {0x3A, new Color("#B8F8B8")},
                {0x3B, new Color("#B8F8D8")},
                {0x3C, new Color("#00FCFC")},
                {0x3D, new Color("#D8D8D8")},
                {0x3E, new Color("#000000")},
                {0x3F, new Color("#000000")}
            };
        }

        public IEnumerator<KeyValuePair<byte, Color>> GetEnumerator()
        {
            return Colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count
        {
            get => Colors.Count;
        }

        public bool ContainsKey(byte key)
        {
            return Colors.ContainsKey(key);
        }

        public bool TryGetValue(byte key, out Color value)
        {
            return Colors.TryGetValue(key, out value);
        }

        public Color this[byte key] => Colors[key];

        public IEnumerable<byte> Keys => Colors.Keys;

        public IEnumerable<Color> Values => Colors.Values;
    }
}