namespace NesEmulator.Input
{
    class NullInputSource : IInputSource
    {
        public byte Read(ushort address)
        {
            return 0;
        }

        public byte Peek(ushort address)
        {
            return 0;
        }

        public void Write(ushort address, byte value)
        {
            
        }
    }
}