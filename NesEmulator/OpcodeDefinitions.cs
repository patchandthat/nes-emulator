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