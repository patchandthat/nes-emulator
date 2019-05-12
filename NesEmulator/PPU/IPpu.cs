using NesEmulator.Processor;

namespace NesEmulator.PPU
{
    internal interface IPpu : IReadWrite
    {
        void Load(IReadWriteChr rom);
    }

    internal class PPU : IPpu
    {
        private readonly IDisplayOutput _displayOut;
        private IReadWriteChr _rom;
        private IFrameBuffer _currentFrame;

        public PPU(IDisplayOutput displayOut)
        {
            _displayOut = displayOut;
        }

        public byte Read(ushort address)
        {
            return 0;
        }

        public byte Peek(ushort address)
        {
            return 0;
        }

        public void Write(ushort address, byte value)
        {
            
        }

        // Something like this
        public void Step(int ticksDelta)
        {
            if (_currentFrame == null)
                _currentFrame = _displayOut.BeginFrame();

            // Draw enough shit to catch up with the CPU then return
            // Draw the rest of the fucking owl

            // If frame is complete
            {
                _displayOut.Draw(_currentFrame);
                _currentFrame = null;
                CPU cpu = null;
                cpu.Interrupt(InterruptType.Nmi);
            }
        }

        public void Load(IReadWriteChr rom)
        {
            _rom = rom;
        }
    }
}