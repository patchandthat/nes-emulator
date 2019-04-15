using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class CMP
    {
        public class Immediate
        {
            public Immediate()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.Immediate);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes2Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 2;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
        
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.ZeroPage);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.ZeroPageX);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.Absolute);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.AbsoluteX);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }

            [Fact]
            public void PageCrossPenalty()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class AbsoluteY
        {
            public AbsoluteY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.AbsoluteY);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class IndirectX
        {
            public IndirectX()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.IndirectX);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
        }
        
        public class IndirectY
        {
            public IndirectY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.IndirectY);

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
            public void Todo()
            {
                Assert.True(false, "Todo");
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                Assert.True(false, "Todo");
            }
        }
    }
}