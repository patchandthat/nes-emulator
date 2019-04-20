using System;
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
                        break;
                    }

                    case Operation.BRK:
                    {
                        break;
                    }

                    case Operation.RTI:
                    {
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
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

            private void Jump(CPU cpu, OpCode opcode, byte firstOperand, byte secondOperand, IMemory memory)
            {
                byte lowByte = 0x0;
                byte highByte = 0x0;

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
                // Base class intends to increment instruction pointer even further
                cpu.InstructionPointer = address; 
            }
        }
    }
}