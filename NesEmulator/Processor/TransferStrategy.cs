using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class TransferStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte valueTransferred = TransferRegister(cpu, opcode);

                if (opcode.AffectsFlags != StatusFlags.None)
                {
                    SetFlags(valueTransferred, cpu);
                }
            }

            private byte TransferRegister(CPU cpu, OpCode opcode)
            {
                byte value;

                switch (opcode.Operation)
                {
                    case Operation.TAX:
                    {
                        value = cpu.Accumulator;
                        cpu.IndexX = value;
                        break;
                    }
                        
                    case Operation.TAY:
                    {
                        value = cpu.Accumulator;
                        cpu.IndexY = value;
                        break;
                    }
                    
                    case Operation.TXA:
                    {
                        value = cpu.IndexX;
                        cpu.Accumulator = value;
                        break;
                    }

                    case Operation.TYA:
                    {
                        value = cpu.IndexY;
                        cpu.Accumulator = value;
                        break;
                    }

                    case Operation.TSX:
                    {
                        value = (byte)(cpu.StackPointer % 256);
                        cpu.IndexX = value;
                        break;
                    }
                    
                    case Operation.TXS:
                    {
                        value = 0; // Does not affect flags
                        cpu.StackPointer = (ushort)(0x0100 + cpu.IndexX);
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle Operation {opcode.Operation}");
                }

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