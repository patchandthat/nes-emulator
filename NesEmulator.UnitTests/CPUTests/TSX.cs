using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {

        public static class TSX
        {
            public class Implied
            {
                private readonly IMemory _memory;
                private readonly OpCode _op;

                public Implied()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.TSX, AddressMode.Implicit);
                }
                
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
                [InlineData(0x01E2)]
                [InlineData(0x0134)]
                [InlineData(0x01F2)]
                [InlineData(0xFFFF)]
                public void TransfersStackPointerValueToX(ushort pointer)
                {
                    var sut = CreateSut();
                    sut.ForceStack(pointer);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    byte expectedValue = (byte)(pointer % 256);

                    sut.IndexX.Should().Be(expectedValue);
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetsZeroFlagIfXIsNowZero(StatusFlags initialFlags)
                {
                    var sut = CreateSut();
                    sut.ForceStack(0x0100);
                    sut.ForceStatus(initialFlags);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
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
                    sut.ForceStack(0x0101);
                    sut.ForceStatus(initialFlags);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
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
                    sut.ForceStack(0x01FF);
                    sut.ForceStatus(initialFlags);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
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
                    sut.ForceStack(0x017F);
                    sut.ForceStatus(initialFlags);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.Status.HasFlag(StatusFlags.Negative)
                        .Should().Be(false);
                }

                [Fact]
                public void IncrementsInstructionPointerBy1()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    var expectedValue = sut.InstructionPointer.Plus(1);

                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedValue);
                }

                [Fact]
                public void ElapsesTwoCycles()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedValue = sut.ElapsedCycles + 2;

                    sut.Step();

                    sut.ElapsedCycles.Should().Be(expectedValue);
                }
            }
        }
    }
}