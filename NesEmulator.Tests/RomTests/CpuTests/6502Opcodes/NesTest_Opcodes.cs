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

namespace NesEmulator.UnitTests.RomTests.CpuTests._6502Opcodes
{
    public class NesTest_Opcodes
    {
        private const string RomFile = "RomTests/CpuTests/6502Opcodes/nestest.nes";
        private const string ExpectedBehaviourLogFile = "RomTests/CpuTests/6502Opcodes/nestest.log.txt";
        
        private const ushort TestStartPrgAddress = 0xC000;
        private const ushort ErrorCodeAddress1 = 0x0002;
        private const ushort ErrorCodeAddress2 = 0x0003;

        [Fact]
        public void RunNesTestRomAutomated()
        {
            Assert.False(true, "Need to dive into this & compare against the log file. \nImplement BRK and Interrupts.");
            
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

            // Todo: work out how long this should be, or a definite exit condition
            for (int i = 0; i < 100000; i++)
            {
                cpu.Step();
            }

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