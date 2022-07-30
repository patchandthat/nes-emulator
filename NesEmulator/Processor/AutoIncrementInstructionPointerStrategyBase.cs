using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal abstract class AutoIncrementInstructionPointerStrategyBase : ExecutionStrategyBase
        {
            public override void Execute(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                base.Execute(cpu, opcode, firstOperand, memoryBus);

                cpu.InstructionPointer += opcode.Bytes;
            }
        }
    }
}