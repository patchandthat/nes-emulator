using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Skia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using NesEmulator.PPU;
using SkiaSharp;

namespace NesEmulator.Desktop
{
    public class RenderSurface : UserControl, IDisplayOutput
    {
        private class SolidColorFrame : IFrameBuffer, IDisposable
        {
            private readonly SKColor _color;

            public SolidColorFrame(PPU.Color color)
            {
                this._color = new SKColor(color.Red, color.Green, color.Blue);

                SKImageInfo info = new SKImageInfo(256, 256, SKColorType.Rgba8888, SKAlphaType.Opaque);
                Image = new SKBitmap(info);

                for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                {
                    Image.SetPixel(i, j, _color);
                }
            }

            public SKBitmap Image { get; }

            public void SetPixel(byte x, byte y, PPU.Color color)
            {
                // No op
            }

            public void Dispose()
            {
                Image?.Dispose();
            }
        }

        public RenderSurface()
        {
            _frames = new Queue<IFrameBuffer>();

            _frames.Enqueue(new SolidColorFrame(new PPU.Color(0xff, 0x00, 0x00)));
            _frames.Enqueue(new SolidColorFrame(new PPU.Color(0x00, 0xff, 0x00)));
            _frames.Enqueue(new SolidColorFrame(new PPU.Color(0x00, 0x00, 0xff)));
        }

        //Our render target we compile everything to and present to the user
        private RenderTargetBitmap _renderTarget;
        private ISkiaDrawingContextImpl _skiaContext;

        public override void EndInit()
        {
            _drawRect = new SKRect(0, 0, 255, 255);

            SKPaint SKBrush = new SKPaint();
            SKBrush.IsAntialias = false;
            SKBrush.Color = new SKColor(0, 0, 0);
            SKBrush.Shader = SKShader.CreateColor(SKBrush.Color);
            _renderTarget = new RenderTargetBitmap(new PixelSize(256,256), new Vector(96, 96));

            var context = _renderTarget.CreateDrawingContext(null);
            _skiaContext = (context as ISkiaDrawingContextImpl);
            _skiaContext.SkCanvas.Clear(new SKColor(255, 255, 255));

            base.EndInit();
        }

        public override void Render(DrawingContext context)
        {
            context.DrawImage(_renderTarget,
                1.0,
                new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
                new Rect(0, 0, Width, Height)
            );
        }

        private readonly Queue<IFrameBuffer> _frames;
        private SolidColorFrame _activeFrame;
        private SKRect _drawRect;

        public IFrameBuffer BeginFrame()
        {
            return _frames.Dequeue();
        }

        public void Draw(IFrameBuffer frameBuffer)
        {
            _activeFrame = (SolidColorFrame) frameBuffer;

            
            _skiaContext.SkCanvas.DrawBitmap(_activeFrame.Image, _drawRect);

            _frames.Enqueue(_activeFrame);

            InvalidateVisual();
        }
    }
}