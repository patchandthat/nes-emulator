using System;
using System.IO;
using FluentAssertions;
using NesEmulator.RomMappers;
using NesEmulator.RomMappers.Parsers;
using Xunit;

namespace NesEmulator.UnitTests.MapperTests.Parsers
{
    public class iNesParserTests
    {
        internal class SpyRom : ROM
        {
            public SpyRom(RomHeader header, Memory<byte> content)
            {
            }

            public override byte Read(ushort address)
            {
                return 0x0;
            }

            public override byte Peek(ushort address)
            {
                return 0x0;
            }

            public override void Write(ushort address, byte value)
            {
                
            }
        }

        private iNesParser CreateSut()
        {
            return new iNesParser();
        }

        [Fact]
        public void Parse_WhenStreamDoesNotStartWithValidHeader_WillThrowArgumentException()
        {
            var inputStream = new MemoryStream(new byte[1024]);

            var sut = CreateSut();

            Action action = () => sut.Parse(inputStream);

            action.Should().Throw<RomParseException>();
        }
    }
}
