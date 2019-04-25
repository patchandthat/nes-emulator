using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BitTestStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte operand = GetOperand(cpu, opcode.AddressMode, firstOperand, memory);
                int andResult = cpu.Accumulator & operand;
                
                cpu.SetFlagState(StatusFlags.Zero, andResult == 0x0);
                cpu.SetFlagState(StatusFlags.Overflow, (operand & 0x40) != 0);
                cpu.SetFlagState(StatusFlags.Negative, (operand & 0x80) != 0);
            }

            private byte GetOperand(CPU cpu, AddressMode addressMode, byte firstOperand, IMemory memory)
            {
                switch (addressMode)
                {
                    case AddressMode.ZeroPage:
                    {
                        return memory.Read(firstOperand);
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memory.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + firstOperand);
                        return memory.Read(address);
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {addressMode}");
                }
            }
        }
    }
}