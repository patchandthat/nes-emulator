﻿using FakeItEasy;
using FluentAssertions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    [Trait("Category", "Unit")]
    public class PowerOnState
    {
        public PowerOnState()
        {
            _memoryBus = A.Fake<IMemoryBus>();
        }

        private readonly IMemoryBus _memoryBus;

        private CPU CreateSut()
        {
            return new CPU(_memoryBus);
        }

        [Fact]
        public void ctor_WhenCalled_PowerStateIsOff()
        {
            var sut = CreateSut();

            sut.IsPowerOn.Should().BeFalse();
        }

        [Fact]
        public void OnPower_WhenCalled_WillTogglePowerState()
        {
            var sut = CreateSut();

            sut.Power();

            sut.IsPowerOn.Should().BeTrue();

            sut.Power();

            sut.IsPowerOn.Should().BeFalse();
        }

        [Fact]
        public void WhenPowerOn_AccumulatorValueIsZero()
        {
            var sut = CreateSut();

            sut.Power();

            sut.Accumulator.Should().Be(0);
        }

        [Fact]
        public void WhenPowerOn_ElapsedCyclesIsZero()
        {
            var sut = CreateSut();

            sut.Power();

            sut.ElapsedCycles.Should().Be(0);
        }

        [Fact]
        public void WhenPowerOn_IndexXValueIsZero()
        {
            var sut = CreateSut();

            sut.Power();

            sut.IndexX.Should().Be(0);
        }

        [Fact]
        public void WhenPowerOn_IndexYValueIsZero()
        {
            var sut = CreateSut();

            sut.Power();

            sut.IndexY.Should().Be(0);
        }

        [Fact]
        public void WhenPowerOn_InstructionPointerValueIsResetVector()
        {
            var sut = CreateSut();

            sut.Power();

            sut.InstructionPointer.Should().Be(MemoryMap.ResetVector);
        }

        [Fact]
        public void WhenPowerOn_StackPointerValueIs0x0100()
        {
            var sut = CreateSut();

            sut.Power();

            sut.StackPointer.Should().Be(0x0100);
        }

        [Fact]
        public void WhenPowerOn_StatusFlagsSetForInterruptDisable()
        {
            var expectedFlags = StatusFlags.InterruptDisable | StatusFlags.Bit5;

            var sut = CreateSut();

            sut.Power();

            sut.Status.Should().Be(expectedFlags);
        }

        [Fact]
        public void WhenPowerOn_WillDisableAllSoundChannels()
        {
            var sut = CreateSut();

            sut.Power();

            A.CallTo(() => _memoryBus.Write(MemoryMap.ApuSoundChannelStatus, 0x00))
                .MustHaveHappened();
        }

        [Fact]
        public void WhenPowerOn_WillSetAllApuParamsToZero()
        {
            const ushort startAddress = 0x4000;
            const ushort endAddress = 0x400F;

            var sut = CreateSut();

            sut.Power();

            for (var i = startAddress; i <= endAddress; i++)
            {
                var address = i;
                A.CallTo(() => _memoryBus.Write(address, 0x00))
                    .MustHaveHappened();
            }
        }

        [Fact]
        public void WhenPowerOn_WillWriteApuFrameCounter_0x00()
        {
            var sut = CreateSut();

            sut.Power();

            A.CallTo(() => _memoryBus.Write(MemoryMap.ApuFrameCounter, 0x00))
                .MustHaveHappened();
        }
    }
}