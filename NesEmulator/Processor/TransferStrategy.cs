using System;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class TransferStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                var valueTransferred = TransferRegister(cpu, opcode);

                if (opcode.AffectsFlags != StatusFlags.None) SetFlags(valueTransferred, cpu);
            }

            private byte TransferRegister(CPU cpu, OpCode opcode)
            {
                byte value;

                switch (opcode.Operation)
                {
                    case Operation.TAX:
                    {
                        value = cpu.Accumulator;
                        cpu.IndexX = value;
                        break;
                    }

                    case Operation.TAY:
                    {
                        value = cpu.Accumulator;
                        cpu.IndexY = value;
                        break;
                    }

                    case Operation.TXA:
                    {
                        value = cpu.IndexX;
                        cpu.Accumulator = value;
                        break;
                    }

                    case Operation.TYA:
                    {
                        value = cpu.IndexY;
                        cpu.Accumulator = value;
                        break;
                    }

                    case Operation.TSX:
                    {
                        value = (byte) (cpu.StackPointer % 256);
                        cpu.IndexX = value;
                        break;
                    }

                    case Operation.TXS:
                    {
                        value = 0; // Does not affect flags
                        cpu.StackPointer = (ushort) (0x0100 + cpu.IndexX);
                        break;
                    }

                    default:
                        throw new NotSupportedException(
                            $"{GetType().FullName} does not handle Operation {opcode.Operation}");
                }

                return value;
            }

            private void SetFlags(byte value, CPU cpu)
            {
                cpu.SetFlagState(StatusFlags.Zero, value == 0x0);
                cpu.SetFlagState(StatusFlags.Negative, value >= 0x80);
            }
        }
    }
}