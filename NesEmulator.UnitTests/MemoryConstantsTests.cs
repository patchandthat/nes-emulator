using FluentAssertions;
using Xunit;

// ReSharper disable IdentifierTypo

namespace NesEmulator.UnitTests
{
    public class MemoryConstantsTests
    {
        [Fact]
        public void ApuFrameCounter_ShouldBe_0x4017()
        {
            // This is correct, it shares the address with Pad.2
            MemoryMap.ApuFrameCounter.Should().Be(0x4017);
        }

        [Fact]
        public void ApuRegisters_ShouldStartAt_4000()
        {
            MemoryMap.ApuRegisters.Should().Be(0x4000);
        }


        [Fact]
        public void DmcFreq_ShouldBe_0x4010()
        {
            MemoryMap.DmcFreq.Should().Be(0x4010);
        }

        [Fact]
        public void DmcLength_ShouldBe_0x4013()
        {
            MemoryMap.DmcLength.Should().Be(0x4013);
        }

        [Fact]
        public void DmcRaw_ShouldBe_0x4011()
        {
            MemoryMap.DmcRaw.Should().Be(0x4011);
        }

        [Fact]
        public void DmcStart_ShouldBe_0x4012()
        {
            MemoryMap.DmcStart.Should().Be(0x4012);
        }

        [Fact]
        public void ExpansionRom_ShouldStartAt_4020()
        {
            MemoryMap.ExpansionRom.Should().Be(0x4020);
        }

        [Fact]
        public void InterruptRequestVector_ShouldStartAt_FFFE()
        {
            MemoryMap.InterruptRequestVector.Should().Be(0xFFFE);
        }

        [Fact]
        public void JoyPad1_ShouldBe_0x4016()
        {
            MemoryMap.JoyPad1.Should().Be(0x4016);
        }

        [Fact]
        public void JoyPad2_ShouldBe_0x4017()
        {
            MemoryMap.JoyPad2.Should().Be(0x4017);
        }

        [Fact]
        public void NoiseHighByte_ShouldBe_0x400F()
        {
            MemoryMap.NoiseHighByte.Should().Be(0x400F);
        }

        [Fact]
        public void NoiseLowByte_ShouldBe_0x400E()
        {
            MemoryMap.NoiseLowByte.Should().Be(0x400E);
        }

        [Fact]
        public void NoiseVolume_ShouldBe_0x400C()
        {
            MemoryMap.NoiseVolume.Should().Be(0x400C);
        }

        [Fact]
        public void NonMaskableInterruptVector_ShouldStartAt_FFFA()
        {
            MemoryMap.NonMaskableInterruptVector.Should().Be(0xFFFA);
        }

        [Fact]
        public void OamAddr_ShouldBe_0x2003()
        {
            MemoryMap.OamAddr.Should().Be(0x2003);
        }

        [Fact]
        public void OamData_ShouldBe_0x2004()
        {
            MemoryMap.OamData.Should().Be(0x2004);
        }

        [Fact]
        public void OamDma_ShouldBe_0x4014()
        {
            MemoryMap.OamDma.Should().Be(0x4014);
        }

        [Fact]
        public void PpuAddr_ShouldBe_0x2006()
        {
            MemoryMap.PpuAddr.Should().Be(0x2006);
        }

        [Fact]
        public void PpuControl_ShouldBe_0x2000()
        {
            MemoryMap.PpuControl.Should().Be(0x2000);
        }

        [Fact]
        public void PpuData_ShouldBe_0x2007()
        {
            MemoryMap.PpuData.Should().Be(0x2007);
        }

        [Fact]
        public void PpuMask_ShouldBe_0x2001()
        {
            MemoryMap.PpuMask.Should().Be(0x2001);
        }

        [Fact]
        public void PpuRegisterMirror_ShouldStartAt_2008()
        {
            MemoryMap.PpuRegisterMirror.Should().Be(0x2008);
        }

        [Fact]
        public void PpuRegisters_ShouldStartAt_2000()
        {
            MemoryMap.PpuRegisters.Should().Be(0x2000);
        }

