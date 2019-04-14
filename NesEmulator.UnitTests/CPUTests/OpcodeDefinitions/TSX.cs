using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    
    
        public class TSX
        {
            private OpCodes CreateSut()
            {
                return new OpCodes();
            }

            [Fact]
            public void DefinitionExistsFor_OpBA()
            {
                var sut = CreateSut();

                const int opValue = 0xBA;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.TSX);
                op.AddressMode.Should().Be(AddressMode.Implicit);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }

