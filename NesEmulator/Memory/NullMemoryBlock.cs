using System;
using System.Collections.Generic;
using System.Text;

namespace NesEmulator.Memory
{
    class NullMemoryBlock : IReadWrite
    {
        public byte Read(ushort address)
        {
            return 0x0;
        }

        public byte Peek(ushort address)
        {
            return 0x0;
        }

        public void Write(ushort address, byte value)
        {
            
        }
    }
}
