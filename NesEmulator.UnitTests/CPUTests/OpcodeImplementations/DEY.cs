using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class DEY
    {
        public class Implicit
        {
            public Implicit()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.DEY, AddressMode.Implicit);

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
            [InlineData(0x00, 0xFF)]
            [InlineData(0x6D, 0x6C)]
            [InlineData(0xFF, 0xFE)]
            public void IncrementsYRegisterValue(byte start, byte expectedResult)
            {
                var sut = CreateSut();
                sut.LDY(start, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.IndexY.Should().Be(expectedResult);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetZeroFlagIfNewRegisterValueIsZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDY(0x01, _memory);
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
            public void ClearZeroFlagIfNewRegisterValueIsNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDY(0x05, _memory);
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
            public void SetNegativeFlagIfNewRegisterValueIsNegative(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDY(0xCA, _memory);
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
            public void ClearNegativeFlagIfNewRegisterValueIsNotNegative(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDY(0x2A, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void ExecutionTakes2Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 2;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IncrementsInstructionPointerBy1()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedPointer = sut.InstructionPointer.Plus(1);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
    }
}