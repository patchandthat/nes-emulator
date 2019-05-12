namespace NesEmulator.PPU
{
    public sealed class Color
    {
        public Color(string color)
        {
            Red = byte.Parse(color.Substring(0, 2));
            Green = byte.Parse(color.Substring(2, 2));
            Blue = byte.Parse(color.Substring(4, 2));
        }
        
        public Color(byte red, byte green, byte blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public byte Red { get; }
        public byte Green { get; }
        public byte Blue { get; }
    }
}