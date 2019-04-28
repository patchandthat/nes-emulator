using System;
using System.Diagnostics.Contracts;

namespace NesEmulator.Memory
{
    public readonly struct MemoryRange
    {
        public MemoryRange(ushort start, ushort end)
        {
            if (start > end) throw new ArgumentException("startAddress must be less than or equal to endAddress");

            Start = start;
            End = end;
            Length = 1 + end - start;
        }

        public ushort Start { get; }
        public ushort End { get; }
        public int Length { get; }

        [Pure]
        public bool Intersects(MemoryRange other)
        {
            return (other.Start >= Start && other.Start <= End) ||
                   (other.End >= Start && other.End <= End);
        }

        [Pure]
        public bool Contains(MemoryRange other)
        {
            return Start <= other.Start && End >= other.End;
        }

        [Pure]
        public bool Contains(ushort address)
        {
            return Start <= address && End >= address;
        }

        public override string ToString()
        {
            return $"{Start}({Start:X4}):{End}({End:X4})";
        }
    }
}