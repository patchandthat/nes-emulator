﻿using FakeItEasy;
using NesEmulator.Memory;
using NesEmulator.Processor;

namespace NesEmulator.UnitTests.Helpers
{
    internal static class CpuTestExtensions
    {
        /// <summary>
        ///     Unit test helper to set up test preconditions, LDA Immediate.
        /// </summary>
        /// <param name="cpu">System under test</param>
        /// <param name="value">Value to load</param>
        /// <param name="memoryBus">FakeItEasy faked IMemory</param>
        /// <returns>Updated CPU instance</returns>
        public static CPU LDA(this CPU cpu, byte value, IMemoryBus memoryBus)
        {
            var op = GetOp(Operation.LDA);

            var address = cpu.InstructionPointer;

            A.CallTo(() => memoryBus.Read(address)).Returns(op.Value);
            A.CallTo(() => memoryBus.Read((ushort) (address + 1))).Returns(value);

            cpu.Step();
            Fake.ClearRecordedCalls(memoryBus);

            return cpu;
        }

        /// <summary>
        ///     Unit test helper to set up test preconditions, LDX Immediate.
        ///     /// Inserts the op at the current instruction pointer location and steps the CPU
        /// </summary>
        /// <param name="cpu">System under test</param>
        /// <param name="value">Value to load</param>
        /// <param name="memoryBus">FakeItEasy faked IMemory</param>
        /// <returns>Updated CPU instance</returns>
        public static CPU LDX(this CPU cpu, byte value, IMemoryBus memoryBus)
        {
            var op = GetOp(Operation.LDX);

            var address = cpu.InstructionPointer;

            A.CallTo(() => memoryBus.Read(address)).Returns(op.Value);
            A.CallTo(() => memoryBus.Read((ushort) (address + 1))).Returns(value);

            cpu.Step();
            Fake.ClearRecordedCalls(memoryBus);

            return cpu;
        }

        /// <summary>
        ///     Unit test helper to set up test preconditions. LDY Immediate.
        ///     Inserts the op at the current instruction pointer location and steps the CPU
        /// </summary>
        /// <param name="cpu">System under test</param>
        /// <param name="value">Value to load</param>
        /// ///
        /// <param name="memoryBus">FakeItEasy faked IMemory</param>
        /// <returns>Updated CPU instance</returns>
        public static CPU LDY(this CPU cpu, byte value, IMemoryBus memoryBus)
        {
            var op = GetOp(Operation.LDY);

            var address = cpu.InstructionPointer;

            A.CallTo(() => memoryBus.Read(address)).Returns(op.Value);
            A.CallTo(() => memoryBus.Read((ushort) (address + 1))).Returns(value);

            cpu.Step();
            Fake.ClearRecordedCalls(memoryBus);

            return cpu;
        }

        /// <summary>
        ///     Unit test helper to set up test preconditions. NOP.
        ///     Inserts the op at the current instruction pointer location and steps the CPU
        /// </summary>
        /// <param name="cpu">System under test</param>
        /// <param name="memoryBus">FakeItEasy faked IMemory</param>
        /// <returns>Updated CPU instance</returns>
        public static CPU NOP(this CPU cpu, IMemoryBus memoryBus)
        {
            var op = GetOp(Operation.LDY);

            var address = cpu.InstructionPointer;

            A.CallTo(() => memoryBus.Read(address)).Returns(op.Value);

            cpu.Step();
            Fake.ClearRecordedCalls(memoryBus);

            return cpu;
        }

        private static OpCode GetOp(Operation op, AddressMode mode = AddressMode.Immediate)
        {
            return new OpCodes().FindOpcode(op, mode);
        }
    }
}