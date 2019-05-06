namespace NesEmulator.Extensions
{
    public static class UshortExtensions
    {
        public static ushort Plus(this ushort start, int difference)
        {
            return (ushort) (start + difference);
        }

        public static byte HighByte(this ushort address)
        {
            return (byte) (address >> 8);
        }
        
        public static byte LowByte(this ushort address)
        {
            return (byte) (address & 0xFF);
        }
    }
}