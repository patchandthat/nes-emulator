using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class SED
    {
        [Trait("Category", "Unit")]
        public class Implicit
        {
            public Implicit()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.SED, AddressMode.Implicit);

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
            [InlineData(StatusFlags.All, StatusFlags.All)]
            [InlineData(StatusFlags.None, StatusFlags.Decimal)]
            public void ShouldSetDecimalFlag(StatusFlags initialFlags, StatusFlags expectedFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.Should().Be(expectedFlags);
            }

            [Fact]
            public void ExecutionTakes2Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIncreasesBy1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
    }
}