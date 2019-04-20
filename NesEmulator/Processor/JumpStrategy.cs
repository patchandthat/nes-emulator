using System;
using System.Transactions;
using NesEmulator.Extensions;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class JumpStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte secondOperand = memory.Read(cpu.InstructionPointer.Plus(2));

                switch (opcode.Operation)
                {
                    case Operation.JMP:
                    {
                        Jump(cpu, opcode, firstOperand, secondOperand, memory);
                        break;
                    }

                    case Operation.JSR:
                    {
                        JumpSubroutine(cpu, opcode, firstOperand, secondOperand, memory);
                        break;
                    }

                    case Operation.RTS:
                    {
                        ReturnSubroutine(cpu);
                        break;
                    }

                    case Operation.BRK:
                    {
                        break;
                    }

                    case Operation.RTI:
                    {
                        ReturnInterrupt(cpu);
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
            }

            private void Jump(CPU cpu, OpCode opcode, byte firstOperand, byte secondOperand, IMemory memory)
            {
                byte lowByte;
                byte highByte;

                switch (opcode.AddressMode)
                {
                    case AddressMode.Absolute:
                    {
                        lowByte = firstOperand;
                        highByte = secondOperand;
                        break;
                    }

                    case AddressMode.Indirect:
                    {
                        ushort indirectLowAddress = (ushort) (firstOperand + (secondOperand << 8));
                        ushort indirectHighAddress = (ushort) (((firstOperand + 1) % 256) + (secondOperand << 8));
                        lowByte = memory.Read(indirectLowAddress);
                        highByte = memory.Read(indirectHighAddress);
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} JMP does not handle {opcode.AddressMode}");
                }

                ushort address = (ushort) (lowByte + (highByte << 8));
                cpu.InstructionPointer = address; 
            }
            
            private void JumpSubroutine(CPU cpu, OpCode opcode, byte firstOperand, byte secondOperand, IMemory memory)
            {
                ushort returnAddress = cpu.InstructionPointer.Plus(opcode.Bytes-1);
                byte returnHighByte = (byte) (returnAddress >> 8);
                byte returnLowByte = (byte) (returnAddress % 256);
                
                cpu.Push(returnHighByte);
                cpu.Push(returnLowByte);

                ushort jumpAddress = (ushort) (firstOperand + (secondOperand << 8));
                cpu.InstructionPointer = jumpAddress;
            }
            
            private void ReturnSubroutine(CPU cpu)
            {
                byte low = cpu.Pop();
                byte high = cpu.Pop();

                cpu.InstructionPointer = (ushort) (low + (high << 8));
                cpu.InstructionPointer += 1;
            }
            
            private void ReturnInterrupt(CPU cpu)
            {
                byte statusByte = cpu.Pop();
                byte low = cpu.Pop();
                byte high = cpu.Pop();
                
                cpu.InstructionPointer = (ushort) (low + (high << 8));
                cpu.Status = (StatusFlags) statusByte;
            }
        }
    }
}