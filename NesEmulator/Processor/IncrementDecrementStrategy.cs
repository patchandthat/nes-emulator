﻿using System;
using NesEmulator.Extensions;
using NesEmulator.Memory;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class IncrementDecrementStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                var resultValue = IncrementDecrement(cpu, opcode, firstOperand, memoryBus);

                SetFlags(resultValue, cpu);
            }

            private byte IncrementDecrement(CPU cpu, OpCode opcode, byte firstOperand, IMemoryBus memoryBus)
            {
                byte updatedValue;

                switch (opcode.Operation)
                {
                    case Operation.INC:
                    {
                        updatedValue = UpdateMemory(cpu, opcode.AddressMode, firstOperand, memoryBus, Increment);
                        break;
                    }

                    case Operation.INX:
                    {
                        updatedValue = ++cpu.IndexX;
                        break;
                    }

                    case Operation.INY:
                    {
                        updatedValue = ++cpu.IndexY;
                        break;
                    }

                    case Operation.DEC:
                    {
                        updatedValue = UpdateMemory(cpu, opcode.AddressMode, firstOperand, memoryBus, Decrement);
                        break;
                    }

                    case Operation.DEX:
                    {
                        updatedValue = --cpu.IndexX;
                        break;
                    }

                    case Operation.DEY:
                    {
                        updatedValue = --cpu.IndexY;
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {opcode.Operation}");
                }

                return updatedValue;
            }

            private byte Increment(byte source)
            {
                return (byte) ((source + 1) % 256);
            }

            private byte Decrement(byte source)
            {
                unchecked
                {
                    return (byte) (source - 1);
                }
            }

            private byte UpdateMemory(
                CPU cpu,
                AddressMode addressMode,
                byte firstOperand,
                IMemoryBus memoryBus,
                Func<byte, byte> modifyFunc)
            {
                ushort address;
                switch (addressMode)
                {
                    case AddressMode.ZeroPage:
                    {
                        address = firstOperand;
                        break;
                    }

                    case AddressMode.ZeroPageX:
                    {
                        address = (byte) ((firstOperand + cpu.IndexX) % 256);
                        break;
                    }

                    case AddressMode.Absolute:
                    {
                        address = (ushort) ((memoryBus.Read(cpu.InstructionPointer.Plus(2)) << 8) + firstOperand);
                        break;
                    }

                    case AddressMode.AbsoluteX:
                    {
                        address = (ushort) ((memoryBus.Read(cpu.InstructionPointer.Plus(2)) << 8) + firstOperand);
                        address += cpu.IndexX;
                        break;
                    }

                    default:
                        throw new NotSupportedException($"{GetType().FullName} does not handle {addressMode}");
                }

                var value = memoryBus.Read(address);
                value = modifyFunc(value);
                memoryBus.Write(address, value);
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