namespace NesEmulator.PPU
{
    internal class Ppu : IPpu
    {
        private IReadWriteChr _romChr;
        
        public byte Read(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public byte Peek(ushort address)
        {
            throw new System.NotImplementedException();
        }

        public void Write(ushort address, byte value)
        {
            throw new System.NotImplementedException();
        }

        public void Load(IReadWriteChr rom)
        {
            _romChr = rom;
            
            throw new System.NotImplementedException();
        }
    }
}