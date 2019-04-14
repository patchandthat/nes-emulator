using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    
    
        public class RTS
        {
            private OpCodes CreateSut()
            {
                return new OpCodes();
            }

            [Fact]
            public void DefinitionExistsFor_Op60()
            {
                var sut = CreateSut();

                const int opValue = 0x60;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.RTS);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.None);
            }
        }
    }
