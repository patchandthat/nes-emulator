using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
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
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.ZeroPage);

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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.ZeroPageX);

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
                [InlineData(0x3E, 0x00, 0x56, 0x0056)]
                [InlineData(0x72, 0x10, 0x8B, 0x009B)]
                [InlineData(0xA3, 0x20, 0xFF, 0x001F)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte xOffset, byte operand, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDX(xOffset, _memory);
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }

            public class Absolute
            {
                private IMemory _memory;
                private OpCode _op;

                public Absolute()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.Absolute);

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
                [InlineData(0x3E, 0x56, 0x03, 0x0356)]
                [InlineData(0x72, 0x8B, 0x00, 0x008B)]
                [InlineData(0xA3, 0xFF, 0x80, 0x80FF)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte operand, byte secondOperand, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                        .Returns(secondOperand);
                    
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }

            public class AbsoluteX
            {
                private IMemory _memory;
                private OpCode _op;

                public AbsoluteX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.AbsoluteX);

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
                [InlineData(0x3E, 0x56, 0x03, 0x10, 0x0366)]
                [InlineData(0x72, 0x8B, 0x00, 0x05, 0x0090)]
                [InlineData(0xA3, 0xFF, 0x80, 0xFF, 0x81FE)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte operand, byte secondOperand, byte xOffset, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDA(value, _memory);
                    sut.LDX(xOffset, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                        .Returns(secondOperand);
                    
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }

            public class AbsoluteY
            {
                private IMemory _memory;
                private OpCode _op;

                public AbsoluteY()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.AbsoluteY);

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
                [InlineData(0x3E, 0x56, 0x03, 0x10, 0x0366)]
                [InlineData(0x72, 0x8B, 0x00, 0x05, 0x0090)]
                [InlineData(0xA3, 0xFF, 0x80, 0xFF, 0x81FE)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte operand, byte secondOperand, byte yOffset, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDA(value, _memory);
                    sut.LDY(yOffset, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                        .Returns(secondOperand);
                    
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }
            
            public class IndirectX
            {
                private IMemory _memory;
                private OpCode _op;

                public IndirectX()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.IndirectX);

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
                [InlineData(0x3E, 0x00, 0x56, 0x0056, 0x10, 0x0057, 0x01, 0x0110)]
                [InlineData(0x72, 0x10, 0x8B, 0x009B, 0x10, 0x009C, 0x01, 0x0110)]
                [InlineData(0xA3, 0x20, 0xFF, 0x001F, 0x4A, 0x0020, 0x5B, 0x5B4A)] // Zero page wrap
                [InlineData(0xA3, 0x01, 0xFE, 0x00FF, 0xCC, 0x0000, 0xDD,  0xDDCC)] // Zero page wrap
                public void WritesValueToCorrectMemoryLocation(byte value, byte xOffset, byte operand, ushort lowIndirectByteAddr, byte lowByteValue, ushort highIndirectByteAddr, byte highByteValue, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDX(xOffset, _memory);
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(lowIndirectByteAddr)).Returns(lowByteValue);
                    A.CallTo(() => _memory.Read(highIndirectByteAddr)).Returns(highByteValue);
                    
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }

            public class IndirectY
            {
                private IMemory _memory;
                private OpCode _op;

                public IndirectY()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.STA, AddressMode.IndirectY);

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
                [InlineData(0x3E, 0x00, 0x56, 0x0056, 0x10, 0x0057, 0x01, 0x0110)]
                [InlineData(0x72, 0x10, 0x8B, 0x008B, 0x10, 0x008C, 0x01, 0x0120)]
                [InlineData(0xA3, 0x20, 0xFF, 0x00FF, 0x4A, 0x0100, 0x5B, 0x5B6A)]
                [InlineData(0xA3, 0x01, 0xFE, 0x00FE, 0xCC, 0x00FF, 0xDD,  0xDDCD)]
                public void WritesValueToCorrectMemoryLocation(byte value, byte yOffset, byte operand, ushort lowIndirectByteAddr, byte lowByteValue, ushort highIndirectByteAddr, byte highByteValue, ushort expectedAddress)
                {
                    var sut = CreateSut();
                    sut.LDY(yOffset, _memory);
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    A.CallTo(() => _memory.Read(lowIndirectByteAddr)).Returns(lowByteValue);
                    A.CallTo(() => _memory.Read(highIndirectByteAddr)).Returns(highByteValue);
                    
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
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
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte)0x00);

                    var expectedInstructionPointer = sut.InstructionPointer .Plus(_op.Bytes);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedInstructionPointer);
                }
            }
        }
    }
