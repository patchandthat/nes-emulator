using FakeItEasy;
using FluentAssertions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    [Trait("Category", "Unit")]
    public class LDA
    {
        public LDA()
        {
            _memoryBus = A.Fake<IMemoryBus>();

            A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                .Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                .Returns((byte) 0x80);
        }

        private readonly IMemoryBus _memoryBus;

        private CPU CreateSut()
        {
            var cpu = new CPU(_memoryBus);
            cpu.Power();
            cpu.Step(); // Execute reset interrupt
            Fake.ClearRecordedCalls(_memoryBus);
            return cpu;
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ImmediateMode_OnExecute_ShouldLoadSecondByteToAccumulator(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Accumulator.Should().Be(value);
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ImmediateMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Zero)
                .Should().Be(value == 0x00);
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ImmediateMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(value >= 0b1000_0000);
        }

        [Theory]
        [InlineData(0x03, 0xA4)]
        [InlineData(0x2D, 0x01)]
        [InlineData(0x71, 0xC5)]
        [InlineData(0x9E, 0x3A)]
        public void ZeroPageMode_OnExecute_ShouldLoadZeroPageByteToAccumulator(byte operand, byte addressValue)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(operand);
            A.CallTo(() => _memoryBus.Read(operand)).Returns(addressValue);

            var sut = CreateSut();

            sut.Step();

            sut.Accumulator.Should().Be(addressValue);
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ZeroPageMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x00FC)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Zero)
                .Should().Be(value == 0x00);
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ZeroPageMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x00FC)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(value >= 0b1000_0000);
        }

        [Theory]
        [InlineData(0x03, 0xA4)]
        [InlineData(0x2D, 0x01)]
        [InlineData(0x71, 0xC5)]
        [InlineData(0x9E, 0x3A)]
        public void ZeroPageXMode_OnExecute_ShouldLoadZeroPageByteToAccumulator(byte operand, byte addressValue)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(operand);
            A.CallTo(() => _memoryBus.Read(operand)).Returns(addressValue);

            var sut = CreateSut();

            sut.Step();

            sut.Accumulator.Should().Be(addressValue);
        }

        [Theory]
        [InlineData(0x03, 0xA4, 0xA7)]
        [InlineData(0x2D, 0x01, 0x2E)]
        [InlineData(0x71, 0xC5, 0x36)]
        [InlineData(0xEE, 0x3A, 0x28)]
        public void ZeroPageXMode_WhenResultExceeds255_ShouldLoopBackToZeroPage(
            byte operand,
            byte xRegister,
            byte expectedAddress)
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns(xRegister);

            var lda = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(lda.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns(operand);

            var sut = CreateSut();

            sut.Step(); // ldx
            sut.Step(); // lda

            A.CallTo(() => _memoryBus.Read(expectedAddress))
                .MustHaveHappened();
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ZeroPageXMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x00FC)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Zero)
                .Should().Be(value == 0x00);
        }

        [Theory]
        [ClassData(typeof(ManyByteValues))]
        public void ZeroPageXMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x00FC)).Returns(value);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(value >= 0b1000_0000);
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x7F, false)]
        [InlineData(0x80, false)]
        [InlineData(0xFF, false)]
        public void Absolute_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

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
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

            var sut = CreateSut();

            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(negativeFlagSet);
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x7F, false)]
        [InlineData(0x80, false)]
        [InlineData(0xFF, false)]
        public void AbsoluteX_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

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
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(negativeFlagSet);
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x7F, false)]
        [InlineData(0x80, false)]
        [InlineData(0xFF, false)]
        public void AbsoluteY_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

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
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns(registerValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(negativeFlagSet);
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x7F, false)]
        [InlineData(0x80, false)]
        [InlineData(0xFF, false)]
        public void IndirectX_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0xEE);
            A.CallTo(() => _memoryBus.Read(0x0022)).Returns((byte) 0xCC);

            A.CallTo(() => _memoryBus.Read(0xCCEE)).Returns(registerValue);

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
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0xEE);
            A.CallTo(() => _memoryBus.Read(0x0022)).Returns((byte) 0xCC);

            A.CallTo(() => _memoryBus.Read(0xCCEE)).Returns(registerValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(negativeFlagSet);
        }

        [Theory]
        [InlineData(0x00, true)]
        [InlineData(0x7F, false)]
        [InlineData(0x80, false)]
        [InlineData(0xFF, false)]
        public void IndirectY_OnExecute_ShouldSetZeroFlagAsAppropriate(byte registerValue, bool zeroFlagSet)
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0020)).Returns((byte) 0xEE);
            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0xCC);

            A.CallTo(() => _memoryBus.Read(0xCCEF)).Returns(registerValue);

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
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0020)).Returns((byte) 0xEE);
            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0xCC);

            A.CallTo(() => _memoryBus.Read(0xCCEF)).Returns(registerValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Status.HasFlag(StatusFlags.Negative)
                .Should().Be(negativeFlagSet);
        }

        [Fact]
        public void Absolute_OnExecute_ShouldElapse4Cycles()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x03);

            var sut = CreateSut();

            var expectedCycles = sut.ElapsedCycles + 4;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void Absolute_OnExecute_ShouldFetch4Bytes()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x00);

            var sut = CreateSut();

            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._))
                .MustHaveHappened(4, Times.Exactly);
        }

        [Fact]
        public void Absolute_OnExecute_ShouldIncreaseInstructionPointerBy3()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns((byte) 0xFF);

            var sut = CreateSut();

            var expectedIp = (ushort) (sut.InstructionPointer + 3);

            sut.Step();

            sut.InstructionPointer.Should().Be(expectedIp);
        }

        [Fact]
        public void Absolute_OnExecute_ShouldSetCorrectValueToAccumulator()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Absolute);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns((byte) 0x03);

            byte expectedValue = 0xFF;
            A.CallTo(() => _memoryBus.Read(0x03FC)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        [Fact]
        public void AbsoluteX_OnExecute_ShouldFetch4Bytes()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var lda = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(lda.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x00);

            var sut = CreateSut();

            sut.Step();
            Fake.ClearRecordedCalls(_memoryBus);
            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._))
                .MustHaveHappened(4, Times.Exactly);
        }

        [Fact]
        public void AbsoluteX_OnExecute_ShouldIncreaseInstructionPointerBy3()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step(); // ldx

            var expectedIp = (ushort) (sut.InstructionPointer + 3);

            sut.Step();

            sut.InstructionPointer.Should().Be(expectedIp);
        }

        [Fact]
        public void AbsoluteX_OnExecute_ShouldSetCorrectValueToAccumulator()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x02);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            byte expectedValue = 0xFF;
            A.CallTo(() => _memoryBus.Read(0x03FE)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        [Fact]
        public void AbsoluteX_OnExecuteAndDoesCrossPageBoundary_ShouldElapse5Cycles()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFF);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            var sut = CreateSut();

            sut.Step(); //ldx

            var expectedCycles = sut.ElapsedCycles + 5;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void AbsoluteX_OnExecuteAndNotCrossingPageBoundary_ShouldElapse4Cycles()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFF);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            var sut = CreateSut();
            sut.Step(); // ldx

            var expectedCycles = sut.ElapsedCycles + 4;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void AbsoluteY_OnExecute_ShouldFetch4Bytes()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var lda = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(lda.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x00);

            var sut = CreateSut();

            sut.Step();
            Fake.ClearRecordedCalls(_memoryBus);
            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._))
                .MustHaveHappened(4, Times.Exactly);
        }

        [Fact]
        public void AbsoluteY_OnExecute_ShouldIncreaseInstructionPointerBy3()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x01);
            A.CallTo(() => _memoryBus.Read(0x0100)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedIp = (ushort) (sut.InstructionPointer + 3);

            sut.Step();

            sut.InstructionPointer.Should().Be(expectedIp);
        }

        [Fact]
        public void AbsoluteY_OnExecute_ShouldSetCorrectValueToAccumulator()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x02);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            byte expectedValue = 0xFF;
            A.CallTo(() => _memoryBus.Read(0x03FE)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        [Fact]
        public void AbsoluteY_OnExecuteAndDoesCrossPageBoundary_ShouldElapse5Cycles()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFF);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedCycles = sut.ElapsedCycles + 5;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void AbsoluteY_OnExecuteAndNotCrossingPageBoundary_ShouldElapse4Cycles()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.AbsoluteY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFF);
            A.CallTo(() => _memoryBus.Read(0x8004)).Returns((byte) 0x03);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedCycles = sut.ElapsedCycles + 4;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void ImmediateMode_OnExecute_ShouldElapseTwoCycles()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var cyclesBefore = sut.ElapsedCycles;

            sut.Step();

            op.Cycles.Should().Be(2);
            sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
        }

        [Fact]
        public void ImmediateMode_OnExecute_ShouldFetchTwoBytes()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._)).MustHaveHappenedTwiceExactly();
            A.CallTo(() => _memoryBus.Read(0x8000)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x8001)).MustHaveHappened();
        }

        [Fact]
        public void ImmediateMode_OnExecute_ShouldIncreaseInstructionPointer()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var ipStart = sut.InstructionPointer;

            sut.Step();

            var ipDiff = sut.InstructionPointer - ipStart;

            ipDiff.Should().Be(op.Bytes);
        }

        [Fact]
        public void IndirectX_OnExecute_ShouldElapse6Cycles()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step(); // ldx

            var expectedCycles = sut.ElapsedCycles + 6;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void IndirectX_OnExecute_ShouldFetch5Bytes()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var lda = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(lda.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x10);

            var sut = CreateSut();

            sut.Step();
            Fake.ClearRecordedCalls(_memoryBus);
            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._))
                .MustHaveHappened(5, Times.Exactly);

            A.CallTo(() => _memoryBus.Read(0x0010)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x0011)).MustHaveHappened();
        }

        [Fact]
        public void IndirectX_OnExecute_ShouldIncreaseInstructionPointerBy2()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);

            var sut = CreateSut();

            sut.Step(); // ldx

            var expectedIp = (ushort) (sut.InstructionPointer + 2);

            sut.Step();

            sut.InstructionPointer.Should().Be(expectedIp);
        }

        [Fact]
        public void IndirectX_OnExecute_ShouldSetCorrectValueToAccumulator()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x05);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0025)).Returns((byte) 0xFE);
            A.CallTo(() => _memoryBus.Read(0x0026)).Returns((byte) 0x03);

            byte expectedValue = 0xAC;
            A.CallTo(() => _memoryBus.Read(0x03FE)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        [Fact]
        public void IndirectX_OnExecute_ShouldWrapToZeroPageIfOffsetExceeds0xFF()
        {
            var ldx = new OpCodes().FindOpcode(Operation.LDX, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldx.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x10);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectX);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xF5);

            // Expect wrap back to ZP
            A.CallTo(() => _memoryBus.Read(0x0005)).Returns((byte) 0xFE);
            A.CallTo(() => _memoryBus.Read(0x0006)).Returns((byte) 0x03);

            byte expectedValue = 0xAC;
            A.CallTo(() => _memoryBus.Read(0x03FE)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        //

        [Fact]
        public void IndirectY_OnExecute_ShouldFetch5Bytes()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var lda = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(lda.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x10);

            var sut = CreateSut();

            sut.Step();
            Fake.ClearRecordedCalls(_memoryBus);
            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._))
                .MustHaveHappened(5, Times.Exactly);

            A.CallTo(() => _memoryBus.Read(0x0010)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x0011)).MustHaveHappened();
        }

        [Fact]
        public void IndirectY_OnExecute_ShouldIncreaseInstructionPointerBy2()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x00);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedIp = (ushort) (sut.InstructionPointer + 2);

            sut.Step();

            sut.InstructionPointer.Should().Be(expectedIp);
        }

        [Fact]
        public void IndirectY_OnExecute_ShouldSetCorrectValueToAccumulator()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x05);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0020)).Returns((byte) 0xFE);
            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0x03);

            byte expectedValue = 0xAC;
            A.CallTo(() => _memoryBus.Read(0x03FE + 0x05)).Returns(expectedValue);

            var sut = CreateSut();

            sut.Step();
            sut.Step();

            sut.Accumulator.Should().Be(expectedValue);
        }

        [Fact]
        public void IndirectY_WhenOffsetToIndirectAddressCausesPageCross_ShouldElapse6Cycles()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x01);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0x20);

            A.CallTo(() => _memoryBus.Read(0x0020)).Returns((byte) 0xFF);
            A.CallTo(() => _memoryBus.Read(0x0021)).Returns((byte) 0x03);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedCycles = sut.ElapsedCycles + 6;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void IndirectY_OnExecuteWithoutCrossingPageWhileIndexing_ShouldElapse5Cycles()
        {
            var ldy = new OpCodes().FindOpcode(Operation.LDY, AddressMode.Immediate);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(ldy.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.IndirectY);
            A.CallTo(() => _memoryBus.Read(0x8002)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8003)).Returns((byte) 0xFE);

            var sut = CreateSut();

            sut.Step(); // ldy

            var expectedCycles = sut.ElapsedCycles + 5;

            sut.Step();

            sut.ElapsedCycles.Should().Be(expectedCycles);
        }

        [Fact]
        public void ZeroPageMode_OnExecute_ShouldElapseThreeCycles()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var cyclesBefore = sut.ElapsedCycles;

            sut.Step();

            op.Cycles.Should().Be(3);
            sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
        }

        [Fact]
        public void ZeroPageMode_OnExecute_ShouldFetchThreeBytes()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._)).MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => _memoryBus.Read(0x8000)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x8001)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x00FF)).MustHaveHappened();
        }

        [Fact]
        public void ZeroPageMode_OnExecute_ShouldIncreaseInstructionPointer()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPage);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var ipStart = sut.InstructionPointer;

            sut.Step();

            var ipDiff = sut.InstructionPointer - ipStart;

            ipDiff.Should().Be(op.Bytes);
        }

        [Fact]
        public void ZeroPageXMode_OnExecute_ShouldElapseThreeCycles()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var cyclesBefore = sut.ElapsedCycles;

            sut.Step();

            op.Cycles.Should().Be(4);
            sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
        }

        [Fact]
        public void ZeroPageXMode_OnExecute_ShouldFetchThreeBytes()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0xFF);

            var sut = CreateSut();

            sut.Step();

            A.CallTo(() => _memoryBus.Read(A<ushort>._)).MustHaveHappened(3, Times.Exactly);
            A.CallTo(() => _memoryBus.Read(0x8000)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x8001)).MustHaveHappened();
            A.CallTo(() => _memoryBus.Read(0x00FF)).MustHaveHappened();
        }

        [Fact]
        public void ZeroPageXMode_OnExecute_ShouldIncreaseInstructionPointer()
        {
            var op = new OpCodes().FindOpcode(Operation.LDA, AddressMode.ZeroPageX);
            A.CallTo(() => _memoryBus.Read(0x8000)).Returns(op.Value);
            A.CallTo(() => _memoryBus.Read(0x8001)).Returns((byte) 0x00);

            var sut = CreateSut();

            var ipStart = sut.InstructionPointer;

            sut.Step();

            var ipDiff = sut.InstructionPointer - ipStart;

            ipDiff.Should().Be(op.Bytes);
        }
    }
}