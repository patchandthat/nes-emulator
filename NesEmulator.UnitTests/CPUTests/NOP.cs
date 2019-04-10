using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public static class NOP
        {
            public class Implicit
            {
                private IMemory _memory;
                private OpCode _op;

                public Implicit()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.NOP, AddressMode.Implicit);

                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                        .Returns((byte) 0x00);
                    A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                        .Returns((byte) 0x80);
                }

                private CPU CreateSut()
                {
                    var cpu = new CPU(_memory);
                    cpu.Power();
                    cpu.Step();
                    Fake.ClearRecordedCalls(_memory);
                    return cpu;
                }

                [Fact]
                public void ExecutionTakes2Cycles()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedCycles = sut.ElapsedCycles + 2;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }

                [Fact]
                public void InstructionPointerIncreasesBy1()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedPointer = sut.InstructionPointer.Plus(1);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }

                [Theory]
                [InlineData(StatusFlags.All)]
                [InlineData(StatusFlags.None)]
                public void FlagsAreUnchanged(StatusFlags flags)
                {
                    var sut = CreateSut();
                    sut.ForceStatus(flags);
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Status.Should().Be(flags);
                }
            }
        }
    }
}