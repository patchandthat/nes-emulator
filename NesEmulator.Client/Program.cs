using System;
using Raylib_cs;

namespace NesEmulator.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(800, 600, "NES");

            while (!Raylib.WindowShouldClose())
            {
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.RAYWHITE);

                Raylib.DrawText("Hello C# Window", 10, 10, 20, Color.RED);

                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}