namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class NoOperationStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                // Intentionally empty
            }
        }
    }
}