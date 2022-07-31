using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class EOR
    {
        [Trait("Category", "Unit")]
        public class Immediate
        {
            public Immediate()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.Immediate);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b0111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByTwoBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesTwoCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 2;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }

        [Trait("Category", "Unit")]
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.ZeroPage);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;
                byte zeroPageAddress = 0x56;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByTwoBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesThreeCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x56;

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }

        [Trait("Category", "Unit")]
        public class ZeroPageX
        {
            public ZeroPageX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.ZeroPageX);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;
                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulatorStart, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByTwoBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFourCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte zeroPageAddress = 0x56;
                byte xOffset = 0x13;
                ushort operandAddress = 0x0069;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }

            [Fact]
            public void ZeroPageWrapAround()
            {
                byte accumulator = 0b1111_0011;
                byte operand = 0b1111_1111;
                byte zeroPageAddress = 0xF0;
                byte xOffset = 0x20;
                ushort operandAddress = 0x0010;

                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .Returns(operand);

                sut.Step();

                sut.Accumulator.Should().Be(0b0000_1100);
                A.CallTo(() => _memoryBus.Read(operandAddress))
                    .MustHaveHappened();
            }
        }

        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.Absolute);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;

                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByThreeBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFourCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }

        [Trait("Category", "Unit")]
        public class AbsoluteX
        {
            public AbsoluteX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.AbsoluteX);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;

                var sut = CreateSut();
                sut.LDX(0x05, _memoryBus);
                sut.LDA(accumulatorStart, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByThreeBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFourCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }

            [Fact]
            public void PageCrossingPenaltyOfOneCycle()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte xOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0xFF;
                byte high = 0x03;
                ushort address = 0x0404;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();
                
                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }
        
        [Trait("Category", "Unit")]
        public class AbsoluteY
        {
            public AbsoluteY()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.AbsoluteY);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;

                var sut = CreateSut();
                sut.LDY(0x05, _memoryBus);
                sut.LDA(accumulatorStart, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte operand = 0b1111_0000;
                byte accumulator = 0b0111_1111;
                
                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);
                sut.ForceStatus(initialFlags);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByThreeBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(3);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFourCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031A;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }

            [Fact]
            public void PageCrossingPenaltyOfOneCycle()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                byte yOffset = 0x05;
                
                var sut = CreateSut();
                sut.LDY(yOffset, _memoryBus);
                sut.LDA(accumulator, _memoryBus);

                byte low = 0xFF;
                byte high = 0x03;
                ushort address = 0x0404;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();
                
                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }
        
        [Trait("Category", "Unit")]
        public class IndirectX
        {
            public IndirectX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.IndirectX);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulatorStart, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                var sut = CreateSut();
                sut.LDA(accumulatorStart, _memoryBus);

                byte operand = valueToAndWith;
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByTwoBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesSixCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
            
            [Fact]
            public void ZeroPageWrap()
            {
                byte accumulator = 0b1010_1111;
                byte operand = 0b0010_0100;
                byte expectedAccumulatorResult = 0b1000_1011;
                
                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0xFD;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0014;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }            
        }
        
        [Trait("Category", "Unit")]
        public class IndirectY
        {
            public IndirectY()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.EOR, AddressMode.IndirectY);

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
            [InlineData(0b0000_0000, 0b0000_0000, 0b0000_0000)]
            [InlineData(0b0000_0000, 0b1111_1111, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b0000_0000, 0b1111_1111)]
            [InlineData(0b1111_1111, 0b1111_1111, 0b0000_0000)]
            [InlineData(0b0101_0101, 0b1001_1001, 0b1100_1100)]
            public void AccumulatorContainsTheResultOfBitwiseAND(byte accumulator, byte valueToAndWith, byte expectedAccumulatorResult)
            {
                byte operand = valueToAndWith;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Accumulator.Should().Be(expectedAccumulatorResult);
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsZeroFlagWhenResultIsZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b1111_0000;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsZeroFlagWhenResultIsNotZero(StatusFlags initialFlags)
            {
                byte accumulator = 0b0000_1111;
                byte operand = 0b1111_1111;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void SetsNegativeFlagWhenResultIsNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }
            
            [Theory]
            [InlineData(StatusFlags.None)]
            [InlineData(StatusFlags.All)]
            public void ClearsNegativeFlagWhenResultIsNotNegative(StatusFlags initialFlags)
            {
                byte accumulator = 0b1000_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(initialFlags);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeFalse();
            }
            
            [Fact]
            public void InstructionPointerMovesByTwoBytes()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer
                    .Should().Be(expectedPointer);
            }

            [Fact]
            public void ExecutionTakesFiveCycles()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x034B;

                sut.LDY(yOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }

            [Fact]
            public void PageCrossPenaltyOfOneExtraCycle()
            {
                byte accumulator = 0b0111_1111;
                byte operand = 0b1111_0000;

                var sut = CreateSut();
                sut.LDA(accumulator, _memoryBus);

                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0xFF;
                byte high = 0x03;
                ushort expectedOperandAddress = 0x0416;

                sut.LDY(yOffset, _memoryBus);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedOperandAddress)).Returns(operand);

                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles
                    .Should().Be(expectedCycles);
            }
        }
    }
}