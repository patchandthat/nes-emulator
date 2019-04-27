using System;
using NesEmulator.Extensions;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BranchStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                var shouldBranch = TestBranchCondition(opcode, cpu);

                if (shouldBranch)
                {
                    var cyclePenalty = 1;

                    var targetAddr = cpu.InstructionPointer.Plus((sbyte) firstOperand);
                    var targetAddrWithBaseBytes = (ushort) (targetAddr + opcode.Bytes);
                    var currentPage = (byte) (cpu.InstructionPointer >> 8);
                    var targetPage = (byte) (targetAddrWithBaseBytes >> 8);

                    if (currentPage != targetPage)
                        cyclePenalty += 1;

                    cpu.InstructionPointer = targetAddr;
                    cpu.ElapsedCycles += cyclePenalty;
                }
            }

            private bool TestBranchCondition(OpCode opcode, CPU cpu)
            {
                switch (opcode.Operation)
                {
                    case Operation.BCC:
                    {
                        return !cpu.Status.HasFlagFast(StatusFlags.Carry);
                    }

                    case Operation.BCS:
                    {
                        return cpu.Status.HasFlagFast(StatusFlags.Carry);
                    }

                    case Operation.BEQ:
                    {
                        return cpu.Status.HasFlagFast(StatusFlags.Zero);
                    }

                    case Operation.BMI:
                    {
                        return cpu.Status.HasFlagFast(StatusFlags.Negative);
                    }

                    case Operation.BNE:
                    {
                        return !cpu.Status.HasFlagFast(StatusFlags.Zero);
                    }

                    case Operation.BPL:
                    {
                        return !cpu.Status.HasFlagFast(StatusFlags.Negative);
                    }

                    case Operation.BVC:
                    {
                        return !cpu.Status.HasFlagFast(StatusFlags.Overflow);
                    }

                    case Operation.BVS:
                    {
                        return cpu.Status.HasFlagFast(StatusFlags.Overflow);
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }
            }
        }
    }
}