﻿using System;
using NesEmulator.Extensions;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class BranchStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                bool shouldBranch = TestBranchCondition(opcode, cpu);

                if (shouldBranch)
                {
                    int cyclePenalty = 1;

                    ushort targetAddr = cpu.InstructionPointer.Plus((sbyte)firstOperand);
                    ushort targetAddrWithBaseBytes = (ushort)(targetAddr + opcode.Bytes);
                    byte currentPage = (byte)(cpu.InstructionPointer >> 8);
                    byte targetPage = (byte)(targetAddrWithBaseBytes >> 8);
                    
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
                        throw new NotSupportedException($"{this.GetType().FullName} does not handle {opcode.Operation}");
                }
            }
        }
    }
}