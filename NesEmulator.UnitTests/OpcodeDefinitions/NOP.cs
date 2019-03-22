using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class NOP
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpEA()
            {
                var sut = CreateSut();

                const int opValue = 0xEA;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.NOP);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
