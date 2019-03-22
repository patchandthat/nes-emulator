using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class STA
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op85()
            {
                var sut = CreateSut();

                const int opValue = 0x85;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op95()
            {
                var sut = CreateSut();

                const int opValue = 0x95;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op8D()
            {
                var sut = CreateSut();

                const int opValue = 0x8D;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op9D()
            {
                var sut = CreateSut();

                const int opValue = 0x9D;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op99()
            {
                var sut = CreateSut();

                const int opValue = 0x99;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.AbsoluteY);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op81()
            {
                var sut = CreateSut();

                const int opValue = 0x81;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.IndirectX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op91()
            {
                var sut = CreateSut();

                const int opValue = 0x91;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STA);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
