using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal class LoadRegisterStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                var value = ResolveOperandValue(opcode, firstOperand, cpu, memoryBus);

                switch (opcode.Operation)
                {
                    case Operation.LDA:
                        LoadRegister(cpu, value, b => cpu.Accumulator = b);
                        break;
                    case Operation.LDX:
                        LoadRegister(cpu, value, b => cpu.IndexX = b);
                        break;
                    case Operation.LDY:
                        LoadRegister(cpu, value, b => cpu.IndexY = b);
                        break;
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
            }

            private byte ResolveOperandValue(OpCode opcode, byte operand, CPU cpu, IMemoryBus memoryBus)
            {
                var cyclePenalty = 0;

                switch (opcode.AddressMode)
                {
                    case AddressMode.Immediate:
                        // Intentionally empty
                        break;

                    case AddressMode.ZeroPage:
                        operand = memoryBus.Read(operand);
                        break;

                    case AddressMode.ZeroPageX:
                        operand = memoryBus.Read(
                            (byte) ((operand + cpu.IndexX) % 256));
                        break;

                    case AddressMode.ZeroPageY:
                        operand = memoryBus.Read(
                            (byte) ((operand + cpu.IndexY) % 256));
                        break;

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

            private void LoadRegister(CPU cpu, byte value, Action<byte> registerAction)
            {
                registerAction(value);

                cpu.SetFlagState(StatusFlags.Zero, value == 0x0);
                cpu.SetFlagState(StatusFlags.Negative, value >= 0x80);
            }
        }
    }
}