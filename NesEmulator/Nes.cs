using System;
using System.IO;
using NesEmulator.APU;
using NesEmulator.Input;
using NesEmulator.Memory;
using NesEmulator.NesEmulator;
using NesEmulator.PPU;
using NesEmulator.Processor;
using NesEmulator.RomMappers;

namespace NesEmulator
{
    public interface INes : IDisposable
    {
        void Power();
        void Reset();
        void InsertCartridge(string filePath);

        void Step();
        void StepToNextFrame();
        
        Frame Screen { get; }
        Frame NameTableView { get;  }
        Frame PatternTableView { get;}
        Frame PaletteTableView { get; }
    }
    
    public class Nes : INes
    {
        private readonly CPU _cpu;
        private readonly Ppu _ppu;
        private readonly IMemoryBus _mainBus;
        private ROM _cartridge;
        
        public long SystemClock { get; set; }
        
        public Frame Screen => _ppu.Screen;
        public Frame NameTableView => _ppu.NameTableView;
        public Frame PatternTableView => _ppu.PatternTableView;
        public Frame PaletteTableView => _ppu.PaletteTableView;
        public DisassemblyInfo Disassembly { get; private set; }

        public Nes()
        {
            _ppu = new Ppu();
            _mainBus = new MemoryBus(
                _ppu,
                new NullApu(),
                new NullInputSource(), 
                new NullInputSource());
            _cpu = new CPU(_mainBus);
        }

        public void Power()
        {
            _cpu.Power();
        }

        public void Reset()
        {
            _cpu.Interrupt(InterruptType.Reset);
        }

        public void InsertCartridge(string filePath)
        {
            _cartridge?.Dispose();
            
            using (var fs = File.OpenRead(filePath))
            {
                _cartridge = ROM.Create(fs);
                _mainBus.Load(_cartridge);
            }
        }

        public void StepToNextFrame()
        {
            while (!_ppu.IsFrameReady)
            {
                Step();
            }

            _ppu.IsFrameReady = false;
        }

        public void Step()
        {
            // Step all chips and handle timings
            _ppu.Step();
            if (SystemClock % 3 == 0)
                _cpu.Step();

            SystemClock += 1;
        }

        public void Dispose()
        {
            _ppu.Dispose();
            _cartridge.Dispose();
        }
    }
}