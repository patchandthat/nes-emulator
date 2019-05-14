using Xunit;

namespace NesEmulator.UnitTests.RomTests.CpuTests.Interrupts
{
    
    [Trait("Category", "Integration")]
    public class InterruptTests
    {
        [Fact(Skip = "Todo")]
        public void CliLatency()
        {
            Assert.True(false, "Todo: Execute 1-cli_latency.nes and compare against Nintendulator log");
        }

        [Fact(Skip = "Todo")]
        public void NmiAndBrk()
        {
            Assert.True(false, "Todo: Execute 2-nmi_and_brk.nes and compare against Nintendulator log");
        }

        [Fact(Skip = "Todo")]
        public void NmiAndIrq()
        {
            Assert.True(false, "Todo: Execute 3-nmi_and_irq.nes and compare against Nintendulator log");
        }

        [Fact(Skip = "Todo")]
        public void IrqAndDma()
        {
            Assert.True(false, "Todo: Execute 4-irq_and_dma.nes and compare against Nintendulator log");
        }

        [Fact(Skip = "Todo")]
        public void BranchDelays()
        {
            Assert.True(false, "Todo: Execute 5-branch_delays_irq.nes and compare against Nintendulator log");
        }
    }
}