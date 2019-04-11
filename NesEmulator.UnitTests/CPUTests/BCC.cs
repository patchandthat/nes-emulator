﻿using FakeItEasy;
using FluentAssertions;
using NesEmulator;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

public partial class CPUTests
{
    public static class BCC
    {
        public class Relative
        {
            private IMemory _memory;
            private OpCode _op;

            public Relative()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpcodeDefinitions().FindOpcode(Operation.BCC, AddressMode.Relative);

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
            public void InstructionPointerIncreasesBy2WhenCarryIsSet()
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakes2CyclesWhenCarryIsSet()
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(38)]
            [InlineData(-25)]
            public void InstructionPointerOffsetByTwoPlusSignedOperandWhenCarryIsClear(sbyte operand)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte)operand);

                var expectedPointer = sut
                    .InstructionPointer
                    .Plus(2)
                    .Plus(operand);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Theory]
            [InlineData(1, 0xFC)]
            [InlineData(-3, 0x01)]
            public void ExecutionTakes3CyclesWhenBranchingOnSamePage(sbyte jumpOffset, byte ipLowByte)
            {
                var sut = CreateSut();
                byte operand = (byte)jumpOffset;
                
                while (sut.InstructionPointer % 256 != ipLowByte)
                {
                    sut.NOP(_memory);
                }
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Theory]
            [InlineData(1, 0xFD)]
            [InlineData(-3, 0x00)]
            public void ExecutionTakes5CyclesWhenBranchingCrossPage(sbyte jumpOffset, byte ipLowByte)
            {
                var sut = CreateSut();
                byte operand = (byte)jumpOffset;
                
                while (sut.InstructionPointer % 256 != ipLowByte)
                {
                    sut.NOP(_memory);
                }
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}