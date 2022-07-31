using System;
using NesEmulator.Extensions;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BitshiftStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte operand, IMemoryBus memoryBus)
            {
                var toRotate = GetValue(opcode.AddressMode, cpu, memoryBus, operand);

                var carryWasSet = cpu.Status.HasFlagFast(StatusFlags.Carry);
                bool carryWillSet;
                byte result;
                switch (opcode.Operation)
                {
                    case Operation.ROL:
                    {
                        carryWillSet = (toRotate & 0x80) > 0;
                        result = RotateLeft(toRotate, carryWasSet);
                        break;
                    }

                    case Operation.ROR:
                    {
                        carryWillSet = (toRotate & 0x01) > 0;
                        result = RotateRight(toRotate, carryWasSet);
                        break;
                    }

                    case Operation.ASL:
                    {
                        carryWillSet = (toRotate & 0x80) > 0;
                        result = RotateLeft(toRotate, false);
                        break;
                    }

                    case Operation.LSR:
                    {
                        carryWillSet = (toRotate & 0x01) > 0;
                        result = RotateRight(toRotate, false);
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }

                WriteResult(opcode.AddressMode, cpu, memoryBus, operand, result);

                cpu.SetFlagState(StatusFlags.Carry, carryWillSet);
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) > 0);
                cpu.SetFlagState(StatusFlags.Zero, result == 0x0);
            }

            private byte GetValue(AddressMode addressMode, CPU cpu, IMemoryBus memoryBus, byte operand)
            {
                switch (addressMode)
                {
                    case AddressMode.Accumulator:
                    {
                        return cpu.Accumulator;
                    }

                    case AddressMode.ZeroPage:
                    {
                        return memoryBus.Read(operand);
                    }

                    case AddressMode.ZeroPageX:
                    {
                        return memoryBus.Read((byte) ((operand + cpu.IndexX) % 256));
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memoryBus.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand);
                        return memoryBus.Read(address);
                    }

                    case AddressMode.AbsoluteX:
                    {
                        var highByte = memoryBus.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand + cpu.IndexX);
                        return memoryBus.Read(address);
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {addressMode}");
                }
            }

            private byte RotateLeft(byte toRotate, bool carryWasSet)
            {
                var result = (byte) ((toRotate << 1) % 256);
                if (carryWasSet) result |= 0x1;

                return result;
            }

            private byte RotateRight(byte toRotate, bool carryWasSet)
            {
                var result = (byte) ((toRotate >> 1) % 256);
                if (carryWasSet) result |= 0x80;

                return result;
            }

            private void WriteResult(AddressMode addressMode, CPU cpu, IMemoryBus memoryBus, byte operand, byte result)
            {
                switch (addressMode)
                {
                    case AddressMode.Accumulator:
                    {
                        cpu.Accumulator = result;
                        break;
                    }

                    case AddressMode.ZeroPage:
                    {
                        memoryBus.Write(operand, result);
                        break;
                    }

                    case AddressMode.ZeroPageX:
                    {
                        memoryBus.Write((byte) ((operand + cpu.IndexX) % 256), result);
                        break;
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memoryBus.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand);
                        memoryBus.Write(address, result);
                        break;
                    }

                    case AddressMode.AbsoluteX:
                    {
                        var highByte = memoryBus.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand + cpu.IndexX);
                        memoryBus.Write(address, result);
                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {addressMode}");
                }
            }
        }
    }
}