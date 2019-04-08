using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NesEmulator.Processor
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
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x65,
                    new OpCode(0x65,
                        Operation.ADC,
                        AddressMode.ZeroPage,
                        bytes: 2,
                        cycles: 3,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x75,
                    new OpCode(0x75,
                        Operation.ADC,
                        AddressMode.ZeroPageX,
                        bytes: 2,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x6D,
                    new OpCode(0x6D,
                        Operation.ADC,
                        AddressMode.Absolute,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x7D,
                    new OpCode(0x7D,
                        Operation.ADC,
                        AddressMode.AbsoluteX,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x79,
                    new OpCode(0x79,
                        Operation.ADC,
                        AddressMode.AbsoluteY,
                        bytes: 3,
                        cycles: 4,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x61,
                    new OpCode(0x61,
                        Operation.ADC,
                        AddressMode.IndirectX,
                        bytes: 2,
                        cycles: 6,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x71,
                    new OpCode(0x71,
                        Operation.ADC,
                        AddressMode.IndirectY,
                        bytes: 2,
                        cycles: 5,
                        affectsFlags: StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x29,
                    new OpCode(0x29,
                        Operation.AND,
                        AddressMode.Immediate,
                        2, 
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x25,
                    new OpCode(0x25,
                        Operation.AND,
                        AddressMode.ZeroPage,
                        2, 
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x35,
                    new OpCode(0x35,
                        Operation.AND,
                        AddressMode.ZeroPageX,
                        2, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x2D,
                    new OpCode(0x2D,
                        Operation.AND,
                        AddressMode.Absolute,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x3D,
                    new OpCode(0x3D,
                        Operation.AND,
                        AddressMode.AbsoluteX,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x39,
                    new OpCode(0x39,
                        Operation.AND,
                        AddressMode.AbsoluteY,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x21,
                    new OpCode(0x21,
                        Operation.AND,
                        AddressMode.IndirectX,
                        2, 
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x31,
                    new OpCode(0x31,
                        Operation.AND,
                        AddressMode.IndirectY,
                        2, 
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xA9,
                    new OpCode(0xA9,
                        Operation.LDA,
                        AddressMode.Immediate,
                        2, 
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xA5,
                    new OpCode(0xA5,
                        Operation.LDA,
                        AddressMode.ZeroPage,
                        2, 
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xB5,
                    new OpCode(0xB5,
                        Operation.LDA,
                        AddressMode.ZeroPageX,
                        2, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xAD,
                    new OpCode(0xAD,
                        Operation.LDA,
                        AddressMode.Absolute,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xBD,
                    new OpCode(0xBD,
                        Operation.LDA,
                        AddressMode.AbsoluteX,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xB9,
                    new OpCode(0xB9,
                        Operation.LDA,
                        AddressMode.AbsoluteY,
                        3, 
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xA1,
                    new OpCode(0xA1,
                        Operation.LDA,
                        AddressMode.IndirectX,
                        2, 
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xB1,
                    new OpCode(0xB1,
                        Operation.LDA,
                        AddressMode.IndirectY,
                        2, 
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0x0A,
                    new OpCode(0x0A,
                        Operation.ASL,
                        AddressMode.Accumulator,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x06,
                    new OpCode(0x06,
                        Operation.ASL,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x16,
                    new OpCode(0x16,
                        Operation.ASL,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x0E,
                    new OpCode(0x0E,
                        Operation.ASL,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x1E,
                    new OpCode(0x1E,
                        Operation.ASL,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x90,
                    new OpCode(0x90,
                        Operation.BCC,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xB0,
                    new OpCode(0xB0,
                        Operation.BCS,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xF0,
                    new OpCode(0xF0,
                        Operation.BEQ,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x30,
                    new OpCode(0x30,
                        Operation.BMI,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xD0,
                    new OpCode(0xD0,
                        Operation.BNE,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x70,
                    new OpCode(0x70,
                        Operation.BVS,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x50,
                    new OpCode(0x50,
                        Operation.BVC,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x10,
                    new OpCode(0x10,
                        Operation.BPL,
                        AddressMode.Relative,
                        2,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x24,
                    new OpCode(0x24,
                        Operation.BIT,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x2C,
                    new OpCode(0x2C,
                        Operation.BIT,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x00,
                    new OpCode(0x00,
                        Operation.BRK,
                        AddressMode.Implicit,
                        1,
                        7,
                        StatusFlags.Bit4 | StatusFlags.Bit5,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x18,
                    new OpCode(0x18,
                        Operation.CLC,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xD8,
                    new OpCode(0xD8,
                        Operation.CLD,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Decimal,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x58,
                    new OpCode(0x58,
                        Operation.CLI,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.InterruptDisable,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xB8,
                    new OpCode(0xB8,
                        Operation.CLV,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC9,
                    new OpCode(0xC9,
                        Operation.CMP,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC5,
                    new OpCode(0xC5,
                        Operation.CMP,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xD5,
                    new OpCode(0xD5,
                        Operation.CMP,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xCD,
                    new OpCode(0xCD,
                        Operation.CMP,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xDD,
                    new OpCode(0xDD,
                        Operation.CMP,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xD9,
                    new OpCode(0xD9,
                        Operation.CMP,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC1,
                    new OpCode(0xC1,
                        Operation.CMP,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xD1,
                    new OpCode(0xD1,
                        Operation.CMP,
                        AddressMode.IndirectY,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE0,
                    new OpCode(0xE0,
                        Operation.CPX,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE4,
                    new OpCode(0xE4,
                        Operation.CPX,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xEC,
                    new OpCode(0xEC,
                        Operation.CPX,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC0,
                    new OpCode(0xC0,
                        Operation.CPY,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC4,
                    new OpCode(0xC4,
                        Operation.CPY,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xCC,
                    new OpCode(0xCC,
                        Operation.CPY,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xC6,
                    new OpCode(0xC6,
                        Operation.DEC,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xD6,
                    new OpCode(0xD6,
                        Operation.DEC,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xCE,
                    new OpCode(0xCE,
                        Operation.DEC,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xDE,
                    new OpCode(0xDE,
                        Operation.DEC,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xCA,
                    new OpCode(0xCA,
                        Operation.DEX,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0x88,
                    new OpCode(0x88,
                        Operation.DEY,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0x49,
                    new OpCode(0x49,
                        Operation.EOR,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x45,
                    new OpCode(0x45,
                        Operation.EOR,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x55,
                    new OpCode(0x55,
                        Operation.EOR,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x4D,
                    new OpCode(0x4D,
                        Operation.EOR,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x5D,
                    new OpCode(0x5D,
                        Operation.EOR,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x59,
                    new OpCode(0x59,
                        Operation.EOR,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x41,
                    new OpCode(0x41,
                        Operation.EOR,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x51,
                    new OpCode(0x51,
                        Operation.EOR,
                        AddressMode.IndirectY,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE6,
                    new OpCode(0xE6,
                        Operation.INC,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xF6,
                    new OpCode(0xF6,
                        Operation.INC,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xEE,
                    new OpCode(0xEE,
                        Operation.INC,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xFE,
                    new OpCode(0xFE,
                        Operation.INC,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xE8,
                    new OpCode(0xE8,
                        Operation.INX,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0xC8,
                    new OpCode(0xC8,
                        Operation.INY,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.IncrementDecrementStrategy())
                },
                {
                    0x4C,
                    new OpCode(0x4C,
                        Operation.JMP,
                        AddressMode.Absolute,
                        3,
                        5,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x6C,
                    new OpCode(0x6C,
                        Operation.JMP,
                        AddressMode.Indirect,
                        3,
                        5,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x20,
                    new OpCode(0x20,
                        Operation.JSR,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xA2,
                    new OpCode(0xA2,
                        Operation.LDX,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xA6,
                    new OpCode(0xA6,
                        Operation.LDX,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xB6,
                    new OpCode(0xB6,
                        Operation.LDX,
                        AddressMode.ZeroPageY,
                        2,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xAE,
                    new OpCode(0xAE,
                        Operation.LDX,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xBE,
                    new OpCode(0xBE,
                        Operation.LDX,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xA0,
                    new OpCode(0xA0,
                        Operation.LDY,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xA4,
                    new OpCode(0xA4,
                        Operation.LDY,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xB4,
                    new OpCode(0xB4,
                        Operation.LDY,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xAC,
                    new OpCode(0xAC,
                        Operation.LDY,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0xBC,
                    new OpCode(0xBC,
                        Operation.LDY,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.LoadRegisterStrategy())
                },
                {
                    0x4A,
                    new OpCode(0x4A,
                        Operation.LSR,
                        AddressMode.Accumulator,
                        1,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x46,
                    new OpCode(0x46,
                        Operation.LSR,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x56,
                    new OpCode(0x56,
                        Operation.LSR,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x4E,
                    new OpCode(0x4E,
                        Operation.LSR,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x5E,
                    new OpCode(0x5E,
                        Operation.LSR,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xEA,
                    new OpCode(0xEA,
                        Operation.NOP,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x09,
                    new OpCode(0x09,
                        Operation.ORA,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x05,
                    new OpCode(0x05,
                        Operation.ORA,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x15,
                    new OpCode(0x15,
                        Operation.ORA,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x0D,
                    new OpCode(0x0D,
                        Operation.ORA,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x1D,
                    new OpCode(0x1D,
                        Operation.ORA,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x19,
                    new OpCode(0x19,
                        Operation.ORA,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x01,
                    new OpCode(0x01,
                        Operation.ORA,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x11,
                    new OpCode(0x11,
                        Operation.ORA,
                        AddressMode.IndirectY,
                        2,
                        5,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x48,
                    new OpCode(0x48,
                        Operation.PHA,
                        AddressMode.Implicit,
                        1,
                        3,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x08,
                    new OpCode(0x08,
                        Operation.PHP,
                        AddressMode.Implicit,
                        1,
                        3,
                        StatusFlags.Bit4 | StatusFlags.Bit5,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x28,
                    new OpCode(0x28,
                        Operation.PLP,
                        AddressMode.Implicit,
                        1,
                        4,
                        StatusFlags.All,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x68,
                    new OpCode(0x68,
                        Operation.PLA,
                        AddressMode.Implicit,
                        1,
                        4,
                        StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x2A,
                    new OpCode(0x2A,
                        Operation.ROL,
                        AddressMode.Accumulator,
                        1,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x26,
                    new OpCode(0x26,
                        Operation.ROL,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x36,
                    new OpCode(0x36,
                        Operation.ROL,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x2E,
                    new OpCode(0x2E,
                        Operation.ROL,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x3E,
                    new OpCode(0x3E,
                        Operation.ROL,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x6A,
                    new OpCode(0x6A,
                        Operation.ROR,
                        AddressMode.Accumulator,
                        1,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x66,
                    new OpCode(0x66,
                        Operation.ROR,
                        AddressMode.ZeroPage,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x76,
                    new OpCode(0x76,
                        Operation.ROR,
                        AddressMode.ZeroPageX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x6E,
                    new OpCode(0x6E,
                        Operation.ROR,
                        AddressMode.Absolute,
                        3,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x7E,
                    new OpCode(0x7E,
                        Operation.ROR,
                        AddressMode.AbsoluteX,
                        3,
                        7,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x40,
                    new OpCode(0x40,
                        Operation.RTI,
                        AddressMode.Implicit,
                        1,
                        6,
                        StatusFlags.All,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x60,
                    new OpCode(0x60,
                        Operation.RTS,
                        AddressMode.Implicit,
                        1,
                        6,
                        StatusFlags.None,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE9,
                    new OpCode(0xE9,
                        Operation.SBC,
                        AddressMode.Immediate,
                        2,
                        2,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE5,
                    new OpCode(0xE5,
                        Operation.SBC,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xF5,
                    new OpCode(0xF5,
                        Operation.SBC,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xED,
                    new OpCode(0xED,
                        Operation.SBC,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xFD,
                    new OpCode(0xFD,
                        Operation.SBC,
                        AddressMode.AbsoluteX,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xF9,
                    new OpCode(0xF9,
                        Operation.SBC,
                        AddressMode.AbsoluteY,
                        3,
                        4,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xE1,
                    new OpCode(0xE1,
                        Operation.SBC,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xF1,
                    new OpCode(0xF1,
                        Operation.SBC,
                        AddressMode.IndirectY,
                        2,
                        5,
                        StatusFlags.Carry | StatusFlags.Zero | StatusFlags.Negative | StatusFlags.Overflow,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x38,
                    new OpCode(0x38,
                        Operation.SEC,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Carry,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0xF8,
                    new OpCode(0xF8,
                        Operation.SED,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Decimal,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x78,
                    new OpCode(0x78,
                        Operation.SEI,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.InterruptDisable,
                        executionStrategy: new CPU.NotImplementedStrategy())
                },
                {
                    0x85,
                    new OpCode(0x85,
                        Operation.STA,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x95,
                    new OpCode(0x95,
                        Operation.STA,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x8D,
                    new OpCode(0x8D,
                        Operation.STA,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x9D,
                    new OpCode(0x9D,
                        Operation.STA,
                        AddressMode.AbsoluteX,
                        3,
                        5,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x99,
                    new OpCode(0x99,
                        Operation.STA,
                        AddressMode.AbsoluteY,
                        3,
                        5,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x81,
                    new OpCode(0x81,
                        Operation.STA,
                        AddressMode.IndirectX,
                        2,
                        6,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x91,
                    new OpCode(0x91,
                        Operation.STA,
                        AddressMode.IndirectY,
                        2,
                        6,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x86,
                    new OpCode(0x86,
                        Operation.STX,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x96,
                    new OpCode(0x96,
                        Operation.STX,
                        AddressMode.ZeroPageY,
                        2,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x8E,
                    new OpCode(0x8E,
                        Operation.STX,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x84,
                    new OpCode(0x84,
                        Operation.STY,
                        AddressMode.ZeroPage,
                        2,
                        3,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x94,
                    new OpCode(0x94,
                        Operation.STY,
                        AddressMode.ZeroPageX,
                        2,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0x8C,
                    new OpCode(0x8C,
                        Operation.STY,
                        AddressMode.Absolute,
                        3,
                        4,
                        StatusFlags.None,
                        executionStrategy: new CPU.StoreRegisterStrategy())
                },
                {
                    0xAA,
                    new OpCode(0xAA,
                        Operation.TAX,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Negative | StatusFlags.Zero,
                        executionStrategy: new CPU.TransferStrategy())
                },
                {
                    0xA8,
                    new OpCode(0xA8,
                        Operation.TAY,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Negative | StatusFlags.Zero,
                        executionStrategy: new CPU.TransferStrategy())
                },
                {
                    0xBA,
                    new OpCode(0xBA,
                        Operation.TSX,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Negative | StatusFlags.Zero,
                        executionStrategy: new CPU.TransferStrategy())
                },
                {
                    0x8A,
                    new OpCode(0x8A,
                        Operation.TXA,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Negative | StatusFlags.Zero,
                        executionStrategy: new CPU.TransferStrategy())
                },
                {
                    0x9A,
                    new OpCode(0x9A,
                        Operation.TXS,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.None,
                        executionStrategy: new CPU.TransferStrategy())
                },
                {
                    0x98,
                    new OpCode(0x98,
                        Operation.TYA,
                        AddressMode.Implicit,
                        1,
                        2,
                        StatusFlags.Negative | StatusFlags.Zero,
                        executionStrategy: new CPU.TransferStrategy())
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

        public OpCode FindOpcode(Operation operation, AddressMode mode)
        {
            OpCode? op = Values.SingleOrDefault(v => v.Operation == operation && v.AddressMode == mode);

            if (op == null) throw new ArgumentException($"Operation {operation} in address mode {mode} is not defined");

            return op.Value;
        }
    }
}