using FakeItEasy;
using NesEmulator.Processor;
using Xunit;

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
                public void IncrementsInstructionPointerByTwo()
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void Consumes5Cycles()
                {
                    Assert.True(false, "Todo");
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
                public void IncrementsInstructionPointerByTwo()
                {
                    Assert.True(false, "Todo");
                }

                [Fact]
                public void Consumes6Cycles()
                {
                    Assert.True(false, "Todo");
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