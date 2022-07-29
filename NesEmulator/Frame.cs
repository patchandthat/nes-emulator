﻿namespace NesEmulator
{
    using System;
    using System.Collections.Generic;
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

        public sealed class PalFrame : Frame
        {
            public PalFrame(int number)
            {
                Number = number;
            }

            public override int Width => 256;

            public override int Height => 240;
        }

        public sealed class NtscFrame : Frame
        {
            public NtscFrame(int number)
            {
                Number = number;
            }

            public override int Width => 256;

            public override int Height => 224;
        }

        public struct Color : IEquatable<Color>
        {
            public Color(int red, int green, int blue)
            {
                Red = red;
                Green = green;
                Blue = blue;
            }

            public int Red { get; set; }
            public int Green { get; set; }
            public int Blue { get; set; }

            public override bool Equals(object obj)
            {
                return obj is Color color && Equals(color);
            }

            public bool Equals(Color other)
            {
                return Red == other.Red &&
                       Green == other.Green &&
                       Blue == other.Blue;
            }

            public override int GetHashCode()
            {
                int hashCode = -1058441243;
                hashCode = hashCode * -1521134295 + Red.GetHashCode();
                hashCode = hashCode * -1521134295 + Green.GetHashCode();
                hashCode = hashCode * -1521134295 + Blue.GetHashCode();
                return hashCode;
            }
        }

        public class NesPalette
        {
            public IReadOnlyDictionary<int, Color> Colors { get; set; } = new Dictionary<int, Color>()
            {
                { 0x00, new Color(0x75, 0x75, 0x75) },
                { 0x01, new Color(0x27, 0x1B, 0x8F) },
                { 0x02, new Color(0x00, 0x00, 0xAB) },
                { 0x03, new Color(0x47, 0x00, 0x9F) },
                { 0x04, new Color(0x8F, 0x00, 0x77) },
                { 0x05, new Color(0xAB, 0x00, 0x13) },
                { 0x06, new Color(0xA7, 0x00, 0x00) },
                { 0x07, new Color(0x7F, 0x0B, 0x00) },
                { 0x08, new Color(0x43, 0x2F, 0x00) },
                { 0x09, new Color(0x00, 0x47, 0x00) },
                { 0x0A, new Color(0x00, 0x51, 0x00) },
                { 0x0B, new Color(0x00, 0x3F, 0x17) },
                { 0x0C, new Color(0x1B, 0x3F, 0x5F) },
                { 0x0D, new Color(0x00, 0x00, 0x00) },
                { 0x0E, new Color(0x00, 0x00, 0x00) },
                { 0x0F, new Color(0x00, 0x00, 0x00) },
                { 0x10, new Color(0xBC, 0xBC, 0xBC) },

                { 0x11, new Color(0x00, 0x73, 0xEF) },
                { 0x12, new Color(0x23, 0x3B, 0xEF) },
                { 0x13, new Color(0x83, 0x00, 0xF3) },
                { 0x14, new Color(0xBF, 0x00, 0xBF) },
                { 0x15, new Color(0xE7, 0x00, 0x5B) },
                { 0x16, new Color(0xDB, 0x2B, 0x00) },
                { 0x17, new Color(0xCB, 0x4F, 0x0F) },
                { 0x18, new Color(0x8B, 0x73, 0x00) },
                { 0x19, new Color(0x00, 0x97, 0x00) },
                { 0x1A, new Color(0x00, 0xAB, 0x00) },
                { 0x1B, new Color(0x00, 0x93, 0x3B) },
                { 0x1C, new Color(0x00, 0x83, 0x8B) },
                { 0x1D, new Color(0x00, 0x00, 0x00) },
                { 0x1E, new Color(0x00, 0x00, 0x00) },
                { 0x1F, new Color(0x00, 0x00, 0x00) },

                { 0x20, new Color(0xFF, 0xFF, 0xFF) },
                { 0x21, new Color(0x3F, 0xBF, 0xFF) },
                { 0x22, new Color(0x5F, 0x97, 0xFF) },
                { 0x23, new Color(0xA7, 0x8B, 0xFD) },
                { 0x24, new Color(0xF7, 0x7B, 0xFF) },
                { 0x25, new Color(0xFF, 0x77, 0xB7) },
                { 0x26, new Color(0xFF, 0x77, 0x63) },
                { 0x27, new Color(0xFF, 0x9B, 0x3B) },
                { 0x28, new Color(0xF3, 0xBF, 0x3F) },
                { 0x29, new Color(0x83, 0xD3, 0x13) },
                { 0x2A, new Color(0x4F, 0xDF, 0x4B) },
                { 0x2B, new Color(0x58, 0xF8, 0x98) },
                { 0x2C, new Color(0x00, 0xEB, 0xDB) },
                { 0x2D, new Color(0x00, 0x00, 0x00) },
                { 0x2E, new Color(0x00, 0x00, 0x00) },
                { 0x2F, new Color(0x00, 0x00, 0x00) },

                { 0x30, new Color(0xFF, 0xFF, 0xFF) },
                { 0x31, new Color(0xAB, 0xE7, 0xFF) },
                { 0x32, new Color(0xC7, 0xD7, 0xFF) },
                { 0x33, new Color(0xD7, 0xCB, 0xFF) },
                { 0x34, new Color(0xFF, 0xC7, 0xFF) },
                { 0x35, new Color(0xFF, 0xC7, 0xDB) },
                { 0x36, new Color(0xFF, 0xBF, 0xB3) },
                { 0x37, new Color(0xFF, 0xDB, 0xAB) },
                { 0x38, new Color(0xFF, 0xE7, 0xA3) },
                { 0x39, new Color(0xE3, 0xFF, 0xA3) },
                { 0x3A, new Color(0xAB, 0xF3, 0xBF) },
                { 0x3B, new Color(0xB3, 0xFF, 0xCF) },
                { 0x3C, new Color(0x9F, 0xFF, 0xF3) },
                { 0x3D, new Color(0x00, 0x00, 0x00) },
                { 0x3E, new Color(0x00, 0x00, 0x00) },
                { 0x3F, new Color(0x00, 0x00, 0x00) }
            };
        }
    }
}