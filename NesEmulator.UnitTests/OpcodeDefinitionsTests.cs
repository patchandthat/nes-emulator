using FluentAssertions;
using Xunit;

namespace NesEmulator.UnitTests
{
    public class OpcodeDefinitionsTests
    {
        public class ADC
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_OP69()
            {
                var sut = CreateSut();

                OpCode op = sut[0x69];

                op.Hex.Should().Be(0x69);
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

                op.Hex.Should().Be(0x65);
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

                op.Hex.Should().Be(0x75);
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

                op.Hex.Should().Be(0x6D);
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

                op.Hex.Should().Be(0x7D);
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

                op.Hex.Should().Be(0x79);
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

                op.Hex.Should().Be(0x61);
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

                op.Hex.Should().Be(0x71);
                op.Operation.Should().Be(Operation.ADC);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry);
            }
        }
        
        public class LDA
        {
            [Fact]
            public void DefinitionExistsFor_OpA9()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpA5()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpB5()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpAD()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpBD()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpB9()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpA1()
            {
                Assert.True(false, "Write me");
            }

            [Fact]
            public void DefinitionExistsFor_OpB1()
            {
                Assert.True(false, "Write me");
            }
        }

        public class LDX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class LDY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class STA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class STX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class STY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TAX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TAY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TXA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TYA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TSX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class TXS
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class PHA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class PHP
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class PLA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class PLP
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class AND
        {
            private OpcodeDefinitions CreateSut()
            {
                return new OpcodeDefinitions();
            }

            [Fact]
            public void DefinitionExistsFor_Op29()
            {
                var sut = CreateSut();

                OpCode op = sut[0x29];

                op.Hex.Should().Be(0x29);
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

                op.Hex.Should().Be(0x25);
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

                op.Hex.Should().Be(0x35);
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

                op.Hex.Should().Be(0x2D);
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

                op.Hex.Should().Be(0x3D);
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

                op.Hex.Should().Be(0x39);
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

                op.Hex.Should().Be(0x21);
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

                op.Hex.Should().Be(0x31);
                op.AffectsFlags.Should().Be(StatusFlags.Zero | StatusFlags.Negative);
                op.AddressMode.Should().Be(AddressMode.IndirectY);
                op.Bytes.Should().Be(2);
                op.Cycles.Should().Be(5);
                op.Operation.Should().Be(Operation.AND);
            }

            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class EOR
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class ORA
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BIT
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class SBC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CMP
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CPX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CPY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class INC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class INX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class INY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class DEC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class DEX
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class DEY
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class ASL
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class LSR
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class ROL
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class ROR
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class JMP
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class JSR
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class RTS
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BCC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BCS
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BEQ
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BMI
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BNE
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BPL
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BVC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BVS
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CLC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CLD
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CLI
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class CLV
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class SEC
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class SED
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class SEI
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class BRK
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class NOP
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class RTI
        {
            [Fact]
            public void ToDo()
            {
                Assert.True(false, "Write me");
            }
        }

        public class Reference
        {
            [Fact]
            public void Implement_remaining_operations()
            {
                Assert.True(false, "See ref http://www.obelisk.me.uk/6502/reference.html");
            }
        }
    }
}
