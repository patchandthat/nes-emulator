using FakeItEasy;
using FluentAssertions;
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            [ClassData(typeof(AllByteValues))]
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
            public void Absolute()
            {
                Assert.True(false, "Todo");
            }

            [Fact]
            public void AbsoluteX()
            {
                Assert.True(false, "Todo");
            }

            [Fact]
            public void AbsoluteY()
            {
                Assert.True(false, "Todo");
            }

            [Fact]
            public void IndirectX()
            {
                Assert.True(false, "Todo");
            }

            [Fact]
            public void IndirectY()
            {
                Assert.True(false, "Todo");
            }
        }
    }
}
