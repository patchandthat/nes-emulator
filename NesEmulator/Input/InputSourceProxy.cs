using System;

namespace NesEmulator.Input
{
    class InputSourceProxy : IInputSource
    {
        // Todo: Delegate to implementer, allowing implementation to be reconfigured at runtime

        public byte Read(ushort address)
        {
            throw new NotImplementedException();
        }

        public byte Peek(ushort address)
        {
            throw new NotImplementedException();
        }

        public void Write(ushort address, byte value)
        {
            throw new NotImplementedException();
        }
    }
}
