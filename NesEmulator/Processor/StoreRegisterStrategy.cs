namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class StoreRegisterStrategy : OperationExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                ushort address = ResolveTargetAddress(opcode, firstOperand, cpu, memory);
                
                memory.Write(address, cpu.Accumulator);
            }

            private ushort ResolveTargetAddress(OpCode opcode, byte operand, CPU cpu, IMemory memory)
            {
                return operand;
            }
        }
    }
}