using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class STY
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op84()
        {
            var sut = CreateSut();

            const int opValue = 0x84;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STY);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op8C()
        {
            var sut = CreateSut();

            const int opValue = 0x8C;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STY);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op94()
        {
            var sut = CreateSut();

            const int opValue = 0x94;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STY);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }
    }
}