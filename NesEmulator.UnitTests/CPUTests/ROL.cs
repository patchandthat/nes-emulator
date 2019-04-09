using FakeItEasy;
using FakeItEasy.Sdk;
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
                
                [Theory]
                [InlineData(0x16, 0x0016, 0b0101_0101, 0b1010_1010)]
                [InlineData(0xFF, 0x00FF, 0b0110_0110, 0b1100_1100)]
                public void ShouldShiftMemoryBitsLeft(byte operand, ushort targetAddress, byte before, byte after)
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(targetAddress)).Returns(before);

                    sut.Step();
                    
                    A.CallTo(() => _memory.Write(targetAddress, after))
                        .MustHaveHappened();
                }
                
                [Theory]
                [InlineData(0x16, 0x0016, StatusFlags.None, 0x0)]
                [InlineData(0xFF, 0x00FF, StatusFlags.Carry, 0x1)]
                public void BitZeroShouldContainCarryFlagsOldState(byte operand, ushort targetAddress, 
                    StatusFlags initialFlags, byte result)
                {
                    var sut = CreateSut();
                    
                    sut.ForceStatus(initialFlags);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(targetAddress)).Returns((byte)0x0);
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(targetAddress, result))
                        .MustHaveHappened();
                }
                
                [Theory]
                [InlineData(0x00, 0x0000, 0b1000_0100, true)]
                [InlineData(0xAB, 0x00AB, 0b1100_0100, true)]
                [InlineData(0x36, 0x0036, 0b1011_0110, true)]
                [InlineData(0x67, 0x0067, 0b1000_1111, true)]
                [InlineData(0xFF, 0x00FF, 0b0111_1111, false)]
                public void CarryFlagShouldContainBit7OldState(byte operand, ushort address, byte start, bool carryFlagPresent)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(address)).Returns(start);
                    
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
                public void NegativeFlagShouldSetIfResultBit7IsHigh(byte start, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(StatusFlags.None);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

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
                [InlineData(0b0100_0000, true)]
                [InlineData(0b1000_0000, false)]
                public void NegativeFlagShouldClearIfResultBit7IsLow(byte start, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(StatusFlags.All);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
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
                public void ZeroFlagShouldSetIfResultIsZero(byte start, bool zeroSet)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
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
                public void ZeroFlagShouldClearSetIfResultIsNotZero(byte start, bool zeroSet)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().Be(zeroSet);
                }

                [Fact]
                public void InstructionPointerShouldIncreaseBy2()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedPointer = sut.InstructionPointer.Plus(2);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void CycleCountShouldIncreaseBy5()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedCycles = sut.ElapsedCycles + 5;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
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
                
                [Theory]
                [InlineData(0x16, 0x10, 0x0026, StatusFlags.None, 0x0)]
                [InlineData(0xFF, 0x10, 0x000F, StatusFlags.Carry, 0x1)]
                public void BitZeroShouldContainCarryFlagsOldState(byte operand, byte xOffset, ushort targetAddress, 
                    StatusFlags initialFlags, byte result)
                {
                    var sut = CreateSut();
                    sut.LDX(xOffset, _memory);
                    
                    sut.ForceStatus(initialFlags);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(targetAddress)).Returns((byte)0x0);
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(targetAddress, result))
                        .MustHaveHappened();
                }
                
                [Theory]
                [InlineData(0x00, 0x0000, 0b1000_0100, true)]
                [InlineData(0xAB, 0x00AB, 0b1100_0100, true)]
                [InlineData(0x36, 0x0036, 0b1011_0110, true)]
                [InlineData(0x67, 0x0067, 0b1000_1111, true)]
                [InlineData(0xFF, 0x00FF, 0b0111_1111, false)]
                public void CarryFlagShouldContainBit7OldState(byte operand, ushort address, byte start, bool carryFlagPresent)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(address)).Returns(start);
                    
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
                public void NegativeFlagShouldSetIfResultBit7IsHigh(byte start, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(StatusFlags.None);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

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
                [InlineData(0b0100_0000, true)]
                [InlineData(0b1000_0000, false)]
                public void NegativeFlagShouldClearIfResultBit7IsLow(byte start, bool negativeSet)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(StatusFlags.All);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
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
                public void ZeroFlagShouldSetIfResultIsZero(byte start, bool zeroSet)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
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
                public void ZeroFlagShouldClearSetIfResultIsNotZero(byte start, bool zeroSet)
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    A.CallTo(() => _memory.Read(0x0000)).Returns(start);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().Be(zeroSet);
                }

                [Fact]
                public void InstructionPointerShouldIncreaseBy2()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedPointer = sut.InstructionPointer.Plus(2);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void CycleCountShouldIncreaseBy6()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedCycles = sut.ElapsedCycles + 6;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
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