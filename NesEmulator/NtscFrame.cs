namespace NesEmulator.NesEmulator
{
    public sealed class NtscFrame : Frame
    {
        public NtscFrame(int number)
        {
            Number = number;
        }

        public override int Width => 256;

        public override int Height => 224;
    }
}