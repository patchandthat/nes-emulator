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
        }
    }
}
