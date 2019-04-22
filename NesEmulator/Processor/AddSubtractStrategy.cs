namespace NesEmulator.Processor
{
    internal partial class CPU
    {
        public class AddSubtractStrategy : AutoIncrementInstructionPointerStrategyBase
        {
            protected override void ExecuteImpl(CPU cpu, OpCode opcode, byte firstOperand, IMemory memory)
            {
                byte operand = firstOperand;
                byte initialAccumulator = cpu.Accumulator;
                int result = initialAccumulator + operand;
                if (cpu.Status.HasFlagFast(StatusFlags.Carry)) result += 1;

                cpu.Accumulator = (byte)(result % 256);

                cpu.SetFlagState(StatusFlags.Carry, result >> 8 != 0);
                bool signWasFlipped = ((initialAccumulator ^ result) & (operand ^ result) & 0x80) != 0;
                cpu.SetFlagState(StatusFlags.Overflow, signWasFlipped);
                cpu.SetFlagState(StatusFlags.Zero, cpu.Accumulator == 0x0);
                cpu.SetFlagState(StatusFlags.Negative, (result & 0x80) != 0);
            }
        }
    }
}