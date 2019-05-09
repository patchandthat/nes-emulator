using System;
using FluentAssertions;
using NesEmulator.RomMappers;
using Xunit;

namespace NesEmulator.UnitTests.MapperTests
{
    [Trait("Category", "Unit")]
    public class RomHeaderTests
    {
        private byte[] _data;

        private byte _byte0;
        private byte _byte1;
        private byte _byte2;
        private byte _byte3;
        private byte _byte4;
        private byte _byte5;
        private byte _byte6;
        private byte _byte7;
        private byte _byte8;
        private byte _byte9;
        private byte _byte10;
        private byte _byte11;
        private byte _byte12;
        private byte _byte13;
        private byte _byte14;
        private byte _byte15;

        public RomHeaderTests()
        {
            _byte0 = 0x4E;
            _byte1 = 0x45;
            _byte2 = 0x53;
            _byte3 = 0x1A;
            _byte4 = 0x00;
            _byte5 = 0x00;
            _byte6 = 0x00;
            _byte7 = 0x00;
            _byte8 = 0x00;
            _byte9 = 0x00;
            _byte10 = 0x00;
            _byte11 = 0x00;
            _byte12 = 0x00;
            _byte13 = 0x00;
            _byte14 = 0x00;
            _byte15 = 0x00;
        }

        private void CreateHeaderData()
        {
            _data = new byte[16]
            {
                _byte0, _byte1, _byte2, _byte3,

                _byte4, _byte5,

                _byte6,
                _byte7,
                _byte8,
                _byte9,
                _byte10,

                _byte11, _byte12, _byte13, _byte14, _byte15
            };
        }

        private RomHeader CreateSut()
        {
            return new RomHeader(_data);
        }

        [Fact]
        public void ctor_WhenCalledWithNullBytes_WillThrow()
        {
            _data = null;

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ctor_WhenLessThan16Bytes_WillThrow()
        {
            _data = new byte[15];

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ctor_WhenMoreThan16Bytes_WillThrow()
        {
            _data = new byte[17];

            Action action = () => CreateSut();

            action.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x4D, true)]
        [InlineData(0x4F, true)]
        [InlineData(0xFF, true)]
        [InlineData(0x4E, false)]
        public void ctor_ThrowsIfFirstByteIsNot4E(byte value, bool throws)
        {
            _byte0 = value;

            CreateHeaderData();

            Action action = () => CreateSut();

            if (throws)
                action.Should().Throw<ArgumentException>();
            else
                action.Should().NotThrow();
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x44, true)]
        [InlineData(0x46, true)]
        [InlineData(0xFF, true)]
        [InlineData(0x45, false)]
        public void ctor_ThrowsIfSecondByteIsNot45(byte value, bool throws)
        {
            _byte1 = value;

            CreateHeaderData();

            Action action = () => CreateSut();

            if (throws)
                action.Should().Throw<ArgumentException>();
            else
                action.Should().NotThrow();
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x52, true)]
        [InlineData(0x54, true)]
        [InlineData(0xFF, true)]
        [InlineData(0x53, false)]
        public void ctor_ThrowsIfThirdByteIsNot53(byte value, bool throws)
        {
            _byte2 = value;

            CreateHeaderData();

            Action action = () => CreateSut();

            if (throws)
                action.Should().Throw<ArgumentException>();
            else
                action.Should().NotThrow();
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x10, true)]
        [InlineData(0x1B, true)]
        [InlineData(0xFF, true)]
        [InlineData(0x1A, false)]
        public void ctor_ThrowsIfFourthByteIsNot1A(byte value, bool throws)
        {
            _byte3 = value;

            CreateHeaderData();

            Action action = () => CreateSut();

            if (throws)
                action.Should().Throw<ArgumentException>();
            else
                action.Should().NotThrow();
        }

        [Theory]
        [InlineData(0x01)]
        [InlineData(0x0A)]
        [InlineData(0x15)]
        public void ctor_ParsesByte4AsNumberOfPrgBanks(byte value)
        {
            _byte4 = value;

            CreateHeaderData();

            var sut = CreateSut();

            sut.PrgRomBanks.Should().Be(value);
        }

        [Theory]
        [InlineData(0x01)]
        [InlineData(0x0A)]
        [InlineData(0x15)]
        public void ctor_ParsesByte5AsNumberOfChrBanks(byte value)
        {
            _byte5 = value;

            CreateHeaderData();

            var sut = CreateSut();

            sut.ChrRomBanks.Should().Be(value);
        }

        [Fact]
        public void Byte6Bit0DenotesMirroringTypeHorizontalWhenBitIsLow()
        {
            CreateHeaderData();

            var sut = CreateSut();

            sut.NametableMirroring.Should().Be(NametableMirrorType.Horizontal);
        }

        [Fact]
        public void Byte6Bit0DenotesMirroringTypeVerticalWhenBitIsHigh()
        {
            _byte6 = 0b0000_0001;
            
            CreateHeaderData();

            var sut = CreateSut();

            sut.NametableMirroring.Should().Be(NametableMirrorType.Vertical);
        }

        [Fact]
        public void Byte6Bit3IndicatesMirroringIsIgnored()
        {
            _byte6 = 0b0000_1000;
            
            CreateHeaderData();

            var sut = CreateSut();

            sut.NametableMirroring.Should().Be(NametableMirrorType.None);
        }

        [Theory]
        [InlineData(0b0000_0000, false)]
        [InlineData(0b0000_0010, true)]
        public void Byte6Bit1IndicatesCartridgeHas8kOnboardRamSpace(byte byte6, bool expectedCartridgePersistence)
        {
            _byte6 = byte6;
            
            CreateHeaderData();

            var sut = CreateSut();

            sut.HasBatteryPrgRam.Should().Be(expectedCartridgePersistence);
        }

        [Theory]
        [InlineData(0b0000_0000, false)]
        [InlineData(0b0000_0100, true)]
        public void Byte6Bit2IndicatesPresenceOf512ByteTrainer(byte byte6, bool hasTrainer)
        {
            _byte6 = byte6;
            
            CreateHeaderData();

            var sut = CreateSut();

            sut.HasTrainer.Should().Be(hasTrainer);
        }

        [Theory]
        [InlineData(0b0001_1111, 0b0000_0000, 0x01)]
        [InlineData(0b0110_0000, 0b0111_1111, 0x76)]
        [InlineData(0b1110_0000, 0b1010_0000, 0xAE)]
        public void ctor_DeterminesMapperNumberFromBytes6And7(byte b6, byte b7, byte expectedMapperNumber)
        {
            // High nybble of byte 6 is the low nybble of the mapper
            // High nybble of byte 7 is the high nybble of the mapper

            _byte6 = b6;
            _byte7 = b7;
            
            CreateHeaderData();

            var sut = CreateSut();

            sut.MapperNumber.Should().Be(expectedMapperNumber);
        }

        // Todo
        public void Byte7Bits2And3UsedToDetermineiNesVersion()
        {
            /* Recommended detection procedure:
                If byte 7 AND $0C = $08, and the size taking into account byte 9 
                    does not exceed the actual size of the ROM image, then NES 2.0.
                If byte 7 AND $0C = $00, and bytes 12-15 are all 0, then iNES.
                Otherwise, archaic iNES.
             */
        }

        // Todo
        public void Byte7Bit0IndicatesVsUniSystemRom()
        {
        }

        // Todo
        public void Byte7Bit1IndicatesPlayChoice10Rom()
        {
            Assert.True(false, "Todo: ");
        }
    }
}