using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class PLP
    {
        public class Implicit
        {
            public Implicit()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.PLP, AddressMode.Implicit);

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
            [InlineData(0x00, StatusFlags.None)]
            [InlineData(0xFF, StatusFlags.All)]
            [InlineData(0x01, StatusFlags.Carry)]
            [InlineData(0x02, StatusFlags.Zero)]
            [InlineData(0x04, StatusFlags.InterruptDisable)]
            [InlineData(0x08, StatusFlags.Decimal)]
            [InlineData(0x10, StatusFlags.Bit4)]
            [InlineData(0x20, StatusFlags.Bit5)]
            [InlineData(0x40, StatusFlags.Overflow)]
            [InlineData(0x80, StatusFlags.Negative)]
            public void PullsStatusFlagsValueFromStack(byte value, StatusFlags expectedFlag)
            {
                var sut = CreateSut();
                sut.ForceStack(0x0166);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.StackPointer.Plus(1)))
                    .Returns(value);

                sut.Step();

                sut.Status.Should().Be(expectedFlag);
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerMoves1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedIp = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIp);
            }

            [Fact]
            public void StackPointerIncreases()
            {
                var sut = CreateSut();
                ushort startingStackPointer = 0x0145;
                var expectedStackPointer = startingStackPointer.Plus(1);
                sut.ForceStack(startingStackPointer);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(expectedStackPointer);
            }

            [Fact]
            // ReSharper disable once InconsistentNaming
            public void StackPointerWrapsAt0x0200()
            {
                var sut = CreateSut();

                sut.StackPointer.Should().Be(0x01FF, "Precondition failed");

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(0x0100);
            }
        }
    }
}