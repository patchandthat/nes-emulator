using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class EOR
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op41()
        {
            var sut = CreateSut();

            const int opValue = 0x41;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.IndirectX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op45()
        {
            var sut = CreateSut();

            const int opValue = 0x45;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op49()
        {
            var sut = CreateSut();

            const int opValue = 0x49;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.Immediate);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op4D()
        {
            var sut = CreateSut();

            const int opValue = 0x4D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op51()
        {
            var sut = CreateSut();

            const int opValue = 0x51;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.IndirectY);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op55()
        {
            var sut = CreateSut();

            const int opValue = 0x55;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op59()
        {
            var sut = CreateSut();

            const int opValue = 0x59;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.AbsoluteY);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }

        [Fact]
        public void DefinitionExistsFor_Op5D()
        {
            var sut = CreateSut();

            const int opValue = 0x5D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.EOR);
            op.AddressMode.Should().Be(AddressMode.AbsoluteX);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
        }
    }
}