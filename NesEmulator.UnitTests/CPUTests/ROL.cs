using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public static class ROL
        {
            public class Accumulator
            {
                private IMemory _memory;
                private OpCode _op;

                public Accumulator()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.ROL, AddressMode.Accumulator);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Theory]
                [InlineData(0b0101_0101, 0b1010_1010)]
                [InlineData(0b0110_0110, 0b1100_1100)]
                public void ShouldShiftBitsLeft(byte before, byte after)
                {
                    var sut = CreateSut();

                    sut.LDA(before, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Accumulator.Should().Be(after);
                }

                [Theory]
                [InlineData(StatusFlags.None, 0x0)]
                [InlineData(StatusFlags.Carry, 0x1)]
                public void BitZeroShouldContainCarryFlagsOldState(StatusFlags initialFlags, byte accumulatorResult)
                {
                    var sut = CreateSut();
                    
                    sut.ForceStatus(initialFlags);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Accumulator.Should().Be(accumulatorResult);
                }

                [Theory]
                [InlineData(0b1000_0100, true)]
                [InlineData(0b1100_0100, true)]
                [InlineData(0b1011_0110, true)]
                [InlineData(0b1000_1111, true)]
                [InlineData(0b0111_1111, false)]
                public void CarryFlagShouldContainBit7OldState(byte accumulatorValue, bool carryFlagPresent)
                {
                    var sut = CreateSut();

                    sut.LDA(accumulatorValue, _memory);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
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
                [InlineData(0b0100_0000, true)]
                [InlineData(0b1000_0000, false)]
                public void NegativeFlagShouldSetIfResultBit7IsHigh(byte accumulator, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.LDA(accumulator, _memory);
                    sut.ForceStatus(StatusFlags.None);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
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
                [InlineData(0b0100_0000, true)]
                [InlineData(0b1000_0000, false)]
                public void NegativeFlagShouldClearIfResultBit7IsLow(byte accumulator, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.LDA(accumulator, _memory);
                    sut.ForceStatus(StatusFlags.Negative);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().Be(negativeSet);
                }

                [Theory]
                [InlineData(0b0000_0000, true)]
                [InlineData(0b0000_0001, false)]
                [InlineData(0b0000_0010, false)]
                [InlineData(0b0000_0100, false)]
                [InlineData(0b0000_1000, false)]
                [InlineData(0b0001_0000, false)]
                [InlineData(0b0010_0000, false)]
                [InlineData(0b0100_0000, false)]
                [InlineData(0b1000_0000, true)]
                public void ZeroFlagShouldSetIfResultIsZero(byte accumulator, bool zeroSet)
                {
                    var sut = CreateSut();
                    sut.LDA(accumulator, _memory);
                    sut.ForceStatus(StatusFlags.None);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().Be(zeroSet);
                }
                
                [Theory]
                [InlineData(0b0000_0000, true)]
                [InlineData(0b0000_0001, false)]
                [InlineData(0b0000_0010, false)]
                [InlineData(0b0000_0100, false)]
                [InlineData(0b0000_1000, false)]
                [InlineData(0b0001_0000, false)]
                [InlineData(0b0010_0000, false)]
                [InlineData(0b0100_0000, false)]
                [InlineData(0b1000_0000, true)]
                public void ZeroFlagShouldClearSetIfResultIsNotZero(byte accumulator, bool zeroSet)
                {
                    var sut = CreateSut();
                    sut.LDA(accumulator, _memory);
                    sut.ForceStatus(StatusFlags.Zero);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().Be(zeroSet);
                }

                [Fact]
                public void InstructionPointerShouldIncreaseBy1()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedPointer = sut.InstructionPointer.Plus(1);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void CycleCountShouldIncreaseBy2()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedCycles = sut.ElapsedCycles + 2;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }
            }

            public class ZeroPage
            {
                private IMemory _memory;
                private OpCode _op;

                public ZeroPage()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.ROL, AddressMode.ZeroPage);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Fact]
                public void Todo()
                {
                    Assert.True(false, "Todo: ");
                }
            }

            public class ZeroPageX
            {
                private IMemory _memory;
                private OpCode _op;

                public ZeroPageX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.ROL, AddressMode.ZeroPageX);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Fact]
                public void Todo()
                {
                    Assert.True(false, "Todo: ");
                }
            }

            public class Absolute
            {
                private IMemory _memory;
                private OpCode _op;

                public Absolute()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.ROL, AddressMode.Absolute);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Fact]
                public void Todo()
                {
                    Assert.True(false, "Todo: ");
                }
            }

            public class AbsoluteX
            {
                private IMemory _memory;
                private OpCode _op;

                public AbsoluteX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.ROL, AddressMode.AbsoluteX);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Fact]
                public void Todo()
                {
                    Assert.True(false, "Todo: ");
                }
            }
        }
    }
}