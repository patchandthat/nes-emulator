using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class BIT
    {
        [Trait("Category", "Unit")]
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.BIT, AddressMode.ZeroPage);

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x00)]
            [InlineData(0x00, 0xFF)]
            [InlineData(0xFF, 0x00)]
            [InlineData(0x0F, 0xF0)]
            public void ZeroFlagRaisedIfBitwiseAndBetweenAccumulatorAndOperandIsZero(byte accumulator, byte operand)
            {
                byte zeroPageAddress = 0x67;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x74, 0xFF)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x01, 0x01)]
            [InlineData(0x0F, 0xF3)]
            public void ZeroFlagClearedIfBitwiseAndBetweenAccumulatorAndOperandIsNotZero(byte accumulator, byte operand)
            {
                byte zeroPageAddress = 0xAE;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x74, 0xFF)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x01, 0x01)]
            [InlineData(0x0F, 0xF3)]
            public void AccumulatorValueIsUnchanged(byte accumulator, byte operand)
            {
                byte zeroPageAddress = 0x01;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(accumulator);
            }
            
            [Theory]
            [InlineData(0x74, 0xFF, true)]
            [InlineData(0xFF, 0b0100_0000, true)]
            [InlineData(0x01, 0b1011_1111, false)]
            [InlineData(0x0F, 0x00, false)]
            public void OverflowFlagMatchesOperandBitSixState(byte accumulator, byte operand, bool expectFlagRaised)
            {
                byte zeroPageAddress = 0xFF;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(0x74, 0xFF, true)]
            [InlineData(0xFF, 0b1000_0000, true)]
            [InlineData(0x01, 0b0111_1111, false)]
            [InlineData(0x0F, 0x00, false)]
            public void NegativeFlagMatchesOperandBitSevenState(byte accumulator, byte operand, bool expectFlagRaised)
            {
                byte zeroPageAddress = 0x99;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void CarryFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Carry;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void InterruptFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.InterruptDisable;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DecimalFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Decimal;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void BreakFlagsNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Bit4 | StatusFlags.Bit5;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Fact]
            public void InstructionPointerMovesTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesThreeCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
        
        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.BIT, AddressMode.Absolute);

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x00)]
            [InlineData(0x00, 0xFF)]
            [InlineData(0xFF, 0x00)]
            [InlineData(0x0F, 0xF0)]
            public void ZeroFlagRaisedIfBitwiseAndBetweenAccumulatorAndOperandIsZero(byte accumulator, byte operand)
            {
                byte lowByte = 0x67;
                byte highByte = 0x00;
                ushort operandAddress = 0x0067;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x74, 0xFF)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x01, 0x01)]
            [InlineData(0x0F, 0xF3)]
            public void ZeroFlagClearedIfBitwiseAndBetweenAccumulatorAndOperandIsNotZero(byte accumulator, byte operand)
            {
                byte lowByte = 0x59;
                byte highByte = 0x03;
                ushort operandAddress = 0x0359;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x74, 0xFF)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x01, 0x01)]
            [InlineData(0x0F, 0xF3)]
            public void AccumulatorValueIsUnchanged(byte accumulator, byte operand)
            {
                byte lowByte = 0x59;
                byte highByte = 0x03;
                ushort operandAddress = 0x0359;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(accumulator);
            }
            
            [Theory]
            [InlineData(0x74, 0xFF, true)]
            [InlineData(0xFF, 0b0100_0000, true)]
            [InlineData(0x01, 0b1011_1111, false)]
            [InlineData(0x0F, 0x00, false)]
            public void OverflowFlagMatchesOperandBitSixState(byte accumulator, byte operand, bool expectFlagRaised)
            {
                byte lowByte = 0xFF;
                byte highByte = 0x26;
                ushort operandAddress = 0x26FF;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(0x74, 0xFF, true)]
            [InlineData(0xFF, 0b1000_0000, true)]
            [InlineData(0x01, 0b0111_1111, false)]
            [InlineData(0x0F, 0x00, false)]
            public void NegativeFlagMatchesOperandBitSevenState(byte accumulator, byte operand, bool expectFlagRaised)
            {
                byte lowByte = 0x00;
                byte highByte = 0x04;
                ushort operandAddress = 0x0400;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void CarryFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Carry;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void InterruptFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.InterruptDisable;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DecimalFlagNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Decimal;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void BreakFlagsNotAffected(StatusFlags initialFlags)
            {
                const StatusFlags flagToTest = StatusFlags.Bit4 | StatusFlags.Bit5;
                bool expectFlagRaised = initialFlags.HasFlag(flagToTest);
                
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(flagToTest)
                    .Should().Be(expectFlagRaised);
            }

            [Fact]
            public void InstructionPointerMovesThreeBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFourCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}