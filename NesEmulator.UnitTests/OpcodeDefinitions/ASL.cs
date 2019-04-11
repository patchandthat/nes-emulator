using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class ASL
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op0A()
            {
                var sut = CreateSut();

                const int opValue = 0x0A;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.ASL);
                op.AddressMode.Should().Be(AddressMode.Accumulator);
                op.Bytes.Should().Be(1);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_Op06()
            {
                var sut = CreateSut();

                const int opValue = 0x06;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.ASL);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }


            [Fact]
            public void DefinitionExistsFor_Op16()
            {
                var sut = CreateSut();

                const int opValue = 0x16;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.ASL);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_Op0E()
            {
                var sut = CreateSut();

                const int opValue = 0x0E;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.ASL);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_Op1E()
            {
                var sut = CreateSut();

                const int opValue = 0x1E;

                OpCode op = sut[opValue];

                op.Value.Should().Be(opValue);
                op.Operation.Should().Be(Operation.ASL);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(7);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
