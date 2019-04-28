using System;
using NesEmulator.APU;
using NesEmulator.Input;
using NesEmulator.PPU;
using NesEmulator.RomMappers;

namespace NesEmulator.Memory
{
    internal class MainMemory : IMemory
    {
        private readonly IReadWrite _ram;
        private readonly IReadWrite _ppu;
        private readonly IApu _apu;
        private readonly IInputSource _pad1;
        private readonly IInputSource _pad2;

        /// <summary>
        /// Use to cover the CPU test mode addresses
        /// </summary>
        private readonly IReadWrite _nullBlock = new NullMemoryBlock();

        private IReadWrite _rom;

        public MainMemory(IPpu ppu, IApu apu, IInputSource pad1, IInputSource pad2)
        {
            _ppu = ppu ?? throw new ArgumentNullException(nameof(ppu));
            _apu = apu ?? throw new ArgumentNullException(nameof(apu));
            _pad1 = pad1 ?? throw new ArgumentNullException(nameof(pad1));
            _pad2 = pad2 ?? throw new ArgumentNullException(nameof(pad2));

            _ppu = new MemoryMirrorDecorator(ppu, 
                new MemoryRange(MemoryMap.PpuRegisters, MemoryMap.PpuRegisterMirror-1), 
                new MemoryRange(MemoryMap.PpuRegisterMirror, MemoryMap.ApuRegisters-1));

            var ramRange = new MemoryRange(0x0000, MemoryMap.RamMirror - 1);
            var ram = new MemoryBlock(ramRange);
            var ramMirrorRange = new MemoryRange(MemoryMap.RamMirror, MemoryMap.PpuRegisters-1);
            
            _ram = new MemoryMirrorDecorator(ram, ramRange, ramMirrorRange);
        }

        public byte Read(ushort address)
        {
            var device = MapAddressToDevice(address);

            return device.Read(address);
        }

        public byte Peek(ushort address)
        {
            var device = MapAddressToDevice(address);

            return device.Peek(address);
        }

        public void Write(ushort address, byte value)
        {
            var device = MapAddressToDevice(address);

            device.Write(address, value);
        }

        private IReadWrite MapAddressToDevice(ushort address)
        {
            if (address < MemoryMap.PpuRegisters)
                return _ram;
            
            if (address >= MemoryMap.ExpansionRom)
            {
                return _rom ?? throw new MissingRomException();
            }
            
            if (address >= MemoryMap.PpuRegisters && address < MemoryMap.ApuRegisters)
                return _ppu;
            
            if (address >= MemoryMap.ApuRegisters && address < MemoryMap.JoyPad1)
                return _apu;

            if (address == MemoryMap.JoyPad1) return _pad1;

            if (address == MemoryMap.JoyPad2) return _pad2;

            return _nullBlock;
        }

        public void Load(ROM rom)
        {
            _rom = rom;
        }
    }
}