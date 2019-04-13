using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal class NotImplementedStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                throw new NotImplementedException($"No implementation exists for opcode {opcode.Value:X2}");
            }
        }
    }
}