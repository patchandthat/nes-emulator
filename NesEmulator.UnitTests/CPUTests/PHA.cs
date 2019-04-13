using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public class PHA
        {
            public class Implicit
            {
                private IMemory _memory;
                private OpCode _op;

                public Implicit()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.PHA, AddressMode.Implicit);

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
                public void PushesAccumulatorValueOntoStack()
                {
                    var sut = CreateSut();

                    byte value = 0x9F;
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    var sp = sut.StackPointer;
                    
                    sut.Step();

                    A.CallTo(() => _memory.Write(sp, value))
                        .MustHaveHappened();
                }

                [Fact]
                public void StackPointerDecreases()
                {
                    var sut = CreateSut();

                    byte value = 0x9F;
                    sut.LDA(value, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    var sp = sut.StackPointer;
                    
                    sut.Step();

                    sut.StackPointer.Should().Be(sp.Plus(-1));
                }

                [Fact]
                // ReSharper disable once InconsistentNaming
                public void StackPointerWrapsAt0x0100()
                {
                    var sut = CreateSut();

                    byte value = 0x9F;
                    sut.LDA(value, _memory);

                    for (int i = 0; i < 255; i++)
                    {
                        A.CallTo(() => _memory.Read(sut.InstructionPointer))
                            .Returns(_op.Value);

                        sut.Step();
                    }

                    sut.StackPointer.Should().Be(0x0100, "Precondition failed");
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    sut.Step();

                    sut.StackPointer.Should().Be(0x01FF);
                }

                [Fact]
                public void InstructionPointerMoves1()
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    ushort expectedIp = sut.InstructionPointer.Plus(1);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedIp);
                }

                [Fact]
                public void ExecutionTakes3Cycles()
                {
                    var sut = CreateSut();

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    var expectedCycles = sut.ElapsedCycles + 3;
                    
                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedCycles);
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void StatusFlagsAreUnchanged(StatusFlags flags)
                {
                    var sut = CreateSut();
                    
                    sut.ForceStatus(flags);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Value);

                    sut.Step();

                    sut.Status.Should().Be(flags);
                }
            }
        }
    }
}