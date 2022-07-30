namespace NesEmulator.NesEmulator
{
    public sealed class PalFrame : Frame
    {
        public PalFrame(int number)
        {
            Number = number;
        }

        public override int Width => 256;

        public override int Height => 240;
    }
}