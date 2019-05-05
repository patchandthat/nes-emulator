using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class STA
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_Op81()
        {
            var sut = CreateSut();

            const int opValue = 0x81;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.IndirectX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op85()
        {
            var sut = CreateSut();

            const int opValue = 0x85;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op8D()
        {
            var sut = CreateSut();

            const int opValue = 0x8D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op91()
        {
            var sut = CreateSut();

            const int opValue = 0x91;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.IndirectY);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op95()
        {
            var sut = CreateSut();

            const int opValue = 0x95;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op99()
        {
            var sut = CreateSut();

            const int opValue = 0x99;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteY);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }

        [Fact]
        public void DefinitionExistsFor_Op9D()
        {
            var sut = CreateSut();

            const int opValue = 0x9D;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.STA);
            op.AddressMode.Should().Be(AddressMode.AbsoluteX);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }
    }
}