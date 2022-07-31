
using System;
using System.Text;

namespace NesEmulator
{
    public abstract class Frame : IDisposable
    {
        private readonly Color[] _pixels;
        private bool _disposedValue = false;

        abstract public int Width { get; }
        abstract public int Height { get; }
        public int Number { get; protected set; }

        public int TotalPixels => Width * Height;

        protected Frame()
        {
            _pixels = System.Buffers.ArrayPool<Color>.Shared.Rent(TotalPixels);
        }

        public Color GetPixel(int x, int y)
        {
            return _pixels[IndexFromPosition(x, y)];
        }

        internal void SetPixel(int x, int y, Color c)
        {
            _pixels[IndexFromPosition(x, y)] = c;
        }

        private int IndexFromPosition(int x, int y)
        {
            return (y * Width) + x;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    System.Buffers.ArrayPool<Color>.Shared.Return(_pixels);
                }
                else
                {
                    System.Buffers.ArrayPool<Color>.Shared.Return(_pixels);
#if DEBUG
                    throw new ApplicationException(
                        "Frame got finalised! This should not happen, dispose should be called.");
#endif
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}