using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class PHA
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op48()
            {
                var sut = CreateSut();

                const int opValue = 0x48;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.PHA);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
