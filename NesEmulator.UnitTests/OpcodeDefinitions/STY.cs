using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class STY
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op84()
            {
                var sut = CreateSut();

                const int opValue = 0x84;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STX);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op94()
            {
                var sut = CreateSut();

                const int opValue = 0x94;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STX);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op8C()
            {
                var sut = CreateSut();

                const int opValue = 0x8C;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.STX);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
