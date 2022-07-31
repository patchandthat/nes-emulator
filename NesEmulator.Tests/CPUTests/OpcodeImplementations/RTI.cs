using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class RTI
    {
        [Trait("Category", "Unit")]
        public class Implicit
        {
            public Implicit()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.RTI, AddressMode.Implicit);

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
            [InlineData(0b0000_0000, StatusFlags.None | StatusFlags.Bit5)]
            [InlineData(0b0000_0001, StatusFlags.Carry | StatusFlags.Bit5)]
            [InlineData(0b0000_0010, StatusFlags.Zero | StatusFlags.Bit5)]
            [InlineData(0b0000_0100, StatusFlags.InterruptDisable | StatusFlags.Bit5)]
            [InlineData(0b0000_1000, StatusFlags.Decimal | StatusFlags.Bit5)]
            [InlineData(0b0001_0000, StatusFlags.Bit5)]
            [InlineData(0b0010_0000, StatusFlags.Bit5)]
            [InlineData(0b0100_0000, StatusFlags.Overflow | StatusFlags.Bit5)]
            [InlineData(0b1000_0000, StatusFlags.Negative | StatusFlags.Bit5)]
            [InlineData(0b1111_1111, StatusFlags.All & ~StatusFlags.Bit4)]
            public void RestoresStatusFlagsFromStack(byte storedStatus, StatusFlags expectedStatus)
            {
                var sut = CreateSut();
                sut.ForceStack(0x0134);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                A.CallTo(() => _memoryBus.Read(sut.StackPointer.Plus(1)))
                    .Returns(storedStatus);
                
                sut.Step();

                sut.Status.Should().Be(expectedStatus);
            }

            [Fact]
            public void RestoresInstructionPointerFromStack()
            {
                ushort stackStart = 0x0134;
                ushort lowByteAddr = stackStart.Plus(2);
                ushort highByteAddr = stackStart.Plus(3);

                byte low = 0x47;
                byte high = 0xE4;
                ushort expectedInstructionPointer = 0xE447;
                
                var sut = CreateSut();
                sut.ForceStack(stackStart);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                A.CallTo(() => _memoryBus.Read(lowByteAddr))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(highByteAddr))
                    .Returns(high);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }

            [Fact]
            public void StackPointerIncrementsByThree()
            {
                var sut = CreateSut();
                ushort stackStart = 0x0134;
                sut.ForceStack(stackStart);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                
                sut.Step();

                sut.StackPointer.Should().Be(stackStart.Plus(3));
            }

            [Fact]
            public void ExecutionTakesSixCycles()
            {
                var sut = CreateSut();
                ushort stackStart = 0x0111;
                sut.ForceStack(stackStart);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}