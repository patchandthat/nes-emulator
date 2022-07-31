using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class PHA
    {
        [Trait("Category", "Unit")]
        public class Implicit
        {
            public Implicit()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.PHA, AddressMode.Implicit);

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
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void StatusFlagsAreUnchanged(StatusFlags flags)
            {
                var sut = CreateSut();

                sut.ForceStatus(flags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.Should().Be(flags);
            }

            [Fact]
            public void ExecutionTakes3Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 3;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerMoves1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedIp = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIp);
            }

            [Fact]
            public void PushesAccumulatorValueOntoStack()
            {
                var sut = CreateSut();

                byte value = 0x9F;
                sut.LDA(value, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var sp = sut.StackPointer;

                sut.Step();

                A.CallTo(() => _memoryBus.Write(sp, value))
                    .MustHaveHappened();
            }

            [Fact]
            public void StackPointerDecreases()
            {
                var sut = CreateSut();

                byte value = 0x9F;
                sut.LDA(value, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var sp = sut.StackPointer;

                sut.Step();

                sut.StackPointer.Should().Be(sp.Plus(-1));
            }

            [Fact]
            // ReSharper disable once InconsistentNaming
            public void StackPointerWrapsAt0x0100()
            {
                var sut = CreateSut();

                byte value = 0x9F;
                sut.LDA(value, _memoryBus);

                for (var i = 0; i < 253; i++) // 3 bytes initially on the stack at power on
                {
                    A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    sut.Step();
                }

                sut.StackPointer.Should().Be(0x0100, "Precondition failed");

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(0x01FF);
            }
        }
    }
}