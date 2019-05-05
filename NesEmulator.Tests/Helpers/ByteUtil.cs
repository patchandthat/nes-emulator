using System;

namespace NesEmulator.UnitTests.Helpers
{
    public static class ByteUtil
    {
        private static Random _rng;
        
        public static byte RandomByte()
        {
            if (_rng == null) _rng = new Random();

            return (byte) (_rng.Next() & 0xFF);
        }
    }
}