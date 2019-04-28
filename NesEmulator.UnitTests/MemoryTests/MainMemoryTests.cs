using System;
using FakeItEasy;
using FluentAssertions;
using NesEmulator.APU;
using NesEmulator.Input;
using NesEmulator.Memory;
using NesEmulator.PPU;
using NesEmulator.RomMappers;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.MemoryTests
{
    public class MainMemoryTests
    {
        private static Random _rng;
        
        private IPpu _ppu;
        private IApu _apu;
        private IInputSource _pad1;
        private IInputSource _pad2;

        public MainMemoryTests()
        {
            _ppu = A.Fake<IPpu>();
            _apu = A.Fake<IApu>();
            _pad1 = A.Fake<IInputSource>();
            _pad2 = A.Fake<IInputSource>();
        }

        private MainMemory CreateSut()
        {
            return new MainMemory(_ppu, _apu, _pad1, _pad2);
        }
        
        private byte RandomByte()
        {
            if (_rng == null) _rng = new Random();

            return (byte) (_rng.Next() & 0xFF);
        }
        
        [Fact]
        public void ctor_WhenCalledWithNullPpu_WillThrow()
        {
            _ppu = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }
        
        [Fact]
        public void ctor_WhenCalledWithNullApu_WillThrow()
        {
            _apu = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_WhenCalledWithNullPad1_WillThrowArgumentNullException()
        {
            _pad1 = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_WhenCalledWithNullPad2_WillThrowArgumentNullException()
        {
            _pad2 = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(MemoryMap.PpuControl, 0x25)]
        [InlineData(MemoryMap.PpuMask, 0x58)]
        [InlineData(MemoryMap.PpuStatus, 0xF2)]
        [InlineData(MemoryMap.OamAddr, 0xD6)]
        [InlineData(MemoryMap.OamData, 0x7E)]
        [InlineData(MemoryMap.PpuScroll, 0x99)]
        [InlineData(MemoryMap.PpuAddr, 0xFA)]
        [InlineData(MemoryMap.PpuData, 0x44)]
        public void WritesToPPURegisters_ShouldForwardToPPU(ushort ppuAddress, byte value)
        {
            var sut = CreateSut();
            
            sut.Write(ppuAddress, value);

            A.CallTo(() => _ppu.Write(ppuAddress, value))
                .MustHaveHappened();
        }
        
        [Theory]
        [InlineData(MemoryMap.PpuControl, 0x25)]
        [InlineData(MemoryMap.PpuMask, 0x58)]
        [InlineData(MemoryMap.PpuStatus, 0xF2)]
        [InlineData(MemoryMap.OamAddr, 0xD6)]
        [InlineData(MemoryMap.OamData, 0x7E)]
        [InlineData(MemoryMap.PpuScroll, 0x99)]
        [InlineData(MemoryMap.PpuAddr, 0xFA)]
        [InlineData(MemoryMap.PpuData, 0x44)]
        public void ReadsFromPPURegisters_ShouldForwardToPPU(ushort ppuAddress, byte value)
        {
            var sut = CreateSut();
            
            A.CallTo(() => _ppu.Read(ppuAddress))
                .Returns(value);
            
            var result = sut.Read(ppuAddress);

            result.Should().Be(value);
        }
        
        [Theory]
        [InlineData(MemoryMap.PpuControl, 0x25)]
        [InlineData(MemoryMap.PpuMask, 0x58)]
        [InlineData(MemoryMap.PpuStatus, 0xF2)]
        [InlineData(MemoryMap.OamAddr, 0xD6)]
        [InlineData(MemoryMap.OamData, 0x7E)]
        [InlineData(MemoryMap.PpuScroll, 0x99)]
        [InlineData(MemoryMap.PpuAddr, 0xFA)]
        [InlineData(MemoryMap.PpuData, 0x44)]
        public void PeeksFromPPURegisters_ShouldForwardToPPU(ushort ppuAddress, byte value)
        {
            var sut = CreateSut();
            
            A.CallTo(() => _ppu.Peek(ppuAddress))
                .Returns(value);
            
            var result = sut.Peek(ppuAddress);

            result.Should().Be(value);
        }
        
        [Theory]
        [InlineData(0x4000)]
        [InlineData(0x4001)]
        [InlineData(0x4002)]
        [InlineData(0x4003)]
        [InlineData(0x4004)]
        [InlineData(0x4005)]
        [InlineData(0x4006)]
        [InlineData(0x4007)]
        [InlineData(0x4008)]
        [InlineData(0x4009)]
        [InlineData(0x400A)]
        [InlineData(0x400B)]
        [InlineData(0x400C)]
        [InlineData(0x400D)]
        [InlineData(0x400E)]
        [InlineData(0x400F)]
        [InlineData(0x4010)]
        [InlineData(0x4011)]
        [InlineData(0x4012)]
        [InlineData(0x4013)]
        [InlineData(0x4014)]
        [InlineData(0x4015)]
        public void WritesToAPURegisters_ShouldForwardToAPU(ushort ppuAddress)
        {
            byte value = RandomByte();
            
            var sut = CreateSut();
            
            sut.Write(ppuAddress, value);

            A.CallTo(() => _apu.Write(ppuAddress, value))
                .MustHaveHappened();
        }

        [Theory]
        [InlineData(0x4000)]
        [InlineData(0x4001)]
        [InlineData(0x4002)]
        [InlineData(0x4003)]
        [InlineData(0x4004)]
        [InlineData(0x4005)]
        [InlineData(0x4006)]
        [InlineData(0x4007)]
        [InlineData(0x4008)]
        [InlineData(0x4009)]
        [InlineData(0x400A)]
        [InlineData(0x400B)]
        [InlineData(0x400C)]
        [InlineData(0x400D)]
        [InlineData(0x400E)]
        [InlineData(0x400F)]
        [InlineData(0x4010)]
        [InlineData(0x4011)]
        [InlineData(0x4012)]
        [InlineData(0x4013)]
        [InlineData(0x4014)]
        [InlineData(0x4015)]
        public void ReadsFromAPURegisters_ShouldForwardToAPU(ushort ppuAddress)
        {
            byte value = RandomByte();
            
            var sut = CreateSut();
            
            A.CallTo(() => _apu.Read(ppuAddress))
                .Returns(value);
            
            var result = sut.Read(ppuAddress);

            result.Should().Be(value);
        }
        
        [Theory]
        [InlineData(0x4000)]
        [InlineData(0x4001)]
        [InlineData(0x4002)]
        [InlineData(0x4003)]
        [InlineData(0x4004)]
        [InlineData(0x4005)]
        [InlineData(0x4006)]
        [InlineData(0x4007)]
        [InlineData(0x4008)]
        [InlineData(0x4009)]
        [InlineData(0x400A)]
        [InlineData(0x400B)]
        [InlineData(0x400C)]
        [InlineData(0x400D)]
        [InlineData(0x400E)]
        [InlineData(0x400F)]
        [InlineData(0x4010)]
        [InlineData(0x4011)]
        [InlineData(0x4012)]
        [InlineData(0x4013)]
        [InlineData(0x4014)]
        [InlineData(0x4015)]
        public void PeeksFromAPURegisters_ShouldForwardToAPU(ushort ppuAddress)
        {
            byte value = RandomByte();
            
            var sut = CreateSut();
            
            A.CallTo(() => _apu.Peek(ppuAddress))
                .Returns(value);
            
            var result = sut.Peek(ppuAddress);

            result.Should().Be(value);
        }

        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomReads_WhenNoRomLoaded_WillThrow(ushort address)
        {
            var sut = CreateSut();

            Action action = () => sut.Read(address);

            action.Should().Throw<MissingRomException>();
        }
        
        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomPeeks_WhenNoRomLoaded_WillThrow(ushort address)
        {
            var sut = CreateSut();

            Action action = () => sut.Peek(address);

            action.Should().Throw<MissingRomException>();
        }
        
        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomWrites_WhenNoRomLoaded_WillThrow(ushort address)
        {
            var sut = CreateSut();

            Action action = () => sut.Write(address, 0xFF);

            action.Should().Throw<MissingRomException>();
        }
        
        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomReads_WhenRomLoaded_WillReadRomAddress(ushort address)
        {
            byte value = RandomByte();
            
            ROM rom = A.Fake<ROM>();

            var sut = CreateSut();
            
            sut.Load(rom);

            A.CallTo(() => rom.Read(address))
                .Returns(value);

            var result = sut.Read(address);

            result.Should().Be(value);
        }
        
        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomPeeks_WhenRomLoaded_WillPeekRomAddress(ushort address)
        {
            byte value = RandomByte();
            
            ROM rom = A.Fake<ROM>();

            var sut = CreateSut();
            
            sut.Load(rom);

            A.CallTo(() => rom.Peek(address))
                .Returns(value);

            var result = sut.Peek(address);

            result.Should().Be(value);
        }
        
        [Theory]
        [ClassData(typeof(AllCartridgePages))]
        public void RomWrites_WhenRomLoaded_WillWriteToRomAddress(ushort address)
        {
            byte value = RandomByte();
            
            ROM rom = A.Fake<ROM>();

            var sut = CreateSut();
            
            sut.Load(rom);

            sut.Write(address, value);
            
            A.CallTo(() => rom.Write(address, value))
                .MustHaveHappened();
        }

        [Theory]
        [ClassData(typeof(ZeroPageStackAndRamAddresses))]
        public void MemoryArea_ReadsReturnLastWrittenValue(ushort address)
        {
            byte value = RandomByte();

            var sut = CreateSut();

            sut.Read(address)
                .Should().Be(0x00);
            
            sut.Write(address, value);

            sut.Peek(address)
                .Should().Be(value);
            sut.Read(address)
                .Should().Be(value);
        }

        [Theory]
        [InlineData(0x00, 0x00)]
        [InlineData(0x800, 0x00)]
        [InlineData(0x1000, 0x00)]
        [InlineData(0x1800, 0x00)]
        [InlineData(0x1FFF, 0x7FF)]
        [InlineData(0x17FF, 0x7FF)]
        [InlineData(0x0FFF, 0x7FF)]
        [InlineData(0x07FF, 0x07FF)]
        public void RamMirror_ReadsFromMirroredAreaShouldMapBackToRAM(ushort address, ushort expectedReadAddress)
        {
            var sut = CreateSut();

            byte value = RandomByte();

            sut.Write(address, value);

            var result = sut.Read(expectedReadAddress);

            result.Should().Be(value);
        }

        [Theory]
        [ClassData(typeof(PpuAndMirrorAddresses))]
        public void PpuMirrors_AddressRange_x2000Tox2007_IsMirroredBy_x2008Tox3FFF(ushort address)
        {
            ushort expectedSourceAddress = (ushort) ((address % 8) + 0x2000);

            var sut = CreateSut();

            sut.Write(address, 0xFF);

            A.CallTo(() => _ppu.Write(expectedSourceAddress, 0xFF))
                .MustHaveHappened();

            A.CallTo(() => _ppu.Write(A<ushort>._, A<byte>._))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Address4016_ShouldAddressPad1()
        {
            ushort address = 0x4016;
            byte value = RandomByte();

            var sut = CreateSut();

            sut.Read(address);
            sut.Peek(address);
            sut.Write(address, value);

            A.CallTo(() => _pad1.Read(address))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _pad1.Peek(address))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _pad1.Write(address, value))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void Address4017_ShouldAddressPad1()
        {
            ushort address = 0x4017;
            byte value = RandomByte();

            var sut = CreateSut();

            sut.Read(address);
            sut.Peek(address);
            sut.Write(address, value);

            A.CallTo(() => _pad2.Read(address))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _pad2.Peek(address))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _pad2.Write(address, value))
                .MustHaveHappenedOnceExactly();
        }
    }
}    