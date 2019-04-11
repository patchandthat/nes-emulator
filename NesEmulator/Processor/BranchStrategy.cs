using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BranchStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                throw new NotSupportedException($"{this.GetType().FullName} does not handle {opcode.Operation}");
            }
        }
    }
}