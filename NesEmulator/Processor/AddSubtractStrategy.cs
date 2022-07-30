using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class AddSubtractStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                byte operand = GetOperand(cpu, opcode, firstOperand, memoryBus);

                switch (opcode.Operation)
                {
                    case Operation.ADC:
                    {
                        break;
                    }

                    case Operation.SBC:
                    {
                        operand = (byte)(~operand);
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle Operation {opcode.Operation}");
                }
                
                byte initialAccumulator = cpu.Accumulator;
                int result = initialAccumulator + operand;
                if (cpu.Status.HasFlagFast(StatusFlags.Carry)) result += 1;

                cpu.Accumulator = (byte)(result % 256);

                cpu.SetFlagState(StatusFlags.Carry, result >> 8 != 0);
                bool signWasFlipped = ((initialAccumulator ^ result) & (operand ^ result) & 0x80) != 0;
                cpu.SetFlagState(StatusFlags.Overflow, signWasFlipped);
                cpu.SetFlagState(StatusFlags.Zero, cpu.Accumulator == 0x0);
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) != 0);
            }
            
            private byte GetOperand(CPU cpu, OpCode opcode, byte operand, IMemoryBus memoryBus)
            {
                int cyclePenalty = 0;

                switch (opcode.AddressMode)
                {
                    case AddressMode.Immediate:
                    {
                        // Intentionally empty
                        break;
                    }

                    case AddressMode.ZeroPage:
                    {
                        operand = memoryBus.Read(operand);
                        break;
                    }

                    case AddressMode.ZeroPageX:
                    {
                        operand = memoryBus.Read(
                            (byte) ((operand + cpu.IndexX) % 256));
                        break;
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memoryBus.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memoryBus.Read(address);
                        break;
                    }

                    case AddressMode.AbsoluteX:
                    {
                        cyclePenalty += cpu.IndexX + operand > 0xFF ? 1 : 0;
                        var highByte = memoryBus.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memoryBus.Read((ushort) (address + cpu.IndexX));
                        break;
                    }

                    case AddressMode.AbsoluteY:
                    {
                        cyclePenalty += cpu.IndexY + operand > 0xFF ? 1 : 0;
                        var highByte = memoryBus.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memoryBus.Read((ushort) (address + cpu.IndexY));
                        break;
                    }

                    case AddressMode.IndirectX:
                    {
                        // Index applied during indirection
                        ushort address = (byte) (operand + cpu.IndexX);
                        var low = memoryBus.Read(address);
                        var high = memoryBus.Read((byte) ((address + 1) % 256));
                        address = (ushort) ((high << 8) + low);
                        operand = memoryBus.Read(address);
                        break;
                    }

                    case AddressMode.IndirectY:
                    {
                        // Index applied after indirection
                        var low = memoryBus.Read(operand);
                        var high = memoryBus.Read((byte) ((operand + 1) % 256));
                        var address = (ushort) ((high << 8) + low + cpu.IndexY);
                        cyclePenalty += address >> 8 != high ? 1 : 0; // Check address page vs high byte
                        operand = memoryBus.Read(address);
                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
                }

                cpu.ElapsedCycles += cyclePenalty;

                return operand;
            }
        }
    }
}