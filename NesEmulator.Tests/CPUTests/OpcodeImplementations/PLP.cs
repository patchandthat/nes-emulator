using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class PLP
    {
        [Trait("Category", "Unit")]
        public class Implicit
        {
            public Implicit()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.PLP, AddressMode.Implicit);

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
            [InlineData(0x00, StatusFlags.Bit5)]
            [InlineData(0xFF, StatusFlags.All & ~StatusFlags.Bit4)]
            [InlineData(0x01, StatusFlags.Carry | StatusFlags.Bit5)]
            [InlineData(0x02, StatusFlags.Zero | StatusFlags.Bit5)]
            [InlineData(0x04, StatusFlags.InterruptDisable | StatusFlags.Bit5)]
            [InlineData(0x08, StatusFlags.Decimal | StatusFlags.Bit5)]
            [InlineData(0x10, StatusFlags.Bit5)]
            [InlineData(0x20, StatusFlags.Bit5)]
            [InlineData(0x40, StatusFlags.Overflow | StatusFlags.Bit5)]
            [InlineData(0x80, StatusFlags.Negative | StatusFlags.Bit5)]
            public void PullsStatusFlagsValueFromStack(byte value, StatusFlags expectedFlag)
            {
                var sut = CreateSut();
                sut.ForceStack(0x0166);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.StackPointer.Plus(1)))
                    .Returns(value);

                sut.Step();

                sut.Status.Should().Be(expectedFlag);
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 4;

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
            public void StackPointerIncreases()
            {
                var sut = CreateSut();
                ushort startingStackPointer = 0x0145;
                var expectedStackPointer = startingStackPointer.Plus(1);
                sut.ForceStack(startingStackPointer);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(expectedStackPointer);
            }

            [Fact]
            // ReSharper disable once InconsistentNaming
            public void StackPointerWrapsAt0x0200()
            {
                var sut = CreateSut();
                sut.ForceStack(0x01FF);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                sut.Step();

                sut.StackPointer.Should().Be(0x0100);
            }
        }
    }
}