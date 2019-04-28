using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class ADC
    {
        public class Immediate
        {
            public Immediate()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.Immediate);

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

            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte accumulatorStart = 0x55;
                byte operandValue = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operandValue);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte accumulatorStart = 0x55;
                byte operandValue = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operandValue);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }

            [Fact]
            public void ExecutionTakesTwoCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
        }

        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.ZeroPage);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte accumulatorStart = 0x55;
                byte zeroPageAddress = 0x6A;
                byte operandValue = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operandValue);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte zeroPageAddress = 0xFF;
                byte accumulatorStart = 0x55;
                byte operandValue = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
                    .Returns(operandValue);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
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

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xB3;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xAA;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x56;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x3E;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x20;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xDA;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x39;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xFC;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
        }

        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.ZeroPageX);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte zeroPageAddress = 0x7F;
                byte xOffset = 0x13;
                byte operandAddress = 0x92;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte zeroPageAddress = 0x7F;
                byte xOffset = 0x13;
                byte operandAddress = 0x92;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
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

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x27;
                byte xOffset = 0x13;
                byte operandAddress = 0x3A;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x27;
                byte xOffset = 0x13;
                byte operandAddress = 0x3A;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xA6;
                byte xOffset = 0x20;
                byte operandAddress = 0xC6;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0xA6;
                byte xOffset = 0x20;
                byte operandAddress = 0xC6;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x00;
                byte xOffset = 0x20;
                byte operandAddress = 0x20;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x00;
                byte xOffset = 0x20;
                byte operandAddress = 0x20;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x5E;
                byte xOffset = 0x22;
                byte operandAddress = 0x80;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x5E;
                byte xOffset = 0x22;
                byte operandAddress = 0x80;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void ZeroPageWrap()
            {
                byte zeroPageAddress = 0xF6;
                byte xOffset = 0x20;
                byte operandAddress = 0x16;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }
        }
    
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.Absolute);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;
                
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsThreeBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
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

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x47;
                byte highByte = 0x04;
                ushort operandAddress = 0x0447;
                
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
        }

        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.AbsoluteX);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsThreeBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
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

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte xOffset = 0x80;
                ushort operandAddress = 0x0702;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                
                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }

        public class AbsoluteY
        {
            public AbsoluteY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.AbsoluteY);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsThreeBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
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

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x07;
                ushort operandAddress = 0x0689;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
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

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                byte lowByte = 0x82;
                byte highByte = 0x06;
                byte yOffset = 0x80;
                ushort operandAddress = 0x0702;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(highByte);
                
                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }

        public class IndirectX
        {
            public IndirectX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.IndirectX);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;
                
                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }

            [Fact]
            public void ExecutionTakesSixCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x18;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x3C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void ZeroPageWrap()
            {
                byte zeroPageAddress = 0xE8;
                byte xOffset = 0x24;
                ushort lowByteAddress = 0x0C;
                byte lowByte = 0x62;
                byte highByte = 0x04;
                ushort operandAddress = 0x0462;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }
        }

        public class IndirectY
        {
            public IndirectY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.IndirectY);

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
            
            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6D;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulatorStart, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }

            [Fact]
            public void ExecutionTakesFiveCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0x59, 0xAC, 0x05)]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0x49, 0x63, 0xAD)]
            [InlineData(0xFD, 0x01, 0xFF)]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x7F, 0x7F, 0xFE)]
            [InlineData(0x80, 0x80, 0x00)]
            [InlineData(0x49, 0x63, 0xAC)]
            public void OverflowFlagRaisedIfSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x00, 0x01, 0x02)]
            [InlineData(0xFE, 0x00, 0xFF)]
            [InlineData(0xF6 /* -10 */, 0xFF /*-1 */, 0xF6)]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Overflow)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x00)]
            [InlineData(0xD0, 0x30, 0x00)]
            public void ZeroFlagSetIfResultIsZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x01, 0x01)]
            [InlineData(0xD0, 0x22, 0xF3)]
            public void ZeroFlagClearedIfResultIsNotZero(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xFF, 0xFC, 0xFB)]
            [InlineData(0x7F, 0x01, 0x80)]
            public void NegativeFlagRaisedIfResultBit7IsHigh(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xFF, 0x02, 0x02)]
            [InlineData(0x7E, 0x00, 0x7F)]
            public void NegativeFlagClearedIfResultBit7IsLow(byte accumulator, byte operand, byte expectedResult)
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0x34;
                byte highByte = 0x03;
                ushort operandAddress = 0x034B;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedResult);
                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte lowByte = 0xFF;
                byte highByte = 0x03;
                ushort operandAddress = 0x0416;

                byte accumulatorStart = 0x55;
                byte operand = 0x17;
                byte expectedResult = 0x6C;

                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDA(accumulatorStart, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(lowByteAddress))
                    .Returns(lowByte);
                A.CallTo(() => _memory.Read((ushort)(lowByteAddress+1)))
                    .Returns(highByte);
                A.CallTo(() => _memory.Read(operandAddress)).Returns(operand);
                
                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}