using System;
using FluentAssertions;
using NesEmulator.RomMappers;
using Xunit;

namespace NesEmulator.UnitTests.MapperTests
{
    public class RomHeaderTests
    {
        private byte[] _data;

        public RomHeaderTests()
        {
            _data = new byte[16]
            {
                0x4E, 0x45, 0x53, 0x1A,

                0x00, 0x00,

                0x00,
                0x00,
                0x00,
                0x00,
                0x00,

                0x00, 0x00, 0x00, 0x00, 0x00
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

        [Fact]
        public void ctor_ThrowsIfFirstFourBytesAreNotExpectedConstants()
        {
            Assert.False(true);
        }
    }
}
