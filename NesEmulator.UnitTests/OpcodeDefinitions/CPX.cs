using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class CPX
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpE0()
            {
                var sut = CreateSut();

                const int opValue = 0xE0;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CPX);
                op.AddressMode.Should().Be(AddressMode.Immediate);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpE4()
            {
                var sut = CreateSut();

                const int opValue = 0xE4;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CPX);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpEC()
            {
                var sut = CreateSut();

                const int opValue = 0xEC;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CPX);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
