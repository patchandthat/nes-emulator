using System.Collections;
using System.Collections.Generic;

namespace NesEmulator
{
    internal sealed class OpcodeDefinitions : IReadOnlyDictionary<byte, OpCode>
    {
        private readonly Dictionary<byte, OpCode> _inner;

        public OpcodeDefinitions()
        {
            _inner = new Dictionary<byte, OpCode>
            {
                {
                    0x69,
                    new OpCode(0x69,
                        Operation.ADC,
                        AddressMode.Immediate,
                        bytes: 2,
                        cycles: 2,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x65,
                    new OpCode(0x65,
                        Operation.ADC,
                        AddressMode.ZeroPage,
                        bytes: 2,
                        cycles: 3,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x75,
                    new OpCode(0x75,
                        Operation.ADC,
                        AddressMode.ZeroPageX,
                        bytes: 2,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x6D,
                    new OpCode(0x6D,
                        Operation.ADC,
                        AddressMode.Absolute,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x7D,
                    new OpCode(0x7D,
                        Operation.ADC,
                        AddressMode.AbsoluteX,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x79,
                    new OpCode(0x79,
                        Operation.ADC,
                        AddressMode.AbsoluteY,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x61,
                    new OpCode(0x61,
                        Operation.ADC,
                        AddressMode.IndirectX,
                        bytes: 2,
                        cycles: 6,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x71,
                    new OpCode(0x71,
                        Operation.ADC,
                        AddressMode.IndirectY,
                        bytes: 2,
                        cycles: 5,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x29,
                    new OpCode(0x29,
                        Operation.AND,
                        AddressMode.Immediate,
                        2, 
                        2,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x25,
                    new OpCode(0x25,
                        Operation.AND,
                        AddressMode.ZeroPage,
                        2, 
                        3,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x35,
                    new OpCode(0x35,
                        Operation.AND,
                        AddressMode.ZeroPageX,
                        2, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x2D,
                    new OpCode(0x2D,
                        Operation.AND,
                        AddressMode.Absolute,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x3D,
                    new OpCode(0x3D,
                        Operation.AND,
                        AddressMode.AbsoluteX,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x39,
                    new OpCode(0x39,
                        Operation.AND,
                        AddressMode.AbsoluteY,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x21,
                    new OpCode(0x21,
                        Operation.AND,
                        AddressMode.IndirectX,
                        2, 
                        6,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x31,
                    new OpCode(0x31,
                        Operation.AND,
                        AddressMode.IndirectY,
                        2, 
                        5,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xA9,
                    new OpCode(0xA9,
                        Operation.LDA,
                        AddressMode.Immediate,
                        2, 
                        2,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xA5,
                    new OpCode(0xA5,
                        Operation.LDA,
                        AddressMode.ZeroPage,
                        2, 
                        3,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xB5,
                    new OpCode(0xB5,
                        Operation.LDA,
                        AddressMode.ZeroPageX,
                        2, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xAD,
                    new OpCode(0xAD,
                        Operation.LDA,
                        AddressMode.Absolute,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xBD,
                    new OpCode(0xBD,
                        Operation.LDA,
                        AddressMode.AbsoluteX,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xB9,
                    new OpCode(0xB9,
                        Operation.LDA,
                        AddressMode.AbsoluteY,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xA1,
                    new OpCode(0xA1,
                        Operation.LDA,
                        AddressMode.IndirectX,
                        2, 
                        6,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xB1,
                    new OpCode(0xB1,
                        Operation.LDA,
                        AddressMode.IndirectY,
                        2, 
                        5,
                        StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0x0A,
                    new OpCode(0x0A,
                        Operation.ASL,
                        AddressMode.Accumulator,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry)
                },
                {
                    0x06,
                    new OpCode(0x06,
                        Operation.ASL,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry)
                },
                {
                    0x16,
                    new OpCode(0x16,
                        Operation.ASL,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry)
                },
                {
                    0x0E,
                    new OpCode(0x0E,
                        Operation.ASL,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry)
                },
                {
                    0x1E,
                    new OpCode(0x1E,
                        Operation.ASL,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry)
                },
                {
                    0x90,
                    new OpCode(0x90,
                        Operation.BCC,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0xB0,
                    new OpCode(0xB0,
                        Operation.BCS,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0xF0,
                    new OpCode(0xF0,
                        Operation.BEQ,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0x30,
                    new OpCode(0x30,
                        Operation.BMI,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0xD0,
                    new OpCode(0xD0,
                        Operation.BNE,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0x70,
                    new OpCode(0x70,
                        Operation.BVS,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0x50,
                    new OpCode(0x50,
                        Operation.BVC,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0x10,
                    new OpCode(0x10,
                        Operation.BPL,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None)
                },
                {
                    0x24,
                    new OpCode(0x24,
                        Operation.BIT,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow)
                },
                {
                    0x2C,
                    new OpCode(0x2C,
                        Operation.BIT,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow)
                },
                {
                    0x00,
                    new OpCode(0x00,
                        Operation.BRK,
                        AddressMode.Implicit,
                        1,
                        7,
                        StatusFlags.Bit4 | StatusFlags.Bit5)
                },
                {
                    0x18,
                    new OpCode(0x18,
                        Operation.CLC,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Carry)
                },
                {
                    0xD8,
                    new OpCode(0xD8,
                        Operation.CLD,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Decimal)
                },
                {
                    0x58,
                    new OpCode(0x58,
                        Operation.CLI,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.InterruptDisable)
                },
                {
                    0xB8,
                    new OpCode(0xB8,
                        Operation.CLV,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Overflow)
                },
                {
                    0xC9,
                    new OpCode(0xC9,
                        Operation.CMP,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xC5,
                    new OpCode(0xC5,
                        Operation.CMP,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xD5,
                    new OpCode(0xD5,
                        Operation.CMP,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xCD,
                    new OpCode(0xCD,
                        Operation.CMP,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xDD,
                    new OpCode(0xDD,
                        Operation.CMP,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xD9,
                    new OpCode(0xD9,
                        Operation.CMP,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xC1,
                    new OpCode(0xC1,
                        Operation.CMP,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xD1,
                    new OpCode(0xD1,
                        Operation.CMP,
                        AddressMode.IndirectY,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xE0,
                    new OpCode(0xE0,
                        Operation.CPX,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xE4,
                    new OpCode(0xE4,
                        Operation.CPX,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xEC,
                    new OpCode(0xEC,
                        Operation.CPX,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xC0,
                    new OpCode(0xC0,
                        Operation.CPY,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xC4,
                    new OpCode(0xC4,
                        Operation.CPY,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
                {
                    0xCC,
                    new OpCode(0xCC,
                        Operation.CPY,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative)
                },
            };
        }

        public IEnumerator<KeyValuePair<byte, OpCode>> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _inner).GetEnumerator();
        }

        public int Count => _inner.Count;

        public bool ContainsKey(byte key)
        {
            return _inner.ContainsKey(key);
        }

        public bool TryGetValue(byte key, out OpCode value)
        {
            return _inner.TryGetValue(key, out value);
        }

        public OpCode this[byte key] => _inner[key];

        public IEnumerable<byte> Keys => ((IReadOnlyDictionary<byte, OpCode>) _inner).Keys;

        public IEnumerable<OpCode> Values => ((IReadOnlyDictionary<byte, OpCode>) _inner).Values;
    }
}