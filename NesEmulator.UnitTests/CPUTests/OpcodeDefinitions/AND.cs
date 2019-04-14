using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeDefinitions
{
    
    
        public class AND
        {
            private OpCodes CreateSut()
            {
                return new OpCodes();
            }

            [Fact]
            public void DefinitionExistsFor_Op29()
            {
                var sut = CreateSut();

                OpCode op = sut[0x29];

                op.Value.Should().Be(0x29);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.Immediate);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(2);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op25()
            {
                var sut = CreateSut();

                OpCode op = sut[0x25];

                op.Value.Should().Be(0x25);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.ZeroPage);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(3);
                op.Operation.Should().Be(Operation.AND);
            }
            [Fact]
            public void DefinitionExistsFor_Op35()
            {
                var sut = CreateSut();

                OpCode op = sut[0x35];

                op.Value.Should().Be(0x35);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.ZeroPageX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(4);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op2D()
            {
                var sut = CreateSut();

                OpCode op = sut[0x2D];

                op.Value.Should().Be(0x2D);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.Absolute);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op3D()
            {
                var sut = CreateSut();

                OpCode op = sut[0x3D];

                op.Value.Should().Be(0x3D);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.AbsoluteX);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op39()
            {
                var sut = CreateSut();

                OpCode op = sut[0x39];

                op.Value.Should().Be(0x39);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.AbsoluteY);
                op.Bytes.Should().Be(3);
                op.Cycles.Should().Be(4);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op21()
            {
                var sut = CreateSut();

                OpCode op = sut[0x21];

                op.Value.Should().Be(0x21);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.IndirectX);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(6);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void DefinitionExistsFor_Op31()
            {
                var sut = CreateSut();

                OpCode op = sut[0x31];

                op.Value.Should().Be(0x31);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.Operation.Should().Be(Operation.AND);
            }
        }
    
}
