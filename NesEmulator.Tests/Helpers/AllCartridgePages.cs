using System.Collections;
using System.Collections.Generic;

namespace NesEmulator.UnitTests.Helpers
{
    public class AllCartridgePages : IEnumerable<object[]>
    {
        private const int Step = 0x100;
        
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]{ 0x4020 };
            
            for (int addr = 0x8001; addr < ushort.MaxValue; addr += Step)
            {
                yield return new object[]{ addr };
            }
            
            yield return new object[]{ 0xFFFF };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}