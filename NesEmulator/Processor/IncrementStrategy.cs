using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class IncrementStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte incrementedValue = Increment(cpu, opcode, memory);
                
                SetFlags(incrementedValue, cpu);
            }

            private byte Increment(CPU cpu, OpCode opcode, IMemory memory)
            {
                byte incrementedValue;
                
                switch (opcode.Operation)
                {
                    case Operation.INC:
                    {
                        throw new NotImplementedException();
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