using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public static class STX
    {
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.STX, AddressMode.ZeroPage);

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x3E, 0x56, 0x0056)]
            [InlineData(0x72, 0x8B, 0x008B)]
            [InlineData(0xA3, 0xFF, 0x00FF)]
            public void WritesValueToCorrectMemoryLocation(byte value, byte operand, ushort expectedAddress)
            {
                var sut = CreateSut();
                sut.LDX(value, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                sut.Step();

                A.CallTo(() => _memory.Write(expectedAddress, value))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.All)]
            [InlineData(StatusFlags.None)]
            public void DoesNotModifyAnyFlags(StatusFlags flagStates)
            {
                var sut = CreateSut();

                sut.LDX(0xFF, _memory);
                sut.ForceStatus(flagStates);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                sut.Step();

                sut.Status.Should().Be(flagStates);
            }

            [Fact]
            public void IncreasesElapsedCycleCount()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedCycleCount = sut.ElapsedCycles + _op.Cycles;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycleCount);
            }

            [Fact]
            public void IncreasesInstructionPointer()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(_op.Bytes);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }
        }

        public class ZeroPageY
        {
            public ZeroPageY()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.STX, AddressMode.ZeroPageY);

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x3E, 0x00, 0x56, 0x0056)]
            [InlineData(0x72, 0x10, 0x8B, 0x009B)]
            [InlineData(0xA3, 0x20, 0xFF, 0x001F)]
            public void WritesValueToCorrectMemoryLocation(
                byte value,
                byte yOffset,
                byte operand,
                ushort expectedAddress)
            {
                var sut = CreateSut();
                sut.LDY(yOffset, _memory);
                sut.LDX(value, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                sut.Step();

                A.CallTo(() => _memory.Write(expectedAddress, value))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.All)]
            [InlineData(StatusFlags.None)]
            public void DoesNotModifyAnyFlags(StatusFlags flagStates)
            {
                var sut = CreateSut();

                sut.LDX(0xFF, _memory);
                sut.ForceStatus(flagStates);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                sut.Step();

                sut.Status.Should().Be(flagStates);
            }

            [Fact]
            public void IncreasesElapsedCycleCount()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedCycleCount = sut.ElapsedCycles + _op.Cycles;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycleCount);
            }

            [Fact]
            public void IncreasesInstructionPointer()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(_op.Bytes);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }
        }

        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.STX, AddressMode.Absolute);

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemory _memory;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x3E, 0x56, 0x03, 0x0356)]
            [InlineData(0x72, 0x8B, 0x00, 0x008B)]
            [InlineData(0xA3, 0xFF, 0x80, 0x80FF)]
            public void WritesValueToCorrectMemoryLocation(
                byte value,
                byte operand,
                byte secondOperand,
                ushort expectedAddress)
            {
                var sut = CreateSut();
                sut.LDX(value, _memory);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(secondOperand);

                sut.Step();

                A.CallTo(() => _memory.Write(expectedAddress, value))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.All)]
            [InlineData(StatusFlags.None)]
            public void DoesNotModifyAnyFlags(StatusFlags flagStates)
            {
                var sut = CreateSut();

                sut.LDX(0xFF, _memory);
                sut.ForceStatus(flagStates);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                sut.Step();

                sut.Status.Should().Be(flagStates);
            }

            [Fact]
            public void IncreasesElapsedCycleCount()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedCycleCount = sut.ElapsedCycles + _op.Cycles;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycleCount);
            }

            [Fact]
            public void IncreasesInstructionPointer()
            {
                var sut = CreateSut();

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns((byte) 0x00);

                var expectedInstructionPointer = sut.InstructionPointer.Plus(_op.Bytes);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedInstructionPointer);
            }
        }
    }
}