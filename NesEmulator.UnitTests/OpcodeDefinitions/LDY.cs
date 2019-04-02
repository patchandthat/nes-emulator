using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class LDY
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpA0()
            {
                var sut = CreateSut();

                const int opValue = 0xA0;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.LDY);
                op.AddressMode.Should().Be(AddressMode.Immediate);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpA4()
            {
                var sut = CreateSut();

                const int opValue = 0xA4;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.LDY);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpB4()
            {
                var sut = CreateSut();

                const int opValue = 0xB4;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.LDY);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpAC()
            {
                var sut = CreateSut();

                const int opValue = 0xAC;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.LDY);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpBC()
            {
                var sut = CreateSut();

                const int opValue = 0xBC;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.LDY);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
