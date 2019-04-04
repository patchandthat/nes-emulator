using System;
using NesEmulator.Extensions;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class StoreRegisterStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                ushort address = ResolveTargetAddress(opcode, firstOperand, cpu, memory);

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
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle Operation {opcode.Operation}");
                }
            }

            private ushort ResolveTargetAddress(OpCode opcode, byte operand, CPU cpu, IMemory memory)
            {
                switch (opcode.AddressMode)
                {
                    case AddressMode.ZeroPage:
                        return operand;
                        
                    case AddressMode.ZeroPageX:
                        return (ushort)((operand + cpu.IndexX) % 256);
                    
                    case AddressMode.ZeroPageY:
                        return (ushort)((operand + cpu.IndexY) % 256);

                    case AddressMode.Absolute:
                    {
                        byte highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        ushort address = (ushort) ((highByte << 8) + operand);
                        return address;
                    }
                    
                    case AddressMode.AbsoluteX:
                    {
                        byte highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        ushort address = (ushort) ((highByte << 8) + operand);
                        return address.Plus(cpu.IndexX);
                    }
                    
                    case AddressMode.AbsoluteY:
                    {
                        byte highByte = memory.Read(cpu.InstructionPointer.Plus(2));
                        ushort address = (ushort) ((highByte << 8) + operand);
                        return address.Plus(cpu.IndexY);
                    }
                    
                    case AddressMode.IndirectX:
                    {
                        byte zeroPageByte = (byte)((operand + cpu.IndexX) % 256);
                        byte lowByte = memory.Read(zeroPageByte);
                        byte highByte = memory.Read((ushort) ((zeroPageByte + 1) % 256));
                        ushort address = (ushort) ((highByte << 8) + lowByte);
                        return address;
                    }
                    
                    case AddressMode.IndirectY:
                    {
                        byte zeroPageByte = operand;
                        byte lowByte = memory.Read(zeroPageByte);
                        byte highByte = memory.Read((ushort) (zeroPageByte + 1));
                        ushort address = (ushort) ((highByte << 8) + lowByte);
                        return address.Plus(cpu.IndexY);
                    }
                    
                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
                }                
            }
        }
    }
}