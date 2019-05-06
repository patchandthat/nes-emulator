using NesEmulator.Processor;

namespace NesEmulator.Extensions
{
    public static class StatusFlagsExtensions
    {
        public static byte AsByte(this StatusFlags flags)
        {
            return (byte) flags;
        }

        public static StatusFlags AsStatusFlags(this byte flags)
        {
            return (StatusFlags) flags;
        }
    }
}