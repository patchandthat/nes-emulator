using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests
{
    public partial class OpcodeDefinitionsTests
    {
        public class CMP
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OpC9()
            {
                var sut = CreateSut();

                const int opValue = 0xC9;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.Immediate);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpC5()
            {
                var sut = CreateSut();

                const int opValue = 0xC5;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpD5()
            {
                var sut = CreateSut();

                const int opValue = 0xD5;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpCD()
            {
                var sut = CreateSut();

                const int opValue = 0xCD;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpDD()
            {
                var sut = CreateSut();

                const int opValue = 0xDD;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpD9()
            {
                var sut = CreateSut();

                const int opValue = 0xD9;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.AbsoluteY);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpC1()
            {
                var sut = CreateSut();

                const int opValue = 0xC1;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.IndirectX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }

            [Fact]
            public void DefinitionExistsFor_OpD1()
            {
                var sut = CreateSut();

                const int opValue = 0xD1;

                OpCode op = sut[opValue];

                op.Hex.Should().Be(opValue);
                op.Operation.Should().Be(Operation.CMP);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative);
            }
        }
    }
}
