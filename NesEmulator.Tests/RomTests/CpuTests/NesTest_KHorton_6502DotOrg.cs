using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NesEmulator.APU;
using NesEmulator.Input;
using NesEmulator.Memory;
using NesEmulator.PPU;
using NesEmulator.Processor;
using NesEmulator.RomMappers;
using Xunit;

namespace NesEmulator.RomTests.CpuTests
{
    public class NesTest_KHorton_6502DotOrg
    {
        private const string RomFile = "RomTests/CpuTests/nestest.nes";
        private const ushort TestStartPrgAddress = 0xC000;
        private const ushort ErrorCodeAddress1 = 0x0002;
        private const ushort ErrorCodeAddress2 = 0x0003;

        [Fact]
        public void RunNesTestRomAutomated()
        {
            bool unimplementedOpcodeEncountered = false;
            ROM rom;
            using (var file = File.OpenRead(RomFile))
            {
                rom = ROM.Create(file);
            }

            var memory = new MainMemory(new NullPpu(), new NullApu(), new NullInputSource(), new NullInputSource());
            var cpu = new CPU(memory);
            memory.Load(rom);
            
            cpu.Power();
            cpu.Step();
            cpu.ForceInstructionPointer(TestStartPrgAddress);

            try
            {
                // Todo: work out how long this should be, or a definite exit condition
                for (int i = 0; i < 100000; i++)
                {
                    cpu.Step();
                }
            }
            catch (KeyNotFoundException e)
            {
                // Unofficial opcode - verify so far and exit
                unimplementedOpcodeEncountered = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Test halted. Exception thrown.");
                Console.WriteLine(e);
                Console.WriteLine($"Ran through {cpu.ElapsedCycles} cycles");
                throw;
            }

            memory.Peek(ErrorCodeAddress1)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress1));
            
            memory.Peek(ErrorCodeAddress2)
                .Should().Be(0x00, LookupFailureCode(ErrorCodeAddress2));
            
            Assert.False(unimplementedOpcodeEncountered, "Encountered an unimplemented opcode - test execution aborted");
        }

        private string LookupFailureCode(ushort errorCodeAddress)
        {
            return $"Todo: lookup failure code in address {errorCodeAddress}";
        }
    }
}