using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class CLI
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op58()
            {
                var sut = CreateSut();

                const int opValue = 0x58;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CLI);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.InterruptDisable);
            }
        }
    }
}
