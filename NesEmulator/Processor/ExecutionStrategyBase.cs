using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal abstract class ExecutionStrategyBase
        {
            public virtual void Execute(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                ExecuteImpl(cpu, opcode, firstOperand, memoryBus);

                cpu.ElapsedCycles += opcode.Cycles;
            }

            protected abstract void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus);
        }
    }
}