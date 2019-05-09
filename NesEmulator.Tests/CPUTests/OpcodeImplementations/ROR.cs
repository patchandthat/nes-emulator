using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class ROR
    {
        [Trait("Category", "Unit")]
        public class Accumulator
        {
            public Accumulator()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ROR, AddressMode.Accumulator);

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
            [InlineData(0b1010_1010, 0b0101_0101)]
            [InlineData(0b1100_1100, 0b0110_0110)]
            public void ShouldShiftBitsLeft(byte before, byte after)
            {
                var sut = CreateSut();

                sut.LDA(before, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Accumulator.Should().Be(after);
            }

            [Theory]
            [InlineData(StatusFlags.None, 0x0)]
            [InlineData(StatusFlags.Carry, 0x80)]
            public void BitSevenShouldContainCarryFlagsOldState(StatusFlags initialFlags, byte accumulatorResult)
            {
                var sut = CreateSut();

                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Accumulator.Should().Be(accumulatorResult);
            }

            [Theory]
            [InlineData(0b1000_0100, false)]
            [InlineData(0b1100_0100, false)]
            [InlineData(0b1011_0110, false)]
            [InlineData(0b1000_1111, true)]
            [InlineData(0b0111_1111, true)]
            public void CarryFlagShouldContainBit1OldState(byte accumulatorValue, bool carryFlagPresent)
            {
                var sut = CreateSut();

                sut.LDA(accumulatorValue, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().Be(carryFlagPresent);
            }

            [Theory]
            [InlineData(0b0000_0000, false)]
            [InlineData(0b0000_0001, false)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void NegativeFlagShouldSetIfResultBit7IsHigh(byte accumulator, bool negativeSet)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, false)]
            [InlineData(0b0000_0001, false)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void NegativeFlagShouldClearIfResultBit7IsLow(byte accumulator, bool negativeSet)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.Negative);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldSetIfResultIsZero(byte accumulator, bool zeroSet)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldClearSetIfResultIsNotZero(byte accumulator, bool zeroSet)
            {
                var sut = CreateSut();
                sut.LDA(accumulator, _memory);
                sut.ForceStatus(StatusFlags.Zero);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Fact]
            public void CycleCountShouldIncreaseBy2()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerShouldIncreaseBy1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void NegativeFlagShouldSetIfResultBit7IsHigh_II()
            {
                var sut = CreateSut();
                sut.LDA(0x00, _memory);
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }
        }

        [Trait("Category", "Unit")]
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ROR, AddressMode.ZeroPage);

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
            [InlineData(0x16, 0x0016, 0b1010_1010, 0b0101_0101)]
            [InlineData(0xFF, 0x00FF, 0b1100_1100, 0b0110_0110)]
            public void ShouldShiftMemoryBitsRight(byte operand, ushort targetAddress, byte before, byte after)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(targetAddress)).Returns(before);

                sut.Step();

                A.CallTo(() => _memory.Write(targetAddress, after))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x16, 0x0016, StatusFlags.None, 0x0)]
            [InlineData(0xFF, 0x00FF, StatusFlags.Carry, 0x80)]
            public void BitSevenShouldContainCarryFlagsOldState(
                byte operand,
                ushort targetAddress,
                StatusFlags initialFlags,
                byte result)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(targetAddress)).Returns((byte) 0x0);

                sut.Step();

                A.CallTo(() => _memory.Write(targetAddress, result))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x00, 0x0000, 0b1000_0100, false)]
            [InlineData(0xAB, 0x00AB, 0b1100_0100, false)]
            [InlineData(0x36, 0x0036, 0b1011_0110, false)]
            [InlineData(0x67, 0x0067, 0b1000_1111, true)]
            [InlineData(0xFF, 0x00FF, 0b0111_1111, true)]
            public void CarryFlagShouldContainBit0OldState(
                byte operand,
                ushort address,
                byte start,
                bool carryFlagPresent)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(address)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().Be(carryFlagPresent);
            }

            [Theory]
            [InlineData(0b0000_0000, StatusFlags.None, false)]
            [InlineData(0b0000_0001, StatusFlags.None, false)]
            [InlineData(0b0000_0010, StatusFlags.None, false)]
            [InlineData(0b0000_0100, StatusFlags.None, false)]
            [InlineData(0b0000_1000, StatusFlags.None, false)]
            [InlineData(0b0001_0000, StatusFlags.None, false)]
            [InlineData(0b0010_0000, StatusFlags.None, false)]
            [InlineData(0b0100_0000, StatusFlags.None, false)]
            [InlineData(0b1000_0000, StatusFlags.None, false)]
            [InlineData(0b0000_0000, StatusFlags.Carry, true)]
            public void NegativeFlagShouldSetIfResultBit7IsHigh(byte start, StatusFlags initialFlags, bool negativeSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, false)]
            [InlineData(0b0000_0001, false)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void NegativeFlagShouldClearIfResultBit7IsLow(byte start, bool negativeSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(~StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldSetIfResultIsZero(byte start, bool zeroSet)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldClearSetIfResultIsNotZero(byte start, bool zeroSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Zero);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Fact]
            public void CycleCountShouldIncreaseBy5()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerShouldIncreaseBy2()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }

        [Trait("Category", "Unit")]
        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ROR, AddressMode.ZeroPageX);

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
            [InlineData(0x16, 0x10, 0x0026, 0b1010_1010, 0b0101_0101)]
            [InlineData(0xFF, 0x10, 0x000F, 0b1100_1100, 0b0110_0110)]
            public void ShouldShiftMemoryBitsRight(
                byte operand,
                byte xOffset,
                ushort targetAddress,
                byte before,
                byte after)
            {
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(targetAddress)).Returns(before);

                sut.Step();

                A.CallTo(() => _memory.Write(targetAddress, after))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x16, 0x10, 0x0026, StatusFlags.None, 0x0)]
            [InlineData(0xFF, 0x10, 0x000F, StatusFlags.Carry, 0x80)]
            public void BitSevenShouldContainCarryFlagsOldState(
                byte operand,
                byte xOffset,
                ushort targetAddress,
                StatusFlags initialFlags,
                byte result)
            {
                var sut = CreateSut();
                sut.LDX(xOffset, _memory);

                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(targetAddress)).Returns((byte) 0x0);

                sut.Step();

                A.CallTo(() => _memory.Write(targetAddress, result))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x00, 0x0000, 0b1000_0100, false)]
            [InlineData(0xAB, 0x00AB, 0b1100_0100, false)]
            [InlineData(0x36, 0x0036, 0b1011_0110, false)]
            [InlineData(0x67, 0x0067, 0b1000_1111, true)]
            [InlineData(0xFF, 0x00FF, 0b0111_1111, true)]
            public void CarryFlagShouldContainBit0OldState(
                byte operand,
                ushort address,
                byte start,
                bool carryFlagPresent)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                A.CallTo(() => _memory.Read(address)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().Be(carryFlagPresent);
            }

            [Theory]
            [InlineData(0b0000_0000, StatusFlags.None, false)]
            [InlineData(0b0000_0001, StatusFlags.None, false)]
            [InlineData(0b0000_0010, StatusFlags.None, false)]
            [InlineData(0b0000_0100, StatusFlags.None, false)]
            [InlineData(0b0000_1000, StatusFlags.None, false)]
            [InlineData(0b0001_0000, StatusFlags.None, false)]
            [InlineData(0b0010_0000, StatusFlags.None, false)]
            [InlineData(0b0100_0000, StatusFlags.None, false)]
            [InlineData(0b1000_0000, StatusFlags.None, false)]
            [InlineData(0b0000_0000, StatusFlags.Carry, true)]
            public void NegativeFlagShouldSetIfResultBit7IsHigh(byte start, StatusFlags initialFlags, bool negativeSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, false)]
            [InlineData(0b0000_0001, false)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void NegativeFlagShouldClearIfResultBit7IsLow(byte start, bool negativeSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(~StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(negativeSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldSetIfResultIsZero(byte start, bool zeroSet)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Theory]
            [InlineData(0b0000_0000, true)]
            [InlineData(0b0000_0001, true)]
            [InlineData(0b0000_0010, false)]
            [InlineData(0b0000_0100, false)]
            [InlineData(0b0000_1000, false)]
            [InlineData(0b0001_0000, false)]
            [InlineData(0b0010_0000, false)]
            [InlineData(0b0100_0000, false)]
            [InlineData(0b1000_0000, false)]
            public void ZeroFlagShouldClearSetIfResultIsNotZero(byte start, bool zeroSet)
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Zero);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                A.CallTo(() => _memory.Read(0x0000)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(zeroSet);
            }

            [Fact]
            public void CycleCountShouldIncreaseBy6()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerShouldIncreaseBy2()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }

        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ROR, AddressMode.Absolute);

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
            [InlineData(0b0111_1111, true)]
            [InlineData(0b1000_0000, false)]
            public void CarryFlagRaisedWhenBit0WasHigh(byte start, bool carryRaised)
            {
                byte low = 0x58;
                byte high = 0x05;
                ushort addr = 0x0558;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().Be(carryRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None, 0x00)]
            [InlineData(StatusFlags.Carry, 0x80)]
            public void Bit7ContainsOldCarryFlagState(StatusFlags initialFlags, byte expectedResult)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                A.CallTo(() => _memory.Write(addr, expectedResult))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All & ~StatusFlags.Carry)]
            public void ZeroFlagSetIfResultIsZero(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All & ~StatusFlags.Carry)]
            public void ZeroFlagClearedIfResultIsNotZero(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0011_1100;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.Carry)]
            public void NegativeFlagSetIfResultIsNegative(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0111_1100;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All & ~StatusFlags.Carry)]
            public void NegativeFlagClearedIfResultIs0(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b1000_0000;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All & ~StatusFlags.Carry)]
            public void NegativeFlagClearedIfResultIsGreaterThan0(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0011_1100;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Fact]
            public void ExecutionTakes6Cycles()
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIncreasesBy3()
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ShiftsMemoryBitsRight()
            {
                const byte start = 0b0111_1100;
                const byte expectedResult = 0b0011_1110;

                byte low = 0x58;
                byte high = 0x05;
                ushort addr = 0x0558;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                A.CallTo(() => _memory.Write(addr, expectedResult))
                    .MustHaveHappened();
            }
        }

        [Trait("Category", "Unit")]
        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ROR, AddressMode.AbsoluteX);

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
            [InlineData(0b0111_1111, true)]
            [InlineData(0b1000_0000, false)]
            public void CarryFlagRaisedWhenBit0WasHigh(byte start, bool carryRaised)
            {
                byte low = 0x58;
                byte high = 0x05;
                ushort addr = 0x0558;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().Be(carryRaised);
            }

            [Theory]
            [InlineData(StatusFlags.None, 0x00)]
            [InlineData(StatusFlags.Carry, 0x80)]
            public void Bit7ContainsOldCarryFlagState(StatusFlags initialFlags, byte expectedResult)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                A.CallTo(() => _memory.Write(addr, expectedResult))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All & ~StatusFlags.Carry)]
            public void ZeroFlagSetIfResultIsZero(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.Zero | StatusFlags.Overflow | StatusFlags.InterruptDisable | StatusFlags.Negative |
                        StatusFlags.Bit4 | StatusFlags.Bit5 | StatusFlags.Decimal)]
            public void ZeroFlagClearedIfResultIsNotZero(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0011_1100;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.Carry)]
            public void NegativeFlagSetIfResultIsNegative(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0000_0000;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(~StatusFlags.Carry)]
            public void NegativeFlagClearedIfResultIs0(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0000_0001;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.Zero | StatusFlags.Overflow | StatusFlags.InterruptDisable | StatusFlags.Negative |
                        StatusFlags.Bit4 | StatusFlags.Bit5 | StatusFlags.Decimal)]
            public void NegativeFlagClearedIfResultIsGreaterThan0(StatusFlags initialFlags)
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0b0011_1100;

                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Fact]
            public void ExecutionTakes7Cycles()
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                var expectedCycles = sut.ElapsedCycles + 7;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIncreasesBy3()
            {
                byte low = 0x85;
                byte high = 0x50;
                ushort addr = 0x5085;
                byte start = 0x0;

                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ShiftsCorrectBitsRight()
            {
                const byte start = 0b0111_1100;
                const byte expectedResult = 0b0011_1110;

                byte low = 0x58;
                byte high = 0x05;
                byte xOffset = 0x46;
                ushort addr = 0x059E;

                var sut = CreateSut();
                sut.LDX(xOffset, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);

                A.CallTo(() => _memory.Read(addr)).Returns(start);

                sut.Step();

                A.CallTo(() => _memory.Write(addr, expectedResult))
                    .MustHaveHappened();
            }
        }
    }
}