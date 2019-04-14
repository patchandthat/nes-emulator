using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class TXS
    {
        public class Implied
        {
            public Implied()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.TXS, AddressMode.Implicit);
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
            [InlineData(0xD3)]
            [InlineData(0x16)]
            [InlineData(0x7A)]
            public void TransfersXValueToStackPointer(byte value)
            {
                var sut = CreateSut();
                sut.LDX(value, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                var expectedValue = (ushort) (0x0100 + value);

                sut.StackPointer.Should().Be(expectedValue);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DoesNotAffectStatusFlags(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDX(0x8C, _memory);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.Status.Should().Be(initialFlags);
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