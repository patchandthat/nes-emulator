using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class JMP
    {
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.JMP, AddressMode.Absolute);

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
            [InlineData(0x05, 0x80, 0x8005)]
            [InlineData(0x67, 0x84, 0x8467)]
            public void InstructionPointerBecomesValueSpecified(byte low, byte high, ushort expectedAddress)
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedAddress);
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
            public void ExecutionTakesThreeCycles()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }

        public class Indirect
        {
            public Indirect()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.JMP, AddressMode.Indirect);

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
            public void InstructionPointerBecomesValueSpecified()
            {
                byte indirectLow = 0xA7;
                byte indirectHigh = 0x04;
                ushort indirectTargetAddress = 0x04A7;
                byte low = 0x6D;
                byte high = 0xAC;
                ushort expectedAddress = 0xAC6D;
                
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(indirectLow);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(indirectHigh);
                A.CallTo(() => _memory.Read(indirectTargetAddress))
                    .Returns(low);
                A.CallTo(() => _memory.Read(indirectTargetAddress.Plus(1)))
                    .Returns(high);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedAddress);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void DoesNotAffectFlags(StatusFlags constFlags)
            {
                byte indirectLow = 0xA7;
                byte indirectHigh = 0x04;
                ushort indirectTargetAddress = 0x04A7;
                byte low = 0x6D;
                byte high = 0xAC;
                
                var sut = CreateSut();
                sut.ForceStatus(constFlags);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(indirectLow);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(indirectHigh);
                A.CallTo(() => _memory.Read(indirectTargetAddress))
                    .Returns(low);
                A.CallTo(() => _memory.Read(indirectTargetAddress.Plus(1)))
                    .Returns(high);
                
                sut.Step();

                sut.Status.Should().Be(constFlags);
            }

            [Fact]
            public void ExecutionTakesFiveCycles()
            {
                byte indirectLow = 0xA7;
                byte indirectHigh = 0x04;
                ushort indirectTargetAddress = 0x04A7;
                byte low = 0x6D;
                byte high = 0xAC;
                
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(indirectLow);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(indirectHigh);
                A.CallTo(() => _memory.Read(indirectTargetAddress))
                    .Returns(low);
                A.CallTo(() => _memory.Read(indirectTargetAddress.Plus(1)))
                    .Returns(high);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void PageBoundaryBugIsEmulated()
            {
                /*
                 * An original 6502 has does not correctly fetch the target address if the indirect vector falls on
                 * a page boundary (e.g. $xxFF where xx is any value from $00 to $FF). In this case fetches the LSB
                 * from $xxFF as expected but takes the MSB from $xx00. This is fixed in some later chips like the
                 * 65SC02.
                 *
                 * As far as I can tell, the RICOH 6502 in the Nes is affected by this bug
                 */
                
                byte indirectLow = 0xFF;
                byte indirectHigh = 0x04;
                ushort indirectLowByteTargetAddress = 0x04FF;
                ushort indirectHighByteTargetAddress = 0x0400;
                byte low = 0xC3;
                byte high = 0xF2;
                ushort expectedAddress = 0xF2C3;
                
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(indirectLow);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(indirectHigh);
                A.CallTo(() => _memory.Read(indirectLowByteTargetAddress))
                    .Returns(low);
                A.CallTo(() => _memory.Read(indirectHighByteTargetAddress))
                    .Returns(high);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedAddress);
            }
        }
    }
}