using System.Security.Cryptography.X509Certificates;

namespace NesEmulator
{
    using System;
    using System.Collections.Generic;

    namespace NesEmulator
    {
        public class TestEmulator
        {
            NesPalette _palette = new NesPalette();
            int _frame = 0;

            public event EventHandler<FrameEventArgs> OnFrameReady = delegate { };

            public Frame CurrentFrame { get; private set; }
            
            public void StubProduceFrame()
            {
                Frame f = new NtscFrame(_frame);
                int pixel = _frame;
                for (int x = 0; x < f.Width; x++)
                for (int y = 0; y < f.Height; y++)
                {
                    Color c = _palette.Colors[pixel++ % 0x3F];
                    f.SetPixel(x, y, c);
                }

                using (Frame oldFrame = CurrentFrame)
                {
                    CurrentFrame = f;
                
                    OnFrameReady(this, new FrameEventArgs(f));
                }
                
                _frame++;
            }
        }

        public class FrameEventArgs
        {
            public Frame Frame { get; private set; }

            public FrameEventArgs(Frame frame)
            {
                Frame = frame ?? throw new ArgumentNullException(nameof(frame));
            }
        }
    }

}