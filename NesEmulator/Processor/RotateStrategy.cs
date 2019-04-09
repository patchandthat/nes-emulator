using System;

namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class RotateStrategy : ExecutionStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte toRotate = cpu.Accumulator;
                bool carryWasSet = cpu.Status.HasFlagFast(StatusFlags.Carry);
                bool bit7WasHigh = (toRotate & 0x80) > 0;

                byte result = (byte) ((toRotate << 1) % 256);
                if (carryWasSet) result |= 0x1;
                cpu.Accumulator = result;
                
                cpu.SetFlagState(StatusFlags.Carry, bit7WasHigh);
                
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) > 0);
                cpu.SetFlagState(StatusFlags.Zero, result == 0x0);
            }
        }
    }
}

//                switch (opcode.AddressMode)
//                {
//                    default:
//                        throw new NotSupportedException($"{this.GetType().FullName} does not handle AddressMode {opcode.AddressMode}");
//                }
                
//                switch (opcode.Operation)
//                {                    
//                   default:
//                        throw new NotSupportedException($"{this.GetType().FullName} does not handle Operation {opcode.Operation}");
//                }