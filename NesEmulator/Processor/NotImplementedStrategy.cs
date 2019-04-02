using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal class NotImplementedStrategy : OperationExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                throw new NotImplementedException();
            }
        }
    }
}