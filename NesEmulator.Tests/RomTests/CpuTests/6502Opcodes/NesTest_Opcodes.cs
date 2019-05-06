using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using NesEmulator.APU;
using NesEmulator.Extensions;
using NesEmulator.Input;
using NesEmulator.Memory;
using NesEmulator.PPU;
using NesEmulator.Processor;
using NesEmulator.RomMappers;
using Xunit;

namespace NesEmulator.UnitTests.RomTests.CpuTests._6502Opcodes
{
    public class LogRow
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

        public LogRow(string rowText)
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
            FrameNumber = int.Parse(span.Slice(82, 3).ToString());
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
        public int FrameNumber { get; }
        public long CpuCycles { get; }
    }
    
    public class NesTest_Opcodes
    {
        private const string RomFile = "RomTests/CpuTests/6502Opcodes/nestest.nes";
        private const string ExpectedBehaviourLogFile = "RomTests/CpuTests/6502Opcodes/nestest.log.txt";
        
        private const ushort AutomatedTestStartPrgAddress = 0xC000;
        private const ushort ErrorCodeAddress1 = 0x0002;
        private const ushort ErrorCodeAddress2 = 0x0003;

        [Fact]
        public void VerifyLogParsing()
        {
            const string rowText =
                "F94E  30 09     BMI $F959                       A:01 X:78 Y:66 P:25 SP:F9 PPU: 44,119 CYC:13548";
            
            var row = new LogRow(rowText);

            row.InstructionPointer.Should().Be(0xF94E);
            row.OpcodeValue.Should().Be(0x30);
            row.Operand1.Should().Be(0x09);
            row.Operand2.Should().BeNull();
            row.Mnemonic.Should().Be("BMI");
            row.Accumulator.Should().Be(0x01);
            row.X.Should().Be(0x78);
            row.Y.Should().Be(0x66);
            row.StatusFlags.Should().Be(0x25);
            row.StackPointer.Should().Be(0xF9);
            row.PpuCycles.Should().Be(44);
            row.FrameNumber.Should().Be(119);
            row.CpuCycles.Should().Be(13548);
        }
        
        [Fact]
        public void RunNesTestRomAutomated()
        {
            ROM rom;
            using (var file = File.OpenRead(RomFile))
            {
                rom = ROM.Create(file);
            }

            var log = File.ReadLines(ExpectedBehaviourLogFile)
                .Select(l => new LogRow(l))
                .ToList();

            int currentFrame = 0;

            var memory = new MainMemory(new NullPpu(), new NullApu(), new NullInputSource(), new NullInputSource());
            var cpu = new CPU(memory);
            memory.Load(rom);

            cpu.Power();
            cpu.Step();
            cpu.ForceInstructionPointer(AutomatedTestStartPrgAddress);

            bool undocumentedOpcode = false;
            for (int i = 0; i < log.Count; i++)
            {
                LogRow expectedState = log[i];
                currentFrame = expectedState.FrameNumber;
                
                string failureMessage = 
                    $"\n\n" +
                    $"CPU diverged from expected behaviour after {i+1} steps (log row {i}).\n" +
                    $"State was: IP:{cpu.InstructionPointer:X4} A:{cpu.Accumulator:X2} X:{cpu.IndexX:X2} Y:{cpu.IndexY:X2} P:{cpu.Status.AsByte():X2} SP:{cpu.StackPointer.LowByte():X2} CYC:{cpu.ElapsedCycles}\n" +
                    $"Expected:  IP:{expectedState.InstructionPointer:X2} A:{expectedState.Accumulator:X2} X:{expectedState.X:X2} Y:{expectedState.Y:X2} P:{expectedState.StatusFlags:X2} SP:{expectedState.StackPointer:X2} CYC:{expectedState.CpuCycles}\n" +
                    $"Log raw: {expectedState.Text}\n" +
                    $"Undocumented opcode: {undocumentedOpcode}" +
                    $"\n\n";
                
                cpu.InstructionPointer.Should().Be(expectedState.InstructionPointer, failureMessage);
                cpu.Accumulator.Should().Be(expectedState.Accumulator, failureMessage);
                cpu.IndexX.Should().Be(expectedState.X, failureMessage);
                cpu.IndexY.Should().Be(expectedState.Y, failureMessage);
                cpu.Status.AsByte().Should().Be(expectedState.StatusFlags, failureMessage);
                cpu.StackPointer.LowByte().Should().Be(expectedState.StackPointer, failureMessage);

                try
                {
                    cpu.Step();
                }
                catch (KeyNotFoundException e)
                {
                    // Ignore, let test fail
                    undocumentedOpcode = true;
                }

                //// Simulate NMI if the next log row is a new frame
                //if (i + 1 < log.Count && log[i + 1].FrameNumber != currentFrame)
                //{
                //    cpu.Interrupt(InterruptType.Nmi);
                //}
            }

            // Rom writes error codes to ZP addresses 0x02 and 0x03
            memory.Peek(ErrorCodeAddress1)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress1));

            memory.Peek(ErrorCodeAddress2)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress2));
        }

        private string LookupFailureCode(ushort errorCodeAddress)
        {
            return $"Todo: lookup failure code in address {errorCodeAddress}";
        }
    }
}