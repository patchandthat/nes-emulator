using System.Collections;
using System.Collections.Generic;

namespace NesEmulator.UnitTests.Helpers
{
    public class ManyByteValues : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            for (int b = 0; b < 256; b += 16)
            {
                yield return new object[]{ (byte)b };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}