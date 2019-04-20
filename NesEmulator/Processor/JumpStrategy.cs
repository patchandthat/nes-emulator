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
                switch (opcode.Operation)
                {
                    case Operation.JMP:
                    {
                        byte secondOperand = memory.Read(cpu.InstructionPointer.Plus(2));
                        Jump(cpu, opcode, firstOperand, secondOperand, memory);
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
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
                cpu.InstructionPointer = address.Plus(-opcode.Bytes); 
            }
        }
    }
}