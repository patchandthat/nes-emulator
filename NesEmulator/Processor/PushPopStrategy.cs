using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class PushPopStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
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
                        cpu.Push((byte) (cpu.Status | StatusFlags.Bit4) );
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
                        // Bit 4 doesn't actually exist, it's just raised in the value pushed to the stack
                        value &= 0xEF;
                        // Nintendulator has this bit raised. Todo: Understand why.
                        value |= 0x20;
                        var statusFlags = (StatusFlags) value; 
                        cpu.Status = statusFlags;
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
            }
        }
    }
}