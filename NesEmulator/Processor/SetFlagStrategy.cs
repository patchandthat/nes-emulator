﻿using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class SetFlagStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                switch (opcode.Operation)
                {
                    case Operation.CLI:
                    case Operation.CLC:
                    case Operation.CLD:
                    case Operation.CLV:
                    {
                        cpu.ClearFlags(opcode.AffectsFlags);
                        break;
                    }

                    case Operation.SEI:
                    case Operation.SED:
                    case Operation.SEC:
                    {
                        cpu.SetFlags(opcode.AffectsFlags);
                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle Operation {opcode.Operation}");
                }
            }
        }
    }
}