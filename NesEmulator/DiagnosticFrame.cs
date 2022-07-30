namespace NesEmulator.NesEmulator
{
    public class DiagnosticFrame : Frame
    {
        public DiagnosticFrame(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override int Width { get; }
        public override int Height { get; }
    }
}