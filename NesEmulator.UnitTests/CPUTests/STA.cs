using FakeItEasy;
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