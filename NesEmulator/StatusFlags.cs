using System;

namespace NesEmulator
{
    [Flags]
    enum StatusFlags
    {
        None = 0b0000_0000,

        Carry = 0b0000_0001,
        Zero = 0b000_0010,
        InterruptDisable = 0b000_0100,
        Decimal = 0b0000_1000,

        Bit4 = 0b0001_0000,
        Bit5 = 0b0010_0000,
        Overflow = 0b0100_0000,
        Negative = 0b1000_0000,

        All = 0b1111_1111
    }
}
