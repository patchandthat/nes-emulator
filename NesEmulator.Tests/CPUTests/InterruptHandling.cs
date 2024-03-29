﻿using FakeItEasy;
using FluentAssertions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    [Trait("Category", "Unit")]
    public class InterruptHandling
    {
        public InterruptHandling()
        {
            _memoryBus = A.Fake<IMemoryBus>();
        }

        private readonly IMemoryBus _memoryBus;

        private CPU CreateSut()
        {
            var cpu = new CPU(_memoryBus);

            cpu.Power();

            return cpu;
        }

        [Fact]
        public void PowerOnInterrupt_OnStep_WillSetInstructionPointer()
        {
            A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                .Returns((byte) 0xFC);
            A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                .Returns((byte) 0x94);

            var sut = CreateSut();

            sut.Step();

            sut.InstructionPointer.Should().Be(0x94FC);
        }

        [Fact(Skip = "Todo")]
        public void Todo()
        {
            Assert.True(false, "Back-fill Interrupt tests");
        }
    }
}