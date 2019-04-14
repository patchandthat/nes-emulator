using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    public class NOP
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_OpEA()
        {
            var sut = CreateSut();

            const int opValue = 0xEA;

            var op = sut[opValue];

            op.Value.Should().Be(opValue);
            op.Operation.Should().Be(Operation.NOP);
            op.AddressMode.Should().Be(AddressMode.Implicit);
            op.Bytes.Should().Be(1);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.None);
        }
    }
}