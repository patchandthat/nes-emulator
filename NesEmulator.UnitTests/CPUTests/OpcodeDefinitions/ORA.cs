using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class ORA
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op01()
        {
            var sut = CreateSut();

            const int opValue = 0x01;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.IndirectX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op05()
        {
            var sut = CreateSut();

            const int opValue = 0x05;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op09()
        {
            var sut = CreateSut();

            const int opValue = 0x09;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.Immediate);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op0D()
        {
            var sut = CreateSut();

            const int opValue = 0x0D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op11()
        {
            var sut = CreateSut();

            const int opValue = 0x11;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.IndirectY);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op15()
        {
            var sut = CreateSut();

            const int opValue = 0x15;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op19()
        {
            var sut = CreateSut();

            const int opValue = 0x19;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteY);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op1D()
        {
            var sut = CreateSut();

            const int opValue = 0x1D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.ORA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteX);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }
    }
}