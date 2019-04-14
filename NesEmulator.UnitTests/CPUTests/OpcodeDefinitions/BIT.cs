using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class BIT
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op24()
        {
            var sut = CreateSut();

            const int opValue = 0x24;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.BIT);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
        }

        [Fact]
        public void DefinitionExistsFor_Op2C()
        {
            var sut = CreateSut();

            const int opValue = 0x2C;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.BIT);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
        }
    }
}