using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class SBC
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpE9()
            {
                var sut = CreateSut();

                const int opValue = 0xE9;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.Immediate);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpE5()
            {
                var sut = CreateSut();

                const int opValue = 0xE5;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpF5()
            {
                var sut = CreateSut();

                const int opValue = 0xF5;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpED()
            {
                var sut = CreateSut();

                const int opValue = 0xED;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpFD()
            {
                var sut = CreateSut();

                const int opValue = 0xFD;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpF9()
            {
                var sut = CreateSut();

                const int opValue = 0xF9;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.AbsoluteY);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpE1()
            {
                var sut = CreateSut();

                const int opValue = 0xE1;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.IndirectX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }

            [Fact]
            public void DefinitionExistsFor_OpF1()
            {
                var sut = CreateSut();

                const int opValue = 0xF1;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.SBC);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow);
            }
        }
    }
}
