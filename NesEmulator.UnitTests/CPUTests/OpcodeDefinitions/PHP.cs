using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class PHP
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op08()
        {
            var sut = CreateSut();

            const int opValue = 0x08;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.PHP);
            op.AddressMode.Should().Be(AddressMode.Implicit);
            op.Bytes.Should().Be(1);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Bit4 | StatusFlags.Bit5);
        }
    }
}