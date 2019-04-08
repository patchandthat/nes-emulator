using System;
using NesEmulator.Extensions;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class IncrementStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte incrementedValue = Increment(cpu, opcode, firstOperand, memory);
                
                SetFlags(incrementedValue, cpu);
            }

            private byte Increment(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte incrementedValue;
                
                switch (opcode.Operation)
                {
                    case Operation.INC:
                    {
                        incrementedValue = IncrementMemory(cpu, opcode.AddressMode, firstOperand, memory);
                        break;
                    }
                    
                    case Operation.INX:
                    {
                        incrementedValue = ++cpu.IndexX;
                        break;
                    }
                    
                    case Operation.INY:
                    {
                        incrementedValue = ++cpu.IndexY;
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle {opcode.Operation}");
                }

                return incrementedValue;
            }

            private byte IncrementMemory(CPU cpu, AddressMode addressMode, byte firstOperand, IMemory memory)
            {
                ushort address;
                switch (addressMode)
                {
                    case AddressMode.ZeroPage:
                    {
                        address = firstOperand;
                        break;
                    }

                    case AddressMode.ZeroPageX:
                    {
                        address = (byte) ((firstOperand + cpu.IndexX) % 256);
                        break;
                    }

                    case AddressMode.Absolute:
                    {
                        address = (ushort) ((memory.Read(cpu.InstructionPointer.Plus(2)) << 8) + firstOperand);
                        break;
                    }
                    
                    case AddressMode.AbsoluteX:
                    {
                        address = (ushort) ((memory.Read(cpu.InstructionPointer.Plus(2)) << 8) + firstOperand);
                        address += cpu.IndexX;
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle {addressMode}");
                }
                
                byte value = memory.Read(address);
                value += 1;
                memory.Write(address, value);
                return value;
            }

            private void SetFlags(byte value, CPU cpu)
            {
                if (value == 0x0)
                {
                    cpu.Status |= (StatusFlags.Zero);
                }
                else
                {
                    cpu.Status &= ~StatusFlags.Zero;
                }

                if (value >= 0x80)
                {
                    cpu.Status |= StatusFlags.Negative;
                }
                else
                {
                    cpu.Status &= ~StatusFlags.Negative;
                }
            }
        }
    }
}