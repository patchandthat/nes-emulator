using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class TAY
    {
        public class Implied
        {
            public Implied()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.TAY, AddressMode.Implicit);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x05)]
            [InlineData(0x7F)]
            [InlineData(0xFF)]
            public void TransfersAccumulatorValueToY(byte value)
            {
                var sut = CreateSut();
                sut.LDA(value, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.IndexY.Should().Be(value);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfYIsNowZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0x0, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfYIsNowNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0x1, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfSignBitOfYIsNowHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0xFF, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfSignBitOfYIsNowLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0x00, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void ElapsesTwoCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedValue = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedValue);
            }

            [Fact]
            public void IncrementsInstructionPointerBy1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedValue = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedValue);
            }
        }
    }
}