using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class JMP
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op4C()
            {
                var sut = CreateSut();

                const int opValue = 0x4C;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.JMP);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }

            [Fact]
            public void DefinitionExistsFor_Op6C()
            {
                var sut = CreateSut();

                const int opValue = 0x6C;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.JMP);
                op.AddressMode.Should().Be(AddressMode.Indirect);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
}
