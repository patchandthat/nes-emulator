using FakeItEasy;
using FluentAssertions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public static class STA
        {
            public class ZeroPage
            {
                private IMemory _memory;
                private OpCode _op;

                public ZeroPage()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.ZeroPage);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }
                
                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step(); // Execute reset interrupt
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }
                
                [Theory]
                [InlineData(0x3E, 0x56, 0x0056)]
                [InlineData(0x72, 0x8B, 0x008B)]
                [InlineData(0xA3, 0xFF, 0x00FF)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte operand, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(expectedAddress, value))
                        .MustHaveHappened();
                }

                [Theory]
                [InlineData(StatusFlags.All)]
                [InlineData(StatusFlags.None)]
                public void DoesNotModifyAnyFlags(StatusFlags flagStates)
                {
                    var sut = CreateSut();

                    sut.LDA(0xFF, _memory);
                    sut.ForceStatus(flagStates);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);
                    
                    sut.Step();

                    sut.Status.Should().Be(flagStates);
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedCycleCount = sut.ElapsedCycles + _op.Cycles;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycleCount);
                }

                [Fact]
                public void IncreasesInstructionPointer()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }

            public class ZeroPageX
            {
                private IMemory _memory;
                private OpCode _op;

                public ZeroPageX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.ZeroPageX);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
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
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.Absolute);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
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
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.AbsoluteX);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
                {
                    Assert.True(false, "Todo: ");
                }
            }

            public class AbsoluteY
            {
                private IMemory _memory;
                private OpCode _op;

                public AbsoluteY()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.AbsoluteY);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
                {
                    Assert.True(false, "Todo: ");
                }
            }
            
            public class IndirectX
            {
                private IMemory _memory;
                private OpCode _op;

                public IndirectX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.IndirectX);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
                {
                    Assert.True(false, "Todo: ");
                }
            }

            public class IndirectY
            {
                private IMemory _memory;
                private OpCode _op;

                public IndirectY()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.STA, AddressMode.IndirectY);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
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
                public void WritesValueToCorrectMemoryLocation()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void DoesNotModifyAnyFlags()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesElapsedCycleCount()
                {
                    Assert.True(false, "Todo: ");
                }

                [Fact]
                public void IncreasesInstructionPointer()
                {
                    Assert.True(false, "Todo: ");
                }
            }
        }
    }
}