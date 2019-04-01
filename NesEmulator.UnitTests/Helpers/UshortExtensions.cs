namespace NesEmulator.UnitTests.Helpers
{
    public static class UshortExtensions
    {
        public static ushort Plus(this ushort start, int difference)
        {
            return (ushort) (start + difference);
        }
    }
}