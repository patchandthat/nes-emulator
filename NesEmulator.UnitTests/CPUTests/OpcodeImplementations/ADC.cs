using FakeItEasy;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class ADC
    {
        public class Immediate
        {
            public Immediate()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.Immediate);

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

            [Fact]
            public void AddsOperandToCurrentAccumulatorValue()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void AddsOneToResultIfCarryFlagWasSet()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void InstructionPointerIncrementsTwoBytes()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void ExecutionTakesTwoCycles()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void CarryFlagRaisedIfUnsignedOverflowOccurred()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void CarryFlagClearedIfNoUnsignedOverflowHappened()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void OverflowFlagRaisedIfSignedOverflowOccurred()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void OverflowFlagClearedIfNoSignedOverflowOccurred()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void ZeroFlagSetOfResultIsZero()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void ZeroFlagClearedIfResultIsNotZero()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void NegativeFlagRaisedIfResultBit7IsHigh()
            {
                Assert.True(false, "Todo: ");
            }

            [Fact]
            public void NegativeFlagClearedIfResultBit7IsLow()
            {
                Assert.True(false, "Todo: ");
            }
        }

        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.ZeroPage);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: ");
            }
        }

        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.ZeroPageX);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: Zero page wrap");
            }
        }
    
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.Absolute);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: ");
            }
        }

        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.AbsoluteX);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: Page cross penalty");
            }
        }

        public class AbsoluteY
        {
            public AbsoluteY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.AbsoluteY);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: Page cross penalty");
            }
        }

        public class IndirectX
        {
            public IndirectX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.IndirectX);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: Zero page wrap");
            }
        }

        public class IndirectY
        {
            public IndirectY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.ADC, AddressMode.IndirectY);

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
            
            [Fact]
            public void TestName()
            {
                Assert.True(false, "Todo: PageCross penalty");
            }
        }
    }
}