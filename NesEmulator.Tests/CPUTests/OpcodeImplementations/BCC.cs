using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class BCC
    {
        [Trait("Category", "Unit")]
        public class Relative
        {
            public Relative()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.BCC, AddressMode.Relative);

                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(38)]
            [InlineData(-25)]
            public void InstructionPointerOffsetByTwoPlusSignedOperandWhenCarryIsClear(sbyte operand)
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
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
                var operand = (byte) jumpOffset;

                while (runNops && sut.InstructionPointer % 256 < ipLowByte) sut.NOP(_memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
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
                var operand = (byte) jumpOffset;

                while (runNops && sut.InstructionPointer % 256 < ipLowByte) sut.NOP(_memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void ExecutionTakes2CyclesWhenCarryIsSet()
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIncreasesBy2WhenCarryIsSet()
            {
                var sut = CreateSut();
                sut.ForceStatus(StatusFlags.Carry);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
    }
}