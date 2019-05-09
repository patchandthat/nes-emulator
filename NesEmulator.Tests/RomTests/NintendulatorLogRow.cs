using System;
using System.Globalization;

namespace NesEmulator.UnitTests.RomTests
{
    public class NintendulatorLogRow
    {
        /*
         * Log format
         * Column (1-indexed)     Field                        Prefix
         * --------------------------------------------------------------
         * 1-4                    Instruction ptr
         * 7-8                    Opcode
         * 10-11                  Operand byte 1 (optional)
         * 13-14                  Operand byte 2 (optional)
         * 17-19                  Opcode Mnemonic
         * 49-52                  Accumulator value            A:
         * 54-57                  X value                      X:
         * 59-62                  Y value                      Y:
         * 64-67                  Status (as byte)             P:
         * 69-73                  Stack pointer                SP:
         * 75-81                  Ppu cycles                   PPU:
         * 83-85                  Frame number
         * 87-EOL                 Cpu cycles (cumulative)
         */

        public NintendulatorLogRow(string rowText)
        {
            Text = rowText;
            var span = Text.AsSpan();
            InstructionPointer = ushort.Parse(span.Slice(0, 4).ToString(), NumberStyles.HexNumber);
            OpcodeValue = byte.Parse(span.Slice(6, 2).ToString(), NumberStyles.HexNumber);
            Operand1 = byte.TryParse(span.Slice(9, 2).ToString(), NumberStyles.HexNumber, null, out byte op1)
                ? (byte?) op1
                : null;
            Operand2 = byte.TryParse(span.Slice(12, 2).ToString(), NumberStyles.HexNumber, null, out byte op2)
                ? (byte?) op2
                : null;
            Mnemonic = span.Slice(16, 3).ToString();
            Accumulator = byte.Parse(span.Slice(50, 2).ToString(), NumberStyles.HexNumber);
            X = byte.Parse(span.Slice(55, 2).ToString(), NumberStyles.HexNumber);
            Y = byte.Parse(span.Slice(60, 2).ToString(), NumberStyles.HexNumber);
            StatusFlags = byte.Parse(span.Slice(65, 2).ToString(), NumberStyles.HexNumber);
            StackPointer = byte.Parse(span.Slice(71, 2).ToString(), NumberStyles.HexNumber);
            PpuCycles = int.Parse(span.Slice(78, 3).ToString());
            Scanline = int.Parse(span.Slice(82, 3).ToString());
            CpuCycles = long.Parse(span.Slice(90 /* to EOL */).ToString());
        }

        public string Text { get; }
        public ushort InstructionPointer { get; }
        public byte OpcodeValue { get; }
        public byte? Operand1 { get; }
        public byte? Operand2 { get; }
        public string Mnemonic { get; }
        public byte Accumulator { get; }
        public byte X { get; }
        public byte Y { get; }
        public byte StatusFlags { get; }
        public byte StackPointer { get; }
        public int PpuCycles { get; }
        public int Scanline { get; }
        public long CpuCycles { get; }
    }
}