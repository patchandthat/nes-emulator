using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BitTestStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                byte operand = GetOperand(cpu, opcode.AddressMode, firstOperand, memoryBus);
                int andResult = cpu.Accumulator & operand;
                
                cpu.SetFlagState(StatusFlags.Zero, andResult == 0x0);
                cpu.SetFlagState(StatusFlags.Overflow, (operand & 0x40) != 0);
                cpu.SetFlagState(StatusFlags.Negative, (operand & 0x80) != 0);
            }

            private byte GetOperand(CPU cpu, AddressMode addressMode, byte firstOperand, IMemoryBus memoryBus)
            {
                switch (addressMode)
                {
                    case AddressMode.ZeroPage:
                    {
                        return memoryBus.Read(firstOperand);
                    }

                    case AddressMode.Absolute:
                    {
                        var highByte = memoryBus.Read((ushort) (cpu.InstructionPointer + 2));
                        var address = (ushort) ((highByte << 8) + firstOperand);
                        return memoryBus.Read(address);
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {addressMode}");
                }
            }
        }
    }
}