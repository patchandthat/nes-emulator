namespace NesEmulator
{
    static class MemoryMap
    {
        // Overall map
        public const ushort ZeroPage = 0x0000;
        public const ushort Stack = 0x0100;
        public const ushort Ram = 0x0200;
        public const ushort RamMirror = 0x0800;
        public const ushort PpuRegisters = 0x2000;
        public const ushort PpuRegisterMirror = 0x2008;
        public const ushort ApuRegisters = 0x4000;
        public const ushort ExpansionRom = 0x4020;
        public const ushort SRam = 0x6000;
        public const ushort PrgRomLowerBank = 0x8000;
        public const ushort PrgRomUpperBank = 0xC000;

        // Interrupt vector locations
        public const ushort NonMaskableInterruptVector = 0xFFFA;
        public const ushort ResetVector = 0xFFFC;
        public const ushort InterruptRequestVector = 0xFFFE;

        // PPU registers
        public const ushort PpuControl = 0x2000;
        public const ushort PpuMask = 0x2001;
        public const ushort PpuStatus = 0x2002;
        public const ushort OamAddr = 0x2003;
        public const ushort OamData = 0x2004;
        public const ushort PpuScroll = 0x2005;
        public const ushort PpuAddr = 0x2006;
        public const ushort PpuData = 0x2007;

        // APU registers
        public const ushort SquareWave1Volume = 0x4000;
        public const ushort SquareWave1Sweep = 0x4001;
        public const ushort SquareWave1PeriodLowByte = 0x4002;
        public const ushort SquareWave1PeriodHighByte = 0x4003;

        public const ushort SquareWave2Volume = 0x4004;
        public const ushort SquareWave2Sweep = 0x4005;
        public const ushort SquareWave2PeriodLowByte = 0x4006;
        public const ushort SquareWave2PeriodHighByte = 0x4007;

        public const ushort TriangleWaveLinearCounter = 0x4008;
        public const ushort Unused1 = 0x4009;
        public const ushort TriangleWavePeriodLowByte = 0x400A;
        public const ushort TriangleWavePeriodHighByte = 0x400B;

        public const ushort NoiseVolume = 0x400C;
        public const ushort Unused2 = 0x400D;
        public const ushort NoiseLowByte = 0x400E;
        public const ushort NoiseHighByte = 0x400F;

        public const ushort DmcFreq = 0x4010;
        public const ushort DmcRaw = 0x4011;
        public const ushort DmcStart = 0x4012;
        public const ushort DmcLength = 0x4013;
        
        // Misc
        public const ushort OamDma = 0x4014;
        public const ushort ApuSoundChannelStatus = 0x4015;
        public const ushort JoyPad1 = 0x4016;
        public const ushort JoyPad2 = 0x4017;
        public const ushort ApuFrameCounter = 0x4017;

        // Nb. 0x4018-0x401F is functionality which is normally disabled
        // See https://wiki.nesdev.com/w/index.php/CPU_Test_Mode
    }
}