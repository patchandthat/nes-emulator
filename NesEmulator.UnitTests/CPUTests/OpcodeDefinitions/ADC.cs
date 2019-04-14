using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{

    public class ADC
    {
        private OpCodes CreateSut()
        {
            return new OpCodes();
        }

        [Fact]
        public void DefinitionExistsFor_OP69()
        {
            var sut = CreateSut();

            OpCode op = sut[0x69];

            op.Value.Should().Be(0x69);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.Immediate);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(2);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP65()
        {
            var sut = CreateSut();

            OpCode op = sut[0x65];

            op.Value.Should().Be(0x65);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.ZeroPage);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(3);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP75()
        {
            var sut = CreateSut();

            OpCode op = sut[0x75];

            op.Value.Should().Be(0x75);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.ZeroPageX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP6D()
        {
            var sut = CreateSut();

            OpCode op = sut[0x6D];

            op.Value.Should().Be(0x6D);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.Absolute);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP7D()
        {
            var sut = CreateSut();

            OpCode op = sut[0x7D];

            op.Value.Should().Be(0x7D);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.AbsoluteX);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP79()
        {
            var sut = CreateSut();

            OpCode op = sut[0x79];

            op.Value.Should().Be(0x79);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.AbsoluteY);
            op.Bytes.Should().Be(3);
            op.Cycles.Should().Be(4);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP61()
        {
            var sut = CreateSut();

            OpCode op = sut[0x61];

            op.Value.Should().Be(0x61);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.IndirectX);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(6);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }

        [Fact]
        public void DefinitionExistsFor_OP71()
        {
            var sut = CreateSut();

            OpCode op = sut[0x71];

            op.Value.Should().Be(0x71);
            op.Operation.Should().Be(Operation.ADC);
            op.AddressMode.Should().Be(AddressMode.IndirectY);
            op.Bytes.Should().Be(2);
            op.Cycles.Should().Be(5);
            op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
        }
    }
}