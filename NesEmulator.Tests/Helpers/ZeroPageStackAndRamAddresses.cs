using System.Collections;
using System.Collections.Generic;

namespace NesEmulator.UnitTests.Helpers
{
    public class ZeroPageStackAndRamAddresses : IEnumerable<object[]>
    {
        private const int Step = 0x100;
        
        public IEnumerator<object[]> GetEnumerator()
        {
            const ushort start = 0x0000;
            const ushort end = 0x1FFF;
            
            yield return new object[]{ start };
            
            for (int addr = start; addr < end; addr += Step)
            {
                yield return new object[]{ addr };
            }
            
            yield return new object[]{ end };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}