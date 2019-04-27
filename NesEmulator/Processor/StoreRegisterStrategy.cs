using System;
using NesEmulator.Extensions;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class StoreRegisterStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                var address = ResolveTargetAddress(opcode, firstOperand, cpu, memory);

                switch (opcode.Operation)
                {
                    case Operation.STA:
                        memory.Write(address, cpu.Accumulator);
                        break;

                    case Operation.STX:
                        memory.Write(address, cpu.IndexX);
                        break;

                    case Operation.STY:
                        memory.Write(address, cpu.IndexY);
                        break;

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle Operation {opcode.Operation}");
                }
            }

            private ushort ResolveTargetAddress(OpCode opcode, byte operand, CPU cpu, IMemory memory)
            {
                switch (opcode.AddressMode)
                {
                    case AddressMode.ZeroPage:
                        return operand;

                    case AddressMode.ZeroPageX:
                        return (ushort) ((operand + cpu.IndexX) % 256);

                    case AddressMode.ZeroPageY:
                        return (ushort) ((operand + cpu.IndexY) % 256);

                    case AddressMode.Absolute:
                    {
                        var highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand);
                        return address;
                    }

                    case AddressMode.AbsoluteX:
                    {
                        var highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand);
                        return address.Plus(cpu.IndexX);
                    }

                    case AddressMode.AbsoluteY:
                    {
                        var highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        var address = (ushort) ((highByte << 8) + operand);
                        return address.Plus(cpu.IndexY);
                    }

                    case AddressMode.IndirectX:
                    {
                        var zeroPageByte = (byte) ((operand + cpu.IndexX) % 256);
                        var lowByte = memory.Read(zeroPageByte);
                        var highByte = memory.Read((ushort) ((zeroPageByte + 1) % 256));
                        var address = (ushort) ((highByte << 8) + lowByte);
                        return address;
                    }

                    case AddressMode.IndirectY:
                    {
                        var zeroPageByte = operand;
                        var lowByte = memory.Read(zeroPageByte);
                        var highByte = memory.Read((ushort) (zeroPageByte + 1));
                        var address = (ushort) ((highByte << 8) + lowByte);
                        return address.Plus(cpu.IndexY);
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
                }
            }
        }
    }
}