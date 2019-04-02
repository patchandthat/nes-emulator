using FakeItEasy;
using FluentAssertions;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public class InterruptHandling
        {
            private IMemory _memory;

            public InterruptHandling()
            {
                _memory = A.Fake<IMemory>();
            }

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);

                cpu.Power();

                return cpu;
            }

            [Fact]
            public void PowerOnInterrupt_OnStep_WillSetInstructionPointer()
            {
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte)0xFC);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector+1))
                    .Returns((byte)0x94);
                
                var sut = CreateSut();

                sut.Step();

                sut.InstructionPointer.Should().Be(0x94FC);
            }
        }
    }
}
