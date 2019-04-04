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
        public class INY
        {
            public class Implicit
            {
                private IMemory _memory;
                private OpCode _op;

                public Implicit()
                {
                    _memory = A.Fake<IMemory>();
                    _op = new OpcodeDefinitions().FindOpcode(Operation.INY, AddressMode.Implicit);

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

                [Theory]
                [InlineData(0x00, 0x01)]
                [InlineData(0x6D, 0x6E)]
                [InlineData(0xFF, 0x00)]
                public void IncrementsYRegisterValue(byte start, byte expectedResult)
                {
                    var sut = CreateSut();
                    sut.LDY(start, _memory);

                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);
                    
                    sut.Step();

                    sut.IndexY.Should().Be(expectedResult);
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetZeroFlagIfNewRegisterValueIsZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo: ");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearZeroFlagIfNewRegisterValueIsNotZero(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo: ");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void SetNegativeFlagIfNewRegisterValueIsNegative(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo: ");
                }

                [Theory]
                [InlineData(StatusFlags.None)]
                [InlineData(StatusFlags.All)]
                public void ClearNegativeFlagIfNewRegisterValueIsNotNegative(StatusFlags initialFlags)
                {
                    Assert.True(false, "Todo: ");
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
                public void IncrementsInstructionPointerBy1()
                {
                    var sut = CreateSut();
                    
                    A.CallTo(() => _memory.Read(sut.InstructionPointer))
                        .Returns(_op.Hex);

                    var expectedPointer = sut.InstructionPointer.Plus(1);
                    
                    sut.Step();

                    sut.InstructionPointer.Should().Be(expectedPointer);
                }
            }
        }
    }
}