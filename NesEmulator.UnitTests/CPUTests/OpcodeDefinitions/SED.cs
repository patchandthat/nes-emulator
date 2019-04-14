using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    
        public class SED
        {
            private OpCodes CreateSut()
            {
                return new OpCodes();
            }

            [Fact]
            public void DefinitionExistsFor_OpF8()
            {
                var sut = CreateSut();

                const int opValue = 0xF8;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SED);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Decimal);
            }
        }
    }
