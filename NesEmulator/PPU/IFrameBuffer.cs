namespace NesEmulator.PPU
{
    public interface IFrameBuffer
    {
        void SetPixel(byte x, byte y, Color color);
    }
}