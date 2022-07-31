using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Raylib_cs;
using RColor = Raylib_cs.Color;
using NesEmulator;

namespace NesEmulator.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Raylib.InitWindow(1600, 1200, "NES");
            Raylib.SetTargetFPS(60);
            Font debuggerFont = Raylib.LoadFont("Menlo-Regular.ttf");
            
            var emulator = new Nes();
            emulator.ShowDiagnostics = true;
            emulator.InsertCartridge("nestest.nes");
            emulator.Power();

            while (!Raylib.WindowShouldClose())
            {
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_F1))
                {
                    emulator.Step();
                }
                if (Raylib.IsKeyDown(KeyboardKey.KEY_F2))
                {
                    emulator.Step();
                }
                else if (Raylib.IsKeyPressed(KeyboardKey.KEY_F5))
                {
                    emulator.StepToNextFrame();
                }
                
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib_cs.Color.RAYWHITE);

                const int scale = 3;
                DrawFrame(0, 0,  scale, emulator.Screen);
                
                if (emulator.ShowDiagnostics)
                {
                    DrawDisassemblyInfo(debuggerFont, scale, emulator);
                    DrawNameTables(debuggerFont, scale, emulator);
                    DrawPatternTables(debuggerFont, scale, emulator);
                }

                Raylib.EndDrawing();
            }
            
            Raylib.UnloadFont(debuggerFont);
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
        
        private static void DrawDisassemblyInfo(Font font, int scale, Nes emulator)
        {
            Vector2 textStartPosition = new Vector2(10 + (256 * scale), 10);
            RColor textColor = RColor.DARKGRAY;
            const float fontSize = 32;
            const float fontSpacing = 0;
            const float lineIncrement = 36;

            // Todo: Consider String.Create from spans of a reusable char buffer to avoid allocations 
            List<string> lines = new List<string>();
            
            lines.Add($"Debug Info ({Raylib.GetFPS()} FPS)");
            lines.Add($"        NVUBDIZC");
            lines.Add($"Status: {emulator.DebugInfo.Status.ToBinaryString()}");
            lines.Add($"PC:     ${emulator.DebugInfo.InstructionPointer:X4}");
            lines.Add($"Acc:    ${emulator.DebugInfo.Accumulator:X2} ({emulator.DebugInfo.Accumulator})");
            lines.Add($"IdX:    ${emulator.DebugInfo.IndexX:X2} ({emulator.DebugInfo.IndexX})");
            lines.Add($"IdY:    ${emulator.DebugInfo.IndexY:X2} ({emulator.DebugInfo.IndexY})");
            lines.Add($"SP:     ${emulator.DebugInfo.StackPointer:X4}");
            lines.Add("");

            var disassembly = emulator.DebugInfo.Disassembly;
            try
            {
                int currentInstrIndex = disassembly.FindIndex(dr => dr.Address == emulator.DebugInfo.InstructionPointer);

                if (currentInstrIndex > 0)
                {
                    for (int i = currentInstrIndex - 10; i < currentInstrIndex + 10; i++)
                    {
                        if (i >= 0 && i < disassembly.Count)
                        {
                            var row = disassembly[i];
                            var sb = new StringBuilder();
                            sb.Append(i == currentInstrIndex ? "=>" : "  ");
                            sb.Append($"${row.Address.ToHexString()}: {row.Mnemonic} ");
                            for (int ob = 0; ob < row.OperandByteCount; ob++)
                            {
                                sb.Append($"${row.OperandBytes[ob].ToHexString()} ");
                            }
                            sb.Append($"({row.AddressMode})");
                        
                            lines.Add(sb.ToString());
                        }
                    }
                }
                else
                {
                    lines.Add("NO DISASSEMBLY AVAILABLE");
                }
            }
            catch (Exception e)
            {
                lines.Add("NO DISASSEMBLY AVAILABLE");
            }

            for (int i = 0; i < lines.Count; i++)
            {
                Raylib.DrawTextEx(
                    font, 
                    lines[i],
                    textStartPosition + new Vector2(0, i * lineIncrement),
                    fontSize,
                    fontSpacing, 
                    textColor);
            }
        }
        
        private static void DrawNameTables(Font font, int scale, Nes emulator)
        {
            
        }
        
        private static void DrawPatternTables(Font font, int scale, Nes emulator)
        {
            
        }
    }
}