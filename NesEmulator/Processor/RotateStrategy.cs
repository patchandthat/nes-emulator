using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class RotateStrategy : ExecutionStrategyBase
        {           
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte operand, IMemory memory)
            {
                byte toRotate = GetValue(opcode.AddressMode, cpu, memory, operand);
                
                bool carryWasSet = cpu.Status.HasFlagFast(StatusFlags.Carry);
                bool bit7WasHigh = (toRotate & 0x80) > 0;
                
                var result = Rotate(toRotate, carryWasSet);

                WriteResult(opcode.AddressMode, cpu, memory, operand, result);
                
                cpu.SetFlagState(StatusFlags.Carry, bit7WasHigh);
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) > 0);
                cpu.SetFlagState(StatusFlags.Zero, result == 0x0);
            }

            private byte GetValue(AddressMode addressMode, CPU cpu, IMemory memory, byte operand)
            {
                switch (addressMode)
                {
                    case AddressMode.Accumulator:
                    {
                        return cpu.Accumulator;
                    }

                    case AddressMode.ZeroPage:
                    {
                        return memory.Read(operand);
                    }
                    
                    case AddressMode.ZeroPageX:
                    {
                        return memory.Read((byte)((operand + cpu.IndexX)%256));
                    }

                    case AddressMode.Absolute:
                    {
                        return 0;
                    }
                    
                    case AddressMode.AbsoluteX:
                    {
                        return 0;
                    }
                    
                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle AddressMode {addressMode}");
                }
            }
            
            private static byte Rotate(byte toRotate, bool carryWasSet)
            {
                byte result = (byte) ((toRotate << 1) % 256);
                if (carryWasSet) result |= 0x1;
                
                return result;
            }

            private void WriteResult(AddressMode addressMode, CPU cpu, IMemory memory, byte operand, byte result)
            {
                switch (addressMode)
                {
                    case AddressMode.Accumulator:
                    {
                        cpu.Accumulator = result;
                        break;
                    }
                    
                    case AddressMode.ZeroPage:
                    {
                        memory.Write(operand, result);
                        break;
                    }
                    
                    case AddressMode.ZeroPageX:
                    {
                        memory.Write((byte)((operand + cpu.IndexX)%256), result);
                        break;
                    }
                    
                    case AddressMode.Absolute:
                    {
                        break;
                    }
                    
                    case AddressMode.AbsoluteX:
                    {
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle AddressMode {addressMode}");
                }
            }
        }
    }
}