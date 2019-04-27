using System;
using NesEmulator.APU;
using NesEmulator.PPU;
using NesEmulator.RomMappers;

namespace NesEmulator.Memory
{
    internal class MainMemory : IMemory
    {
        private readonly IReadWrite _zeroPageAndStack;
        private readonly IReadWrite _ram;
        private readonly IPpu _ppu;
        private readonly IApu _apu;
        private IReadWrite _rom;

        private const int PageSize = 0x100;

        private const ushort PpuMin = MemoryMap.PpuRegisters;
        private const ushort PpuMax = MemoryMap.PpuRegisterMirror - 1;
        
        private const ushort ApuMin = MemoryMap.ApuRegisters;
        private const ushort ApuMax = MemoryMap.JoyPad1 - 1;
        
        public MainMemory(IPpu ppu, IApu apu)
        {
            _ppu = ppu ?? throw new ArgumentNullException(nameof(ppu));
            _apu = apu ?? throw new ArgumentNullException(nameof(apu));
            
            var ramRange = new MemoryRange(MemoryMap.Ram, MemoryMap.RamMirror-1);
            var ram = new MemoryBlock(ramRange.Start, ramRange.End);
            var ramMirrorRange = new MemoryRange(MemoryMap.RamMirror, MemoryMap.PpuRegisters-1);

            _zeroPageAndStack = new MemoryBlock(0x0000, MemoryMap.Ram-1);
            _ram = new MemoryMirrorDecorator(ram, ramRange, ramMirrorRange);
        }

        public byte Read(ushort address)
        {
            var device = MemoryMappedDevice(address);

            return device.Read(address);
        }

        public byte Peek(ushort address)
        {
            var device = MemoryMappedDevice(address);

            return device.Peek(address);
        }

        public void Write(ushort address, byte value)
        {
            var device = MemoryMappedDevice(address);

            device.Write(address, value);
        }

        private IReadWrite MemoryMappedDevice(ushort address)
        {
            if (address < MemoryMap.Ram)
                return _zeroPageAndStack;
            
            if (address < MemoryMap.PpuRegisters)
                return _ram;
            
            if (address >= MemoryMap.PrgRomLowerBank)
            {
                return _rom ?? throw new MissingRomException();
            }
            
            if (address >= PpuMin && address <= PpuMax)
                return _ppu;
            
            if (address >= ApuMin && address <= ApuMax)
                return _apu;

            return null;
        }

        public void Load(ROM rom)
        {
            _rom = rom;
        }
    }
}