using System.Reflection.Emit;
using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;
using OpCode = NesEmulator.Processor.OpCode;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public class INC
        {
            public class ZeroPage
            {
                private IMemory _memory;
                private OpCode _op;

                public ZeroPage()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.INC, AddressMode.ZeroPage);

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
                [InlineData(0x00, 0x00, 0x01)]
                [InlineData(0x30, 0x63, 0x64)]
                [InlineData(0xC7, 0xFE, 0xFF)]
                [InlineData(0xC7, 0xFF, 0x00)]
                public void IncrementsCorrectAddress(byte zeroPageAddr, byte stored, byte expectedWrite)
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(zeroPageAddr))
                        .Returns(stored);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(zeroPageAddr);
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(zeroPageAddr, expectedWrite))
                        .MustHaveHappened();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0xFF;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().BeTrue();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x06;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().BeFalse();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x85;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().BeTrue();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().BeFalse();
                }

                [Fact]
                public void IncrementsInstructionPointerByTwo()
                {
                    var sut = CreateSut();

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);

                    var expectedPointer = sut.InstructionPointer.Plus(2);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void Consumes5Cycles()
                {
                    var sut = CreateSut();

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);

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
                    _op = new OpcodeDefinitions().FindOpcode(Operation.INC, AddressMode.ZeroPageX);

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
                [InlineData(0x00, 0x15, 0x0015, 0x00, 0x01)]
                [InlineData(0x30, 0x24, 0x0054, 0x63, 0x64)]
                [InlineData(0xC7, 0xD1, 0x0098, 0xFE, 0xFF)]
                [InlineData(0xC7, 0x00, 0x00C7, 0xFF, 0x00)]
                public void IncrementsCorrectAddress(byte operand, byte xOffset, ushort expectedTargetAddr, byte valueBefore, byte valueAfter)
                {
                    var sut = CreateSut();
                    sut.LDX(xOffset, _memory);

                    A.CallTo(() => _memory.Read(expectedTargetAddr))
                        .Returns(valueBefore);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(expectedTargetAddr, valueAfter))
                        .MustHaveHappened();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0xFF;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().BeTrue();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x06;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Zero)
                        .Should().BeFalse();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x85;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().BeTrue();
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(initialFlags);

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().BeFalse();
                }

                [Fact]
                public void IncrementsInstructionPointerByTwo()
                {
                    var sut = CreateSut();

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);

                    var expectedPointer = sut.InstructionPointer.Plus(2);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void Consumes6Cycles()
                {
                    var sut = CreateSut();

                    byte address = 0x03;
                    byte value = 0x38;
                    
                    A.CallTo(() => _memory.Read(address))
                        .Returns(value);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(address);

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
                    _op = new OpcodeDefinitions().FindOpcode(Operation.INC, AddressMode.Absolute);

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
                public void IncrementsCorrectAddress()
                {
                    Assert.True(false, "Todo: ");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void IncrementsInstructionPointerBy3()
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void Consumes6Cycles()
                {
                    Assert.True(false, "Todo");
                }
            }

            public class AbsoluteX
            {
                private IMemory _memory;
                private OpCode _op;

                public AbsoluteX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.INC, AddressMode.AbsoluteX);

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
                public void IncrementsCorrectAddress()
                {
                    Assert.True(false, "Todo: ");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void IncrementsInstructionPointerBy3()
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void Consumes7Cycles()
                {
                    Assert.True(false, "Todo");
                }
            }
        }
    }
}