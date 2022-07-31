using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using NesEmulator.Memory;
using NesEmulator.Processor;

namespace NesEmulator
{
    public class DebugInfo
    {
        private readonly CPU _cpu;
        private readonly IDebugRead _bus;
        private List<DisassemblyRow> _disassemblyRows = new List<DisassemblyRow>();
        
        // Properties to draw CPU state and disassembly info
        internal DebugInfo(CPU cpu, IDebugRead memoryBus)
        {
            _cpu = cpu;
            _bus = memoryBus;
        }

        public byte Accumulator => _cpu.Accumulator;

        public byte IndexX => _cpu.IndexX;

        public byte IndexY => _cpu.IndexY;

        public StatusFlags Status => _cpu.Status;

        public ushort InstructionPointer => _cpu.InstructionPointer;

        public ushort StackPointer => _cpu.StackPointer;

        public long ElapsedCycles => _cpu.ElapsedCycles;

        public bool IsPowerOn => _cpu.IsPowerOn;

        public List<DisassemblyRow> Disassembly => _disassemblyRows;

        internal void Disassemble()
        {
            _disassemblyRows.Clear();
            ushort start = MemoryMap.PrgRomLowerBank;
            ushort end = MemoryMap.NonMaskableInterruptVector;
            ushort address = start;

            try
            {
                while (address < end)
                {
                    byte opcode = _bus.Peek(address);
                    OpCode op = _cpu.LookupOpcode(opcode);

                    DisassemblyRow d = new DisassemblyRow()
                    {
                        Address = address,
                        Opcode = opcode,
                        Mnemonic = Enum.GetName(typeof(Operation), op.Operation),
                        AddressMode = Enum.GetName(typeof(AddressMode), op.AddressMode),
                        OperandByteCount = op.Bytes - 1,
                    };

                    for (byte b = 0; b < d.OperandByteCount; b++)
                    {
                        const byte offset = 1;
                        d.OperandBytes[b] = _bus.Peek((ushort)(address + offset + b));
                    }
                
                    _disassemblyRows.Add(d);

                    address += op.Bytes;
                }
            }
            catch (Exception e)
            {
                // Intentionally empty
                // Encountered undocumented opcode
                // Stop disassembling for now
                
                // Todo: remove this when undocumented opcodes are implemented
            }
        }
    }

    public class DisassemblyRow
    {
        public ushort Address { get; internal set; }
        public byte Opcode { get; internal set; }
        public string Mnemonic { get; internal set; }
        public string AddressMode { get; internal set; }
        public int OperandByteCount { get; internal set; }
        public byte[] OperandBytes { get; internal set; }

        public DisassemblyRow()
        {
            OperandBytes = new byte[3];
        }
    }
}