using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal abstract class AutoIncrementInstructionPointerStrategyBase : ExecutionStrategyBase
        {
            public override void Execute(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                base.Execute(cpu, opcode, firstOperand, memory);

                cpu.InstructionPointer += opcode.Bytes;
            }
        }
    }
}