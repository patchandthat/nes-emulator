using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class PushPopStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                switch (opcode.Operation)
                {
                    case Operation.PHA:
                    {
                        cpu.Push(cpu.Accumulator);
                        break;
                    }

                    case Operation.PHP:
                    {
                        cpu.Push((byte) cpu.Status);
                        break;
                    }

                    case Operation.PLA:
                    {
                        var value = cpu.Pop();
                        cpu.Accumulator = value;
                        cpu.SetFlagState(StatusFlags.Zero, value == 0);
                        cpu.SetFlagState(StatusFlags.Negative, (value & 0x80) != 0);
                        break;
                    }

                    case Operation.PLP:
                    {
                        var value = cpu.Pop();
                        cpu.Status = (StatusFlags) value;
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
            }
        }
    }
}