        [Fact]
        public void PpuScroll_ShouldBe_0x2005()
        {
            MemoryMap.PpuScroll.Should().Be(0x2005);
        }

        [Fact]
        public void PpuStatus_ShouldBe_0x2002()
        {
            MemoryMap.PpuStatus.Should().Be(0x2002);
        }

        [Fact]
        public void PrgRomLowerBank_ShouldStartAt_8000()
        {
            MemoryMap.PrgRomLowerBank.Should().Be(0x8000);
        }

        [Fact]
        public void PrgRomUpperBank_ShouldStartAt_C000()
        {
            MemoryMap.PrgRomUpperBank.Should().Be(0xC000);
        }

        [Fact]
        public void Ram_ShouldStartAt_0200()
        {
            MemoryMap.Ram.Should().Be(0x0200);
        }

        [Fact]
        public void RamMirror_ShouldStartAt_0800()
        {
            MemoryMap.RamMirror.Should().Be(0x0800);
        }

        [Fact]
        public void ResetVector_ShouldStartAt_FFFC()
        {
            MemoryMap.ResetVector.Should().Be(0xFFFC);
        }

        [Fact]
        public void SoundChannelStatus_ShouldBe_0x4015()
        {
            MemoryMap.ApuSoundChannelStatus.Should().Be(0x4015);
        }

        [Fact]
        public void SquareWave1PeriodHighByte_ShouldBe_0x4003()
        {
            MemoryMap.SquareWave1PeriodHighByte.Should().Be(0x4003);
        }

        [Fact]
        public void SquareWave1PeriodLowByte_ShouldBe_0x4002()
        {
            MemoryMap.SquareWave1PeriodLowByte.Should().Be(0x4002);
        }

        [Fact]
        public void SquareWave1Sweep_ShouldBe_0x4001()
        {
            MemoryMap.SquareWave1Sweep.Should().Be(0x4001);
        }


        [Fact]
        public void SquareWave1Volume_ShouldBe_0x4000()
        {
            MemoryMap.SquareWave1Volume.Should().Be(0x4000);
        }

        [Fact]
        public void SquareWave2PeriodHighByte_ShouldBe_0x4007()
        {
            MemoryMap.SquareWave2PeriodHighByte.Should().Be(0x4007);
        }

        [Fact]
        public void SquareWave2PeriodLowByte_ShouldBe_0x4006()
        {
            MemoryMap.SquareWave2PeriodLowByte.Should().Be(0x4006);
        }

        [Fact]
        public void SquareWave2Sweep_ShouldBe_0x4005()
        {
            MemoryMap.SquareWave2Sweep.Should().Be(0x4005);
        }


        [Fact]
        public void SquareWave2Volume_ShouldBe_0x4004()
        {
            MemoryMap.SquareWave2Volume.Should().Be(0x4004);
        }

        [Fact]
        public void SRam_ShouldStartAt_6000()
        {
            MemoryMap.SRam.Should().Be(0x6000);
        }

        [Fact]
        public void Stack_ShouldStartAt_0100()
        {
            MemoryMap.Stack.Should().Be(0x0100);
        }

        [Fact]
        public void TriangleWaveLinearCounter_ShouldBe_0x4008()
        {
            MemoryMap.TriangleWaveLinearCounter.Should().Be(0x4008);
        }

        [Fact]
        public void TriangleWavePeriodHighByte_ShouldBe_0x400B()
        {
            MemoryMap.TriangleWavePeriodHighByte.Should().Be(0x400B);
        }

        [Fact]
        public void TriangleWavePeriodLowByte_ShouldBe_0x400A()
        {
            MemoryMap.TriangleWavePeriodLowByte.Should().Be(0x400A);
        }

        [Fact]
        public void Unused1_ShouldBe_0x4009()
        {
            MemoryMap.Unused1.Should().Be(0x4009);
        }

        [Fact]
        public void Unused2_ShouldBe_0x400D()
        {
            MemoryMap.Unused2.Should().Be(0x400D);
        }

        [Fact]
        public void ZeroPage_ShouldStartAt_0000()
        {
            MemoryMap.ZeroPage.Should().Be(0x0000);
        }
    }
}