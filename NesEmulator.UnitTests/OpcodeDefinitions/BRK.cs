using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class BRK
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op00()
            {
                var sut = CreateSut();

                const int opValue = 0x00;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.BRK);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(7);
                op.AffectsFlags.Should().Be(StatusFlags.Bit4 | StatusFlags.Bit5);
            }
        }
    }
}
