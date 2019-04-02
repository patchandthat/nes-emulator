using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class PHP
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op08()
            {
                var sut = CreateSut();

                const int opValue = 0x08;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.PHP);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.Bit4 | StatusFlags.Bit5);
            }
        }
    }
}
