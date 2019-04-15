using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class CompareStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte comparisonValue = GetComparisonValue(cpu, opcode, firstOperand, memory);

                byte registerValue;
                switch (opcode.Operation)
                {
                    case Operation.CMP:
                    {
                        registerValue = cpu.Accumulator;
                        break;
                    }
                    
                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }

                unchecked
                {
                    byte result = (byte) (registerValue - comparisonValue);
                    cpu.SetFlagState(StatusFlags.Carry, registerValue >= comparisonValue);
                    cpu.SetFlagState(StatusFlags.Zero, registerValue == comparisonValue);
                    cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) > 0);
                }
            }

            

            private byte GetComparisonValue(CPU cpu, OpCode opcode, byte operand, IMemory memory)
            {
                switch (opcode.AddressMode)
                {
                    case AddressMode.Immediate:
                    {
                        return operand;
                    }
                    
                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
                }
            }
        }
    }
}