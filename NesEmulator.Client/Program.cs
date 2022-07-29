using System;
using NesEmulator.NesEmulator;
using Raylib_cs;
using Color = Raylib_cs.Color;

namespace NesEmulator.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1600, 1200, "NES");
            Raylib.SetTargetFPS(60);
            
            var emulator = new TestEmulator();

            while (!Raylib.WindowShouldClose())
            {
                emulator.StubProduceFrame();
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);

                Raylib.DrawText($"Hello C# Window! FPS: {Raylib.GetFPS()}", 10, 10, 20, Color.RED);
                DrawFrame(10, 50,  5, emulator.CurrentFrame);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }

        private static void DrawFrame(int startX, int startY, int scale, Frame frame)
        {
            for (int y = 0; y < frame.Height; y++)
            for (int x = 0; x < frame.Width; x++)
            {
                var pixelColor = frame.GetPixel(x, y);

                Raylib.DrawRectangle(
                    startX + (x * scale), 
                    startY + (y * scale), 
                    scale, 
                    scale, 
                    pixelColor.ToRaylib());
            }
        }
    }

    public static class MappingExtensions
    {
        public static Raylib_cs.Color ToRaylib(this NesEmulator.Color c)
        {
            return new Raylib_cs.Color(c.Red, c.Green, c.Blue, 255);
        }
    }
}