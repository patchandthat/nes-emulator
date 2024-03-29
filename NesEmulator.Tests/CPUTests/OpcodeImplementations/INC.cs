using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class INC
    {
        [Trait("Category", "Unit")]
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.INC, AddressMode.ZeroPage);

                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x00, 0x01)]
            [InlineData(0x30, 0x63, 0x64)]
            [InlineData(0xC7, 0xFE, 0xFF)]
            [InlineData(0xC7, 0xFF, 0x00)]
            public void IncrementsCorrectAddress(byte zeroPageAddr, byte stored, byte expectedWrite)
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(zeroPageAddr))
                    .Returns(stored);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddr);

                sut.Step();

                A.CallTo(() => _memoryBus.Write(zeroPageAddr, expectedWrite))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0xFF;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x06;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x85;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Fact]
            public void Consumes5Cycles()
            {
                var sut = CreateSut();

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IncrementsInstructionPointerByTwo()
            {
                var sut = CreateSut();

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                var expectedPointer = sut.InstructionPointer.Plus(2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }

        [Trait("Category", "Unit")]
        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.INC, AddressMode.ZeroPageX);

                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x15, 0x0015, 0x00, 0x01)]
            [InlineData(0x30, 0x24, 0x0054, 0x63, 0x64)]
            [InlineData(0xC7, 0xD1, 0x0098, 0xFE, 0xFF)]
            [InlineData(0xC7, 0x00, 0x00C7, 0xFF, 0x00)]
            public void IncrementsCorrectAddress(
                byte operand,
                byte xOffset,
                ushort expectedTargetAddr,
                byte valueBefore,
                byte valueAfter)
            {
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);

                A.CallTo(() => _memoryBus.Read(expectedTargetAddr))
                    .Returns(valueBefore);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                sut.Step();

                A.CallTo(() => _memoryBus.Write(expectedTargetAddr, valueAfter))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0xFF;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x06;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x85;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.ForceStatus(initialFlags);

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }

            [Fact]
            public void Consumes6Cycles()
            {
                var sut = CreateSut();

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IncrementsInstructionPointerByTwo()
            {
                var sut = CreateSut();

                byte address = 0x03;
                byte value = 0x38;

                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(value);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(address);

                var expectedPointer = sut.InstructionPointer.Plus(2);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }

        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.INC, AddressMode.Absolute);

                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x03, 0x0300, 0x00, 0x01)]
            [InlineData(0x18, 0x20, 0x2018, 0x80, 0x81)]
            [InlineData(0xFF, 0x14, 0x14FF, 0xFF, 0x00)]
            public void IncrementsCorrectAddress(
                byte lowByte,
                byte highByte,
                ushort expectedAddress,
                byte valueBefore,
                byte valueAfter)
            {
                var sut = CreateSut();

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(expectedAddress)).Returns(valueBefore);

                sut.Step();

                A.CallTo(() => _memoryBus.Write(expectedAddress, valueAfter)).MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0xFF;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0x4C;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0b0111_1111;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0x05;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void Consumes6Cycles()
            {
                var sut = CreateSut();

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0x05;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                var expectedCycles = sut.ElapsedCycles + 6;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IncrementsInstructionPointerBy3()
            {
                var sut = CreateSut();

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0x05;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }

        [Trait("Category", "Unit")]
        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.INC, AddressMode.AbsoluteX);

                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector))
                    .Returns((byte) 0x00);
                A.CallTo(() => _memoryBus.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte) 0x80);
            }

            private readonly IMemoryBus _memoryBus;
            private readonly OpCode _op;

            private CPU CreateSut()
            {
                var cpu = new CPU(_memoryBus);
                cpu.Power();
                cpu.Step();
                Fake.ClearRecordedCalls(_memoryBus);
                return cpu;
            }

            [Theory]
            [InlineData(0x00, 0x03, 0x06, 0x0306, 0x00, 0x01)]
            [InlineData(0x18, 0x20, 0x07, 0x201F, 0x80, 0x81)]
            [InlineData(0xFF, 0x14, 0x08, 0x1507, 0xFF, 0x00)]
            public void IncrementsCorrectAddress(
                byte lowByte,
                byte highByte,
                byte xOffset,
                ushort expectedAddress,
                byte valueBefore,
                byte valueAfter)
            {
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(expectedAddress)).Returns(valueBefore);

                sut.Step();

                A.CallTo(() => _memoryBus.Write(expectedAddress, valueAfter)).MustHaveHappened();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagIfResultIsZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDX(0x10, _memoryBus);

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2020;
                byte value = 0xFF;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagIfResultIsNotZero(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDX(0x10, _memoryBus);

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2020;
                byte value = 0x4C;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagIfBit7IsHigh(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDX(0x01, _memoryBus);

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2011;
                byte value = 0xA5;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagIfBit7IsLow(StatusFlags initialFlags)
            {
                var sut = CreateSut();
                sut.LDX(0x02, _memoryBus);

                sut.ForceStatus(initialFlags);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2012;
                byte value = 0x10;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void Consumes7Cycles()
            {
                var sut = CreateSut();

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2010;
                byte value = 0x05;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                var expectedCycles = sut.ElapsedCycles + 7;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void IncrementsInstructionPointerBy3()
            {
                var sut = CreateSut();
                sut.LDX(0x05, _memoryBus);

                byte lowByte = 0x10;
                byte highByte = 0x20;
                ushort address = 0x2015;
                byte value = 0x05;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer)).Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);

                A.CallTo(() => _memoryBus.Read(address)).Returns(value);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
    }
}