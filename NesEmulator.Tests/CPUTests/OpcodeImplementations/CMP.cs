using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class CMP
    {
        [Trait("Category", "Unit")]
        public class Immediate
        {
            public Immediate()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.Immediate);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes2Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 2;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
        
        [Trait("Category", "Unit")]
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.ZeroPage);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x50;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xF1;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xA6;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x64;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x2F;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xAA;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(zeroPageAddress))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes3Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 3;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

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
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.ZeroPageX);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xD1;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xA6;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x64;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0x2F;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                byte zeroPageAddress = 0xAA;
                byte xOffset = 0x10;
                ushort address = (ushort)(zeroPageAddress + xOffset);
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 4;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
            
            [Fact]
            public void ZeroPageWrapAround()
            {
                byte zeroPageAddress = 0xAA;
                byte xOffset = 0x66;
                ushort expectedAddress = 0x0010;
                
                var sut = CreateSut();
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                
                sut.Step();
                
                A.CallTo(() => _memoryBus.Read(expectedAddress))
                    .MustHaveHappened();
            }
        }

        [Trait("Category", "Unit")]
        public class Absolute
        {
            public Absolute()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.Absolute);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

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
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.AbsoluteX);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x05, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

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
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void PageCrossPenaltyTakesOneExtraCycle()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDX(0x01, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0xFF;
                byte high = 0x03;
                ushort address = 0x0400;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
        
        [Trait("Category", "Unit")]
        public class AbsoluteY
        {
            public AbsoluteY()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.AbsoluteY);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue,
                byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
            }

            [Fact]
            public void ExecutionTakes4Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 4;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x06, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x031B;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
            
            [Fact]
            public void PageCrossPenaltyTakesOneExtraCycle()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;

                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.LDY(0x01, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0xFF;
                byte high = 0x03;
                ushort address = 0x0400;
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(address))
                    .Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 5;

                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }
        }
        
        [Trait("Category", "Unit")]
        public class IndirectX
        {
            public IndirectX()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.IndirectX);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Fact]
            public void ExecutionTakes6Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);
                
                var expectedCycles = sut.ElapsedCycles + 6;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0067;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();
                
                sut.InstructionPointer.Should().Be(expectedPointer);
            }

            [Fact]
            public void ZeroPageWrapAround()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0xFD;
                byte xOffset = 0x17;
                ushort lowByteAddress = 0x0014;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0334;
                
                sut.LDX(xOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();
                
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }
        }
        
        [Trait("Category", "Unit")]
        public class IndirectY
        {
            public IndirectY()
            {
                _memoryBus = A.Fake<IMemoryBus>();
                _op = new OpCodes().FindOpcode(Operation.CMP, AddressMode.IndirectY);

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
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x4A, 0x62)]
            public void ClearsCarryFlagWhenTargetRegisterIsLessThanComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Carry)
                    .Should().BeFalse();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x01)]
            [InlineData(0xCA, 0xCA)]
            public void SetsZeroFlagWhenTargetRegisterIsEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0x01, 0x02)]
            [InlineData(0x56, 0x34)]
            public void ClearsZeroFlagWhenTargetRegisterIsNotEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().BeFalse();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void SetsNegativeFlagIfComparisonResultIsNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Theory]
            [InlineData(0xC0, 0x12)]
            [InlineData(0xFF, 0x00)]
            public void ClearsNegativeFlagIfComparisonResultIsNotNegative(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().BeTrue();
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }

            [Fact]
            public void ExecutionTakes5Cycles()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x17;
                ushort lowByteAddress = 0x0050;
                byte low = 0x34;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x034B;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 5;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
            }

            [Fact]
            public void InstructionPointerIsIncremented()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
            
            [Fact]
            public void PageCrossPenalty()
            {
                byte registerValue = 0x00;
                byte comparisonValue = 0x00;
                                
                var sut = CreateSut();
                sut.LDA(registerValue, _memoryBus);
                
                byte zeroPageAddress = 0x50;
                byte yOffset = 0x01;
                ushort lowByteAddress = 0x0050;
                byte low = 0xFF;
                byte high = 0x03;
                ushort expectedComparisonValueAddress = 0x0400;
                
                sut.LDY(yOffset, _memoryBus);
                sut.ForceStatus(StatusFlags.None);
                    
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memoryBus.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memoryBus.Read(lowByteAddress))
                    .Returns(low);
                A.CallTo(() => _memoryBus.Read((ushort)(lowByteAddress+1)))
                    .Returns(high);
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress)).Returns(comparisonValue);

                var expectedCycles = sut.ElapsedCycles + 6; 
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedCycles);
                
                A.CallTo(() => _memoryBus.Read(expectedComparisonValueAddress))
                    .MustHaveHappened();
            }
        }
    }
}