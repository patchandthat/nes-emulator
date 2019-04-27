namespace NesEmulator.APU
{
    public class NullApu : IApu
    {
        public byte Read(ushort address)
        {
            return 0x0;
        }

        public byte Peek(ushort address)
        {
            return 0x0;
        }

        public void Write(ushort address, byte value)
        {
        }
    }
}