using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class DEY
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op88()
            {
                var sut = CreateSut();

                const int opValue = 0x88;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.DEY);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
