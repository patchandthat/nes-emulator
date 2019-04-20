using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class JumpStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
            }
        }
    }
}