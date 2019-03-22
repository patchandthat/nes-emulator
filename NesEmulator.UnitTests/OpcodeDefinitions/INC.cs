using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class INC
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpE6()
            {
                var sut = CreateSut();

                const int opValue = 0xE6;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.INC);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }


            [Fact]
            public void DefinitionExistsFor_OpF6()
            {
                var sut = CreateSut();

                const int opValue = 0xF6;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.INC);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }


            [Fact]
            public void DefinitionExistsFor_OpEE()
            {
                var sut = CreateSut();

                const int opValue = 0xEE;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.INC);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }


            [Fact]
            public void DefinitionExistsFor_OpFE()
            {
                var sut = CreateSut();

                const int opValue = 0xFE;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.INC);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(7);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
