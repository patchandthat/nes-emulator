using FakeItEasy;
using FluentAssertions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public class LDA
        {
            private IMemory _memory;

            public LDA()
            {
                _memory = A.Fake<IMemory>();

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte)0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte)0x80);
            }

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldFetchTwoBytes()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFF);

                var sut = CreateSut();
                
                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._)).MustHaveHappenedTwiceExactly();
                A.CallTo(() => _memory.Read(0x8000)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x8001)).MustHaveHappened();
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ImmediateMode_OnExecute_ShouldLoadSecondByteToAccumulator(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Accumulator.Should().Be(value);
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldElapseTwoCycles()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var cyclesBefore = sut.ElapsedCycles;

                sut.Step();

                op.Cycles.Should().Be(2);
                sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ImmediateMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(value == 0x00);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ImmediateMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(value >= 0b1000_0000);
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldIncreaseInstructionPointer()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var ipStart = sut.InstructionPointer;

                sut.Step();

                var ipDiff = sut.InstructionPointer - ipStart;

                ipDiff.Should().Be(op.Bytes);
            }

            [Fact]
            public void ZeroPageMode_OnExecute_ShouldFetchThreeBytes()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._)).MustHaveHappened(3, Times.Exactly);
                A.CallTo(() => _memory.Read(0x8000)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x8001)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x00FF)).MustHaveHappened();
            }

            [Theory]
            [InlineData(0x03, 0xA4)]
            [InlineData(0x2D, 0x01)]
            [InlineData(0x71, 0xC5)]
            [InlineData(0x9E, 0x3A)]
            public void ZeroPageMode_OnExecute_ShouldLoadZeroPageByteToAccumulator(byte operand, byte addressValue)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)operand);
                A.CallTo(() => _memory.Read(operand)).Returns(addressValue);

                var sut = CreateSut();

                sut.Step();

                sut.Accumulator.Should().Be(addressValue);
            }

            [Fact]
            public void ZeroPageMode_OnExecute_ShouldElapseThreeCycles()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var cyclesBefore = sut.ElapsedCycles;

                sut.Step();

                op.Cycles.Should().Be(3);
                sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ZeroPageMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x00FC)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(value == 0x00);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ZeroPageMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x00FC)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(value >= 0b1000_0000);
            }

            [Fact]
            public void ZeroPageMode_OnExecute_ShouldIncreaseInstructionPointer()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var ipStart = sut.InstructionPointer;

                sut.Step();

                var ipDiff = sut.InstructionPointer - ipStart;

                ipDiff.Should().Be(op.Bytes);
            }

            [Fact]
            public void ZeroPageXMode_OnExecute_ShouldFetchThreeBytes()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._)).MustHaveHappened(3, Times.Exactly);
                A.CallTo(() => _memory.Read(0x8000)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x8001)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x00FF)).MustHaveHappened();
            }

            [Theory]
            [InlineData(0x03, 0xA4)]
            [InlineData(0x2D, 0x01)]
            [InlineData(0x71, 0xC5)]
            [InlineData(0x9E, 0x3A)]
            public void ZeroPageXMode_OnExecute_ShouldLoadZeroPageByteToAccumulator(byte operand, byte addressValue)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)operand);
                A.CallTo(() => _memory.Read(operand)).Returns(addressValue);

                var sut = CreateSut();

                sut.Step();

                sut.Accumulator.Should().Be(addressValue);
            }

            [Theory]
            [InlineData(0x03, 0xA4, 0xA7)]
            [InlineData(0x2D, 0x01, 0x2E)]
            [InlineData(0x71, 0xC5, 0x36)]
            [InlineData(0xEE, 0x3A, 0x28)]
            public void ZeroPageXMode_WhenResultExceeds255_ShouldLoopBackToZeroPage(byte operand, byte xRegister, byte expectedAddress)
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)xRegister);

                OpCode lda = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(lda.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)operand);

                var sut = CreateSut();

                sut.Step(); // ldx
                sut.Step(); // lda

                A.CallTo(() => _memory.Read(expectedAddress))
                    .MustHaveHappened();
            }

            [Fact]
            public void ZeroPageXMode_OnExecute_ShouldElapseThreeCycles()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var cyclesBefore = sut.ElapsedCycles;

                sut.Step();

                op.Cycles.Should().Be(4);
                sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ZeroPageXMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x00FC)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(value == 0x00);
            }

            [Theory]
            [ClassData(typeof(ManyByteValues))]
            public void ZeroPageXMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x00FC)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(value >= 0b1000_0000);
            }

            [Fact]
            public void ZeroPageXMode_OnExecute_ShouldIncreaseInstructionPointer()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var ipStart = sut.InstructionPointer;

                sut.Step();

                var ipDiff = sut.InstructionPointer - ipStart;

                ipDiff.Should().Be(op.Bytes);
            }

            [Fact]
            public void Absolute_OnExecute_ShouldFetch4Bytes()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x00);

                var sut = CreateSut();

                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._))
                    .MustHaveHappened(4, Times.Exactly);
            }

            [Fact]
            public void Absolute_OnExecute_ShouldSetCorrectValueToAccumulator()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x03);

                byte expectedValue = 0xFF;
                A.CallTo(() => _memory.Read(0x03FC)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void Absolute_OnExecute_ShouldElapse4Cycles()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x03);

                var sut = CreateSut();

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0x00, true)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, false)]
            [InlineData(0xFF, false)]
            public void Absolute_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroFlagSet);
            }

            [Theory]
            [InlineData(0x00, false)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, true)]
            [InlineData(0xFF, true)]
            public void Absolute_OnExecute_ShouldSetNegativeFlagAsAppropriate(byte registerValue, bool negativeFlagSet)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeFlagSet);
            }

            [Fact]
            public void Absolute_OnExecute_ShouldIncreaseInstructionPointerBy3()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8002)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns((byte)0xFF);

                var sut = CreateSut();

                var expectedIP = (ushort)(sut.InstructionPointer + 3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIP);
            }

            [Fact]
            public void AbsoluteX_OnExecute_ShouldFetch4Bytes()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode lda = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(lda.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x00);

                var sut = CreateSut();

                sut.Step();
                Fake.ClearRecordedCalls(_memory);
                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._))
                    .MustHaveHappened(4, Times.Exactly);
            }

            [Fact]
            public void AbsoluteX_OnExecute_ShouldSetCorrectValueToAccumulator()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x02);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                byte expectedValue = 0xFF;
                A.CallTo(() => _memory.Read(0x03FE)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void AbsoluteX_OnExecuteAndNotCrossingPageBoundary_ShouldElapse4Cycles()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFF);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                var sut = CreateSut();
                sut.Step(); // ldx

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void AbsoluteX_OnExecuteAndDoesCrossPageBoundary_ShouldElapse5Cycles()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFF);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                var sut = CreateSut();

                sut.Step(); //ldx

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0x00, true)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, false)]
            [InlineData(0xFF, false)]
            public void AbsoluteX_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroFlagSet);
            }

            [Theory]
            [InlineData(0x00, false)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, true)]
            [InlineData(0xFF, true)]
            public void AbsoluteX_OnExecute_ShouldSetNegativeFlagAsAppropriate(byte registerValue, bool negativeFlagSet)
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeFlagSet);
            }

            [Fact]
            public void AbsoluteX_OnExecute_ShouldIncreaseInstructionPointerBy3()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step(); // ldx

                var expectedIP = (ushort)(sut.InstructionPointer + 3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIP);
            }

            [Fact]
            public void AbsoluteY_OnExecute_ShouldFetch4Bytes()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode lda = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(lda.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x00);

                var sut = CreateSut();

                sut.Step();
                Fake.ClearRecordedCalls(_memory);
                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._))
                    .MustHaveHappened(4, Times.Exactly);
            }

            [Fact]
            public void AbsoluteY_OnExecute_ShouldSetCorrectValueToAccumulator()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x02);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                byte expectedValue = 0xFF;
                A.CallTo(() => _memory.Read(0x03FE)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void AbsoluteY_OnExecuteAndNotCrossingPageBoundary_ShouldElapse4Cycles()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFF);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void AbsoluteY_OnExecuteAndDoesCrossPageBoundary_ShouldElapse5Cycles()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFF);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x03);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0x00, true)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, false)]
            [InlineData(0xFF, false)]
            public void AbsoluteY_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroFlagSet);
            }

            [Theory]
            [InlineData(0x00, false)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, true)]
            [InlineData(0xFF, true)]
            public void AbsoluteY_OnExecute_ShouldSetNegativeFlagAsAppropriate(byte registerValue, bool negativeFlagSet)
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeFlagSet);
            }

            [Fact]
            public void AbsoluteY_OnExecute_ShouldIncreaseInstructionPointerBy3()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);
                A.CallTo(() => _memory.Read(0x8004)).Returns((byte)0x01);
                A.CallTo(() => _memory.Read(0x0100)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedIP = (ushort)(sut.InstructionPointer + 3);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIP);
            }

            [Fact]
            public void IndirectX_OnExecute_ShouldFetch5Bytes()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode lda = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(lda.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x10);

                var sut = CreateSut();

                sut.Step();
                Fake.ClearRecordedCalls(_memory);
                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._))
                    .MustHaveHappened(5, Times.Exactly);

                A.CallTo(() => _memory.Read(0x0010)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x0011)).MustHaveHappened();
            }

            [Fact]
            public void IndirectX_OnExecute_ShouldSetCorrectValueToAccumulator()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x05);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0025)).Returns((byte)0xFE);
                A.CallTo(() => _memory.Read(0x0026)).Returns((byte)0x03);

                byte expectedValue = 0xAC;
                A.CallTo(() => _memory.Read(0x03FE)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void IndirectX_OnExecute_ShouldWrapToZeroPageIfOffsetExceeds0xFF()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x10);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xF5);

                // Expect wrap back to ZP
                A.CallTo(() => _memory.Read(0x0005)).Returns((byte)0xFE);
                A.CallTo(() => _memory.Read(0x0006)).Returns((byte)0x03);

                byte expectedValue = 0xAC;
                A.CallTo(() => _memory.Read(0x03FE)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void IndirectX_OnExecute_ShouldElapse6Cycles()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step(); // ldx

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0x00, true)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, false)]
            [InlineData(0xFF, false)]
            public void IndirectX_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0xEE);
                A.CallTo(() => _memory.Read(0x0022)).Returns((byte)0xCC);

                A.CallTo(() => _memory.Read(0xCCEE)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroFlagSet);
            }

            [Theory]
            [InlineData(0x00, false)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, true)]
            [InlineData(0xFF, true)]
            public void IndirectX_OnExecute_ShouldSetNegativeFlagAsAppropriate(byte registerValue, bool negativeFlagSet)
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0xEE);
                A.CallTo(() => _memory.Read(0x0022)).Returns((byte)0xCC);

                A.CallTo(() => _memory.Read(0xCCEE)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeFlagSet);
            }

            [Fact]
            public void IndirectX_OnExecute_ShouldIncreaseInstructionPointerBy2()
            {
                OpCode ldx = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldx.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectX);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);

                var sut = CreateSut();

                sut.Step(); // ldx

                var expectedIP = (ushort)(sut.InstructionPointer + 2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIP);
            }

            //

            [Fact]
            public void IndirectY_OnExecute_ShouldFetch5Bytes()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode lda = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(lda.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x10);

                var sut = CreateSut();

                sut.Step();
                Fake.ClearRecordedCalls(_memory);
                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._))
                    .MustHaveHappened(5, Times.Exactly);

                A.CallTo(() => _memory.Read(0x0010)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x0011)).MustHaveHappened();
            }

            [Fact]
            public void IndirectY_OnExecute_ShouldSetCorrectValueToAccumulator()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x05);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0020)).Returns((byte)0xFE);
                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0x03);

                byte expectedValue = 0xAC;
                A.CallTo(() => _memory.Read(0x03FE + 0x05)).Returns(expectedValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Accumulator.Should().Be(expectedValue);
            }

            [Fact]
            public void IndirectY_OnExecuteWithoutCrossingPageWhileIndexing_ShouldElapse5Cycles()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0xFE);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IndirectY_OnExecuteAndTargetAddressIsNotZeroPage_ShouldElapse6Cycles()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0020)).Returns((byte)0xFF);
                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0x03);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(0x00, true)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, false)]
            [InlineData(0xFF, false)]
            public void IndirectY_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0020)).Returns((byte)0xEE);
                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0xCC);

                A.CallTo(() => _memory.Read(0xCCEF)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroFlagSet);
            }

            [Theory]
            [InlineData(0x00, false)]
            [InlineData(0x7F, false)]
            [InlineData(0x80, true)]
            [InlineData(0xFF, true)]
            public void IndirectY_OnExecute_ShouldSetNegativeFlagAsAppropriate(byte registerValue, bool negativeFlagSet)
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x01);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x20);

                A.CallTo(() => _memory.Read(0x0020)).Returns((byte)0xEE);
                A.CallTo(() => _memory.Read(0x0021)).Returns((byte)0xCC);

                A.CallTo(() => _memory.Read(0xCCEF)).Returns(registerValue);

                var sut = CreateSut();

                sut.Step();
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeFlagSet);
            }

            [Fact]
            public void IndirectY_OnExecute_ShouldIncreaseInstructionPointerBy2()
            {
                OpCode ldy = new OpcodeDefinitions().FindOpcode(Operation.LDY, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(ldy.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDA, AddressMode.IndirectY);
                A.CallTo(() => _memory.Read(0x8002)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8003)).Returns((byte)0x00);

                var sut = CreateSut();

                sut.Step(); // ldy

                var expectedIP = (ushort)(sut.InstructionPointer + 2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIP);
            }
        }
    }
}
