using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class PLA
    {
        public class Implicit
        {
            public Implicit()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.PLA, AddressMode.Implicit);

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
            [InlineData(StatusFlags.None, 0x00, true)]
            [InlineData(StatusFlags.All, 0x00, true)]
            [InlineData(StatusFlags.None, 0x01, false)]
            [InlineData(StatusFlags.All, 0xFF, false)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags, byte value, bool zeroRaised)
            {
                var sut = CreateSut();
                sut.ForceStack(0x0176);

                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.StackPointer))
                    .Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero).Should().Be(zeroRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None, 0xFF, true)]
            [InlineData(StatusFlags.All, 0x80, true)]
            [InlineData(StatusFlags.None, 0x00, false)]
            [InlineData(StatusFlags.All, 0x01, false)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags, byte value, bool negativeRaised)
            {
                var sut = CreateSut();
                sut.ForceStack(0x0134);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.StackPointer))
                    .Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative).Should().Be(negativeRaised);
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerMoves1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedIp = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIp);
            }

            [Fact]
            public void PullsAccumulatorValueFromStack()
            {
                var sut = CreateSut();
                sut.ForceStack(0x0110);

                byte value = 0x9F;

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.StackPointer))
                    .Returns(value);

                sut.Step();

                sut.Accumulator.Should().Be(value);
            }

            [Fact]
            public void StackPointerIncreases()
            {
                var sut = CreateSut();
                ushort startingStackPointer = 0x0145;
                var expectedStackPointer = startingStackPointer.Plus(1);
                sut.ForceStack(startingStackPointer);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(expectedStackPointer);
            }

            [Fact]
            // ReSharper disable once InconsistentNaming
            public void StackPointerWrapsAt0x0200()
            {
                var sut = CreateSut();
                sut.ForceStack(0x01FF);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(0x0100);
            }
        }
    }
}