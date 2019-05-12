namespace NesEmulator.PPU
{
    public interface IDisplayOutput
    {
        IFrameBuffer BeginFrame();
        
        void Draw(IFrameBuffer frameBuffer);
    }
}