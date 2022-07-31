using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class TAX
    {
        [Trait("Category", "Unit")]
        public class Implied
        {
            public Implied()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.TAX, AddressMode.Implicit);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(0x00)]
            [InlineData(0x25)]
            [InlineData(0x6A)]
            [InlineData(0xFC)]
            public void TransfersAccumulatorValueToX(byte value)
            {
                var sut = CreateSut();
                sut.LDA(value, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.IndexX.Should().Be(value);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfXIsNowZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0x0, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfXIsNowNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0xFF, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfSignBitOfXIsNowHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0xFF, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfSignBitOfXIsNowLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDA(0x15, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void ElapsesTwoCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedValue = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedValue);
            }

            [Fact]
            public void IncrementsInstructionPointerBy1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedValue = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedValue);
            }
        }
    }
}