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