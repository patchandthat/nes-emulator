using System;
using NesEmulator.NesEmulator;

namespace NesEmulator.PPU
{
    internal class Ppu : IPpu
    {
        private IReadWriteChrBus _romChr;
        private NesPalette _palette = new NesPalette();

        public int Scanline { get; private set; }
        public int CurrentCycle { get; private set; }
        public bool IsFrameReady { get; internal set; }

        public bool DrawDiagnostics { get; internal set; } = true;
        
        public Frame Screen { get; private set; }
        public Frame NameTableView { get; private set; }
        public Frame PatternTableView { get; private set; }
        public Frame PaletteTableView { get; private set; }

        public Ppu()
        {
            Screen = new NtscFrame(0);
            NameTableView = new DiagnosticFrame(10, 10);
            PatternTableView = new DiagnosticFrame(10, 10);
            PaletteTableView = new DiagnosticFrame(10, 10);
        }

        public void Step()
        {
            DebugDrawNoisePixel();
            
            CurrentCycle++;

            if (CurrentCycle >= 341)
            {
                CurrentCycle = 0;
                Scanline++;
                if (Scanline >= 261)
                {
                    Scanline = -1;
                    IsFrameReady = true;
                }
            }
        }
        
        private static Random _rng = new Random();
        private void DebugDrawNoisePixel()
        {
            int addr = _rng.Next() % 2 == 0 ? 0x3F : 0x30;
            Screen.SetPixel(CurrentCycle % Screen.Width, Scanline % Screen.Height, _palette[addr]);
        }

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

        public void Load(IReadWriteChrBus rom)
        {
            _romChr = rom;
        }

        public void Dispose()
        {
            Screen?.Dispose();
            NameTableView?.Dispose();
            PatternTableView?.Dispose();
            PaletteTableView?.Dispose();
        }
    }
}