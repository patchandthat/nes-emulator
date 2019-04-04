using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        internal abstract class ExecutionStrategyBase
        {
            public void Execute(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                ExecuteImpl(cpu, opcode, firstOperand, memory);

                cpu.ElapsedCycles += opcode.Cycles;
                cpu.InstructionPointer += opcode.Bytes;
            }

            protected abstract void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory);
        }
    }
}