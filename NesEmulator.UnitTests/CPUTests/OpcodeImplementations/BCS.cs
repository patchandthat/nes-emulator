using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    
        public static class BCS
        {
            public class Relative
            {
                private IMemory _memory;
                private OpCode _op;

                public Relative()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpCodes().FindOpcode(Operation.BCS, AddressMode.Relative);

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
                public void InstructionPointerIncreasesBy2WhenCarryIsClear()
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    var expectedPointer = sut.InstructionPointer.Plus(2);

                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Fact]
                public void ExecutionTakes2CyclesWhenCarryIsClear()
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    var expectedCycles = sut.ElapsedCycles + 2;

                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }

                [Theory]
                [InlineData(38)]
                [InlineData(-25)]
                public void InstructionPointerOffsetByTwoPlusSignedOperandWhenCarryIsSet(sbyte operand)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(StatusFlags.Carry);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns((byte) operand);

                    var expectedPointer = sut
                        .InstructionPointer
                        .Plus(2)
                        .Plus(operand);

                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Theory]
                [InlineData(1, true, 0xFC)]
                [InlineData(87, false, 0x01)]
                public void ExecutionTakes3CyclesWhenBranchingOnSamePage(sbyte jumpOffset, bool runNops, byte ipLowByte)
                {
                    var sut = CreateSut();
                    byte operand = (byte) jumpOffset;

                    while (runNops && sut.InstructionPointer % 256 < ipLowByte)
                    {
                        sut.NOP(_memory);
                    }

                    sut.ForceStatus(StatusFlags.Carry);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    var expectedCycles = sut.ElapsedCycles + 3;

                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }

                [Theory]
                [InlineData(1, true, 0xFD)]
                [InlineData(-30, false, 0x00)]
                public void ExecutionTakes4CyclesWhenBranchingCrossPage(sbyte jumpOffset, bool runNops, byte ipLowByte)
                {
                    var sut = CreateSut();
                    byte operand = (byte) jumpOffset;

                    while (runNops && sut.InstructionPointer % 256 < ipLowByte)
                    {
                        sut.NOP(_memory);
                    }

                    sut.ForceStatus(StatusFlags.Carry);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);
                    A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                        .Returns(operand);

                    var expectedCycles = sut.ElapsedCycles + 4;

                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }
            }
        }
    }
