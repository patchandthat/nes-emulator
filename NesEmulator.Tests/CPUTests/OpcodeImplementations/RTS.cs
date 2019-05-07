using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class RTS
    {
        public class Implicit
        {
            public Implicit()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.RTS, AddressMode.Implicit);

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
            public void PullInstructionPointerFromStack()
            {
                var sut = CreateSut();
                sut.ForceStack(0x01FA);

                byte low = 0x2D;
                byte high = 0x9F;
                ushort expectedInstructionPointer = 0x9F2E;

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.StackPointer))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.StackPointer.Plus(1)))
                    .Returns(high);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }

            [Fact]
            public void StackPointerIncreasesByTwo()
            {
                var sut = CreateSut();
                const ushort stackStart = 0x01FA;
                const ushort expectedStack = 0x01FC;
                sut.ForceStack(stackStart);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                
                sut.Step();

                sut.StackPointer.Should().Be(expectedStack);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DoesNotAffectFlags(StatusFlags constFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(constFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                
                sut.Step();

                sut.Status.Should().Be(constFlags);
            }

            [Fact]
            public void ExecutionTakesSixCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}