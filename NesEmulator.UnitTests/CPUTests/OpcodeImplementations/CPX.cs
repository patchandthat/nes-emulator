using FakeItEasy;
using FluentAssertions;
using NesEmulator.Extensions;
using NesEmulator.Memory;
using NesEmulator.Processor;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests.OpcodeImplementations
{
    public class CPX
    {
        public class Immediate
        {
            public Immediate()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CPX, AddressMode.Immediate);

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
                cpu.Step();
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Theory]
            [InlineData(0x01, 0x00)]
            [InlineData(0xFF, 0xFF)]
            [InlineData(0x7F, 0x0F)]
            public void SetsCarryFlagWhenTargetRegisterIsGreaterThanOrEqualToComparedValue(byte registerValue, byte comparisonValue)
            {
                var sut = CreateSut();
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
        
        public class ZeroPage
        {
            public ZeroPage()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CPX, AddressMode.ZeroPage);

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
                cpu.Step();
                Fake.ClearRecordedCalls(_memory);
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(zeroPageAddress);
                A.CallTo(() => _memory.Read(zeroPageAddress))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(2);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
        
        public class Absolute
        {
            public Absolute()
            {
                _memory = A.Fake<IMemory>();
                _op = new OpCodes().FindOpcode(Operation.CPX, AddressMode.Absolute);

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
                cpu.Step();
                Fake.ClearRecordedCalls(_memory);
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.None);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
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
                sut.LDX(registerValue, _memory);
                sut.ForceStatus(StatusFlags.All);

                byte low = 0x15;
                byte high = 0x03;
                ushort address = 0x0315;
                
                A.CallTo(() => _memory.Read(sut.InstructionPointer))
                    .Returns(_op.Value);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1)))
                    .Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2)))
                    .Returns(high);
                A.CallTo(() => _memory.Read(address))
                    .Returns(comparisonValue);

                var expectedPointer = sut.InstructionPointer.Plus(3);

                sut.Step();

                sut.InstructionPointer.Should().Be(expectedPointer);
            }
        }
    }
}