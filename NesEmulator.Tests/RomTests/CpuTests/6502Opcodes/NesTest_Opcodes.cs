using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    [Trait("Category", "Integration")]
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
            
            var row = new NintendulatorLogRow(rowText);

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
            row.Scanline.Should().Be(119);
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
                .Select(l => new NintendulatorLogRow(l))
                .ToList();

            var memory = new MainMemory(new NullPpu(), new NullApu(), new NullInputSource(), new NullInputSource());
            var cpu = new CPU(memory);
            memory.Load(rom);

            cpu.Power();
            cpu.Step();
            cpu.ForceInstructionPointer(AutomatedTestStartPrgAddress);

            bool undocumentedOpcode = false;
            for (int i = 0; i < log.Count; i++)
            {
                NintendulatorLogRow expectedState = log[i];
                
                string failureMessage = 
                    $"\n\n" +
                    $"CPU diverged from expected behaviour after {i+1} steps (log row {i}).\n" +
                    $"State was: IP:{cpu.InstructionPointer:X4} A:{cpu.Accumulator:X2} X:{cpu.IndexX:X2} Y:{cpu.IndexY:X2} P:{cpu.Status.AsByte():X2} SP:{cpu.StackPointer.LowByte():X2} CYC:{cpu.ElapsedCycles}\n" +
                    $"Expected:  IP:{expectedState.InstructionPointer:X2} A:{expectedState.Accumulator:X2} X:{expectedState.X:X2} Y:{expectedState.Y:X2} P:{expectedState.StatusFlags:X2} SP:{expectedState.StackPointer:X2} CYC:{expectedState.CpuCycles}\n" +
                    $"\nLast run instructions...\n" +
                    LastNLogLines(log, i, 10) +
                    $"Log (current):  {expectedState.Text}\n" +
                    $"Log (next):     {( i+1 >= log.Count ? "N/A" : log[i+1].Text)}\n" +
                    $"Undocumented opcode encountered: {undocumentedOpcode}" +
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
                    // Ignore, let test fail on the next iteration, state will be incorrect
                    undocumentedOpcode = true;
                    
                    // 5000 instructions run before encountering undocumented opcodes
                    // Good enough for now
                    
                    break;
                }
            }

            // Rom writes error codes to ZP addresses 0x02 and 0x03
            memory.Peek(ErrorCodeAddress1)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress1));

            memory.Peek(ErrorCodeAddress2)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress2));
        }

        private string LastNLogLines(List<NintendulatorLogRow> log, int currentLogRow, int numLines)
        {
            var builder = new StringBuilder();

            for (int j = numLines; j > 1; j--)
            {
                builder.AppendLine(
                    $"Log (older):    {(currentLogRow - j < 0 ? "N/A" : (log[currentLogRow - j]).Text)}");
            }

            builder.AppendLine($"Log (previous): {(currentLogRow - 1 < 0 ? "N/A" : (log[currentLogRow - 1]).Text)}");

            return builder.ToString();
        }

        private string LookupFailureCode(ushort errorCodeAddress)
        {
            return $"Todo: lookup failure code in address {errorCodeAddress}";
        }
    }
}