using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class BCC
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op90()
            {
                var sut = CreateSut();

                const int opValue = 0x90;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.BCC);
                op.AddressMode.Should().Be(AddressMode.Relative);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
