using System;
using NesEmulator;
using Raylib_cs;

namespace NesEmulator.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1600, 1200, "NES");
            Raylib.SetTargetFPS(60);
            
            var emulator = new Nes();
            emulator.InsertCartridge("nestest.nes");
            emulator.Power();

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyDown(KeyboardKey.KEY_C))
                {
                    emulator.Step();
                }
                else if (Raylib.IsKeyDown(KeyboardKey.KEY_F))
                {
                    emulator.StepToNextFrame();
                }
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib_cs.Color.RAYWHITE);

                const int scale = 3;
                DrawFrame(0, 0,  scale, emulator.Screen);
                DrawDisassemblyInfo(scale, emulator);
                DrawNameTables(scale, emulator);
                DrawPatternTables(scale, emulator);

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
        
        private static void DrawDisassemblyInfo(int scale, Nes emulator)
        {
            Raylib.DrawText($"Hello C# Window! FPS: {Raylib.GetFPS()}", 10 + (256 * scale), 10, 20, Raylib_cs.Color.RED);
        }
        
        private static void DrawNameTables(int scale, Nes emulator)
        {
            
        }
        
        private static void DrawPatternTables(int scale, Nes emulator)
        {
            
        }
    }

    public static class MappingExtensions
    {
        public static Raylib_cs.Color ToRaylib(this Color c)
        {
            return new Raylib_cs.Color(c.Red, c.Green, c.Blue, 255);
        }
    }
}