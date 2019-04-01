using System;
using FakeItEasy;
using FluentAssertions;
using NesEmulator.UnitTests.Helpers;
using Xunit;

namespace NesEmulator.UnitTests.CPUTests
{
    public partial class CPUTests
    {
        public class LDX
        {
            private IMemory _memory;

            public LDX()
            {
                _memory = A.Fake<IMemory>();

                A.CallTo(() => _memory.Read(MemoryMap.ResetVector))
                    .Returns((byte)0x00);
                A.CallTo(() => _memory.Read(MemoryMap.ResetVector + 1))
                    .Returns((byte)0x80);
            }

            private CPU CreateSut()
            {
                var cpu = new CPU(_memory);
                cpu.Power();
                cpu.Step(); // Execute reset interrupt
                Fake.ClearRecordedCalls(_memory);
                return cpu;
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldFetchTwoBytes()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0xFF);

                var sut = CreateSut();

                sut.Step();

                A.CallTo(() => _memory.Read(A<ushort>._)).MustHaveHappenedTwiceExactly();
                A.CallTo(() => _memory.Read(0x8000)).MustHaveHappened();
                A.CallTo(() => _memory.Read(0x8001)).MustHaveHappened();
            }

            [Theory]
            [ClassData(typeof(AllByteValues))]
            public void ImmediateMode_OnExecute_ShouldLoadSecondByteToX(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.IndexX.Should().Be(value);
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldElapseTwoCycles()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var cyclesBefore = sut.ElapsedCycles;

                sut.Step();

                op.Cycles.Should().Be(2);
                sut.ElapsedCycles.Should().Be(cyclesBefore + op.Cycles);
            }

            [Theory]
            [ClassData(typeof(AllByteValues))]
            public void ImmediateMode_OnExecuteAndOperandIsZero_ShouldRaiseZeroFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(value == 0x00);
            }

            [Theory]
            [ClassData(typeof(AllByteValues))]
            public void ImmediateMode_OnExecuteAndOperandBit7IsHigh_ShouldRaiseNegativeFlag(byte value)
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)value);

                var sut = CreateSut();

                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(value >= 0b1000_0000);
            }

            [Fact]
            public void ImmediateMode_OnExecute_ShouldIncreaseInstructionPointer()
            {
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Immediate);
                A.CallTo(() => _memory.Read(0x8000)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(0x8001)).Returns((byte)0x00);

                var sut = CreateSut();

                var ipStart = sut.InstructionPointer;

                sut.Step();

                var ipDiff = sut.InstructionPointer - ipStart;

                ipDiff.Should().Be(op.Bytes);
            }
            
            [Theory]
            [InlineData(0x00)]
            [InlineData(0x0D)]
            [InlineData(0x3F)]
            [InlineData(0x45)]
            [InlineData(0xE2)]
            [InlineData(0xFF)]
            public void ZeroPage_OnExecute_ShouldLoadByteToX(byte value)
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x10);

                A.CallTo(() => _memory.Read(0x10))
                    .Returns(value);

                sut.Step();

                sut.IndexX.Should().Be(value);
            }

            [Fact]
            public void ZeroPage_WhenOperandIsZero_ShouldSetZeroFlag()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x00);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Fact]
            public void ZeroPage_WhenOperandNotZero_ShouldClearZeroFlag()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0xFF);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Fact]
            public void ZeroPage_WhenOperandBit7High_ShouldRaiseNegativeFlag()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x80);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Fact]
            public void ZeroPage_WhenOperandBit7IsLow_ShouldClearNegativeFlag()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x15);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void ZeroPage_OnExecute_ShouldIncreaseCycleCount()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x01);               

                var cyclesBefore = sut.ElapsedCycles;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(
                    cyclesBefore + op.Cycles);
            }

            [Fact]
            public void ZeroPage_OnExecute_ShouldIncreaseInstructionPointer()
            {
                var sut = CreateSut();
                
                byte value = 0xD4;

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPage);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0xD4))
                    .Returns((byte)0x01);               

                var ipBefore = sut.InstructionPointer;
                
                sut.Step();

                sut.InstructionPointer.Should().Be(
                    ipBefore.Plus(op.Bytes));
            }           

            [Theory]
            [InlineData(0x00)]
            [InlineData(0x0D)]
            [InlineData(0x3F)]
            [InlineData(0x45)]
            [InlineData(0xE2)]
            [InlineData(0xFF)]
            public void ZeroPageY_OnExecute_ShouldLoadByteToX(byte value)
            {
                var sut = CreateSut();
                
                sut.LDY(0x10, _memory);
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x10);

                A.CallTo(() => _memory.Read(0x20))
                    .Returns(value);

                sut.Step();

                sut.IndexX.Should().Be(value);
            }

            [Fact]
            public void ZeroPageY_WhenOperandIsZero_ShouldSetZeroFlag()
            {
                var sut = CreateSut();
                sut.LDY(0x01, _memory);
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x00);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(true);
            }

            [Fact]
            public void ZeroPageY_WhenOperandNotZero_ShouldClearZeroFlag()
            {
                var sut = CreateSut();
                sut.LDY(0x00, _memory);
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0xFF);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero)
                    .Should().Be(false);
            }

            [Fact]
            public void ZeroPageY_WhenOperandBit7High_ShouldRaiseNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDY((byte)0, _memory);
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x80);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(true);
            }

            [Fact]
            public void ZeroPageY_WhenOperandBit7IsLow_ShouldClearNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDY((byte)0xDD, _memory);
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x15);   
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative)
                    .Should().Be(false);
            }

            [Fact]
            public void ZeroPageY_OnExecute_ShouldIncreaseCycleCount()
            {
                var sut = CreateSut();
                
                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0x00))
                    .Returns((byte)0x01);               

                var cyclesBefore = sut.ElapsedCycles;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(
                    cyclesBefore + op.Cycles);
            }

            [Fact]
            public void ZeroPageY_OnExecute_ShouldIncreaseInstructionPointer()
            {
                var sut = CreateSut();
                
                byte value = 0xD4;
                sut.LDY(value, _memory);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x00);

                A.CallTo(() => _memory.Read(0xD4))
                    .Returns((byte)0x01);               

                var ipBefore = sut.InstructionPointer;
                
                sut.Step();

                sut.InstructionPointer.Should().Be(
                    ipBefore.Plus(op.Bytes));
            }           

            [Fact]
            public void ZeroPageY_WhenCrossingPageBoundary_WrapsBackToZeroPage()
            {
                var sut = CreateSut();
                
                byte value = 0xD4;
                sut.LDY(value, _memory);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.ZeroPageY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns((byte)0x8E);

                A.CallTo(() => _memory.Read(0x62))
                    .Returns((byte)0xEC);               
                
                sut.Step();

                sut.IndexX.Should().Be(0xEC);
            }

            [Theory]
            [InlineData(0x05, 0x03, 0x0305, 0xFE)]
            [InlineData(0x4A, 0x16, 0x164A, 0x74)]
            [InlineData(0xE1, 0x06, 0x06E1, 0x8C)]
            public void Absolute_ShouldLoadCorrectValueToX(byte lowByte, byte highByte, ushort expectedAddress, byte expectedValue)
            {
                var sut = CreateSut();

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);
                A.CallTo(() => _memory.Read(expectedAddress)).Returns(expectedValue);
                
                sut.Step();

                sut.IndexX.Should().Be(expectedValue);
            }

            [Fact]
            public void Absolute_WhenValueIsZero_ShouldSetZeroFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x01, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
                byte value = 0x00;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero).Should().Be(true);
            }

            [Fact]
            public void Absolute_WhenValueIsNotZero_ShouldClearZeroFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x00, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
                byte value = 0x01;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero).Should().Be(false);
            }
            
            [Fact]
            public void Absolute_WhenValueIsNegative_ShouldSetNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x01, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 250;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative).Should().Be(true);
            }

            [Fact]
            public void Absolute_WhenValueIsNotNegative_ShouldClearNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDA(0xFA, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative).Should().Be(false);
            }

            [Fact]
            public void Absolute_ShouldIncreaseInstructionPointerByOpBytes()
            {
                var sut = CreateSut();
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);

                ushort expectedIPLocation = sut.InstructionPointer.Plus(op.Bytes);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIPLocation);
            }

            [Fact]
            public void Absolute_ShouldIncreaseElapsedCyclesByOpCycles()
            {
                var sut = CreateSut();
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.Absolute);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);

                long expectedElapsedCycles = sut.ElapsedCycles + op.Cycles;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedElapsedCycles);
            }
            
            [Theory]
            [InlineData(0x05, 0x03, 0x17, 0x031C, 0xFE)]
            [InlineData(0x4A, 0x16, 0x5F, 0x16A9, 0x74)]
            [InlineData(0xE1, 0x06, 0xA2, 0x0783, 0x8C)]
            public void AbsoluteY_ShouldLoadCorrectValueToX(byte lowByte, byte highByte, byte yOffset, ushort expectedAddress, byte expectedValue)
            {
                var sut = CreateSut();
                sut.LDY(yOffset, _memory);

                OpCode op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(lowByte);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(highByte);
                A.CallTo(() => _memory.Read(expectedAddress)).Returns(expectedValue);
                
                sut.Step();

                sut.IndexX.Should().Be(expectedValue);
            }
            
            [Fact]
            public void AbsoluteY_WhenValueIsZero_ShouldSetZeroFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x01, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
                byte value = 0x00;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero).Should().Be(value == 0x00);
            }
            
            [Fact]
            public void AbsoluteY_WhenValueIsNotZero_ShouldClearZeroFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x00, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
                byte value = 0x01;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Zero).Should().Be(value == 0x00);
            }
            
            [Fact]
            public void AbsoluteY_WhenValueIsNegative_ShouldSetNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x01, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 250;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative).Should().Be(true);
            }

            [Fact]
            public void AbsoluteY_WhenValueIsNotNegative_ShouldClearNegativeFlag()
            {
                var sut = CreateSut();
                sut.LDA(0x9C, _memory);
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 20;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);
                
                sut.Step();

                sut.Status.HasFlag(StatusFlags.Negative).Should().Be(false);
            }
            
            [Fact]
            public void AbsoluteY_ShouldIncreaseInstructionPointerByOpBytes()
            {
                var sut = CreateSut();
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);

                ushort expectedIPLocation = sut.InstructionPointer.Plus(op.Bytes);
                
                sut.Step();

                sut.InstructionPointer.Should().Be(expectedIPLocation);
            }

            [Fact]
            public void AbsoluteY_ShouldIncreaseElapsedCyclesByOpCycles()
            {
                var sut = CreateSut();
                
                byte low = 0x55;
                byte high = 0x04;
                ushort address = (ushort)((high << 8) + low);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);

                long expectedElapsedCycles = sut.ElapsedCycles + op.Cycles;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedElapsedCycles);
            }
            
            [Fact]
            public void AbsoluteY_WhenCrossingPageBoundary_ShouldIncreaseElapsedCyclesByOpCyclesPlusOne()
            {
                var sut = CreateSut();
                byte offset = 0x10;
                sut.LDY(offset, _memory);
                
                byte low = 0xFA;
                byte high = 0x04;
                ushort address = (ushort) (((high << 8) + low) + offset);
   
                byte value = 5;
                
                var op = new OpcodeDefinitions().FindOpcode(Operation.LDX, AddressMode.AbsoluteY);
                A.CallTo(() => _memory.Read(sut.InstructionPointer)).Returns(op.Hex);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(1))).Returns(low);
                A.CallTo(() => _memory.Read(sut.InstructionPointer.Plus(2))).Returns(high);

                A.CallTo(() => _memory.Read(address)).Returns(value);

                long expectedElapsedCycles = sut.ElapsedCycles + op.Cycles + 1;
                
                sut.Step();

                sut.ElapsedCycles.Should().Be(expectedElapsedCycles);
            }
        }
    }
}