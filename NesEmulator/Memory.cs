namespace NesEmulator
{
    /* CPU Memory Map
     * Memory Layout
     * 0x0000 Zero page
     * 0x0100 Stack
     * 0x0200 Ram
     * 0x0800 Mirrors 0x0000 to 0x07FF
     * 0x2000 I/O Registers
     * 0x2008 Mirrors 0x2000 to 0x2007
     * 0x4000 I/O Registers
     * 0x4020 Expansion ROM
     * 0x6000 SRAM
     * 0x8000 PRG ROM lower bank
     * 0xC000 PRG ROM upper bank
     */

    internal interface IMemory
    {
        byte Read(ushort address);
        void Write(ushort address, byte value);
        void Load(ROM rom);
    }

    class Memory : IMemory
    {
        public byte Read(ushort address)
        {
            return 0x0;
        }

        public void Write(ushort address, byte value)
        {

        }

        public void Load(ROM rom)
        {

        }
    }
}
