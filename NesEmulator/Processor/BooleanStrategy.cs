using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BooleanStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte operand = GetOperand(cpu, opcode, firstOperand, memory);

                Func<byte, byte, byte> op = And;
                
                byte result = op(cpu.Accumulator, operand);
                cpu.Accumulator = result;

                cpu.SetFlagState(StatusFlags.Zero, result == 0);
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) != 0);
            }
            
            private byte GetOperand(CPU cpu, OpCode opcode, byte operand, IMemory memory)
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
                        operand = memory.Read(operand);
                        break;
                    }

                    case AddressMode.ZeroPageX:
                    {
                        operand = memory.Read(
                            (byte) ((operand + cpu.IndexX) % 256));
                        break;
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memory.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memory.Read(address);
                        break;
                    }

                    case AddressMode.AbsoluteX:
                    {
                        cyclePenalty += cpu.IndexX + operand > 0xFF ? 1 : 0;
                        var highByte = memory.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memory.Read((ushort) (address + cpu.IndexX));
                        break;
                    }

                    case AddressMode.AbsoluteY:
                    {
                        cyclePenalty += cpu.IndexY + operand > 0xFF ? 1 : 0;
                        var highByte = memory.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + operand);
                        operand = memory.Read((ushort) (address + cpu.IndexY));
                        break;
                    }

                    case AddressMode.IndirectX:
                    {
                        // Index applied during indirection
                        ushort address = (byte) (operand + cpu.IndexX);
                        var low = memory.Read(address);
                        var high = memory.Read((byte) ((address + 1) % 256));
                        address = (ushort) ((high << 8) + low);
                        operand = memory.Read(address);
                        break;
                    }

                    case AddressMode.IndirectY:
                    {
                        // Index applied after indirection
                        var low = memory.Read(operand);
                        var high = memory.Read((byte) ((operand + 1) % 256));
                        var address = (ushort) ((high << 8) + low + cpu.IndexY);
                        cyclePenalty += address >> 8 != high ? 1 : 0; // Check address page vs high byte
                        operand = memory.Read(address);
                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
                }

                cpu.ElapsedCycles += cyclePenalty;

                return operand;
            }
            
            private byte And(byte operand1, byte operand2)
            {
                return (byte)(operand1 & operand2);
            }
        }
    }
}