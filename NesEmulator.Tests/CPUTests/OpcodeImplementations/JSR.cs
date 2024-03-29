using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class JSR
    {
        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.JSR, AddressMode.Absolute);

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

            [Fact]
            public void PushReturnAddressMinusOneToStack()
            {
                byte jumpTargetLow = 0x0;
                byte jumpTargetHigh = 0x0;
                
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read( sut.InstructionPointer.Plus(1)))
                    .Returns(jumpTargetLow);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(jumpTargetHigh);

                ushort addressForInstructionAfterReturn = 
                    sut.InstructionPointer.Plus(_op.Bytes);
                ushort expectedPushValue = addressForInstructionAfterReturn.Plus(-1);

                byte lowByte = (byte)(expectedPushValue % 256);
                byte highByte = (byte)(expectedPushValue >> 8);

                ushort highPushAddress = sut.StackPointer;
                ushort lowPushAddress = sut.StackPointer.Plus(-1);
                
                sut.Step();

                A.CallTo(() => _memoryBus.Write(highPushAddress, highByte))
                    .MustHaveHappened();
                A.CallTo(() => _memoryBus.Write(lowPushAddress, lowByte))
                    .MustHaveHappened();
            }

            [Fact]
            public void StackPointerIsReducedByTwoBytes()
            {
                byte jumpTargetLow = 0x0;
                byte jumpTargetHigh = 0x0;
                
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read( sut.InstructionPointer.Plus(1)))
                    .Returns(jumpTargetLow);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(jumpTargetHigh);

                var expectedStackPointer = sut.StackPointer.Plus(-2);
                
                sut.Step();

                sut.StackPointer.Should().Be(expectedStackPointer);
            }

            [Fact]
            public void SetsInstructionPointerToDestinationAddress()
            {
                byte jumpTargetLow = 0xD4;
                byte jumpTargetHigh = 0xC2;
                const int expectedAddress = 0xC2D4;
                
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read( sut.InstructionPointer.Plus(1)))
                    .Returns(jumpTargetLow);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(jumpTargetHigh);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedAddress);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DoesNotAffectFlags(StatusFlags constFlags)
            {
                byte jumpTargetLow = 0xD4;
                byte jumpTargetHigh = 0xC2;
                
                var sut = CreateSut();
                sut.ForceStatus(constFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read( sut.InstructionPointer.Plus(1)))
                    .Returns(jumpTargetLow);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(jumpTargetHigh);

                sut.Step();

                sut.Status.Should().Be(constFlags);
            }

            [Fact]
            public void ExecutionTakesSixCycles()
            {
                byte jumpTargetLow = 0xD4;
                byte jumpTargetHigh = 0xC2;
                
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read( sut.InstructionPointer.Plus(1)))
                    .Returns(jumpTargetLow);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(jumpTargetHigh);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
    }
}