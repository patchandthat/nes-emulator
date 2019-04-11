using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class BEQ
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpF0()
            {
                var sut = CreateSut();

                const int opValue = 0xF0;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.BEQ);
                op.AddressMode.Should().Be(AddressMode.Relative);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
