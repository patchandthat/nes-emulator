using System.Collections;
using System.Collections.Generic;

namespace NesEmulator.UnitTests.Helpers
{
    public class AllByteValues : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            for (int b = 0; b < 256; b++)
            {
                yield return new object[]{ (byte)b };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class AllBytePairs : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            for (int b = 0; b < 256; b++)
            for (int c = 0; c < 256; c++)
            {
                yield return new object[]{ (byte)b, (byte)c };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}