// ReSharper disable InconsistentNaming

namespace NesEmulator.Processor
{
    internal enum Operation
    {
        /// <summary>
        /// Load Accumulator
        /// </summary>
        LDA,

        /// <summary>
        /// Load X register
        /// </summary>
        LDX,

        /// <summary>
        /// Load Y register
        /// </summary>
        LDY,

        /// <summary>
        /// Store Accumulator
        /// </summary>
        STA,

        /// <summary>
        /// Store X register
        /// </summary>
        STX,

        /// <summary>
        /// Store Y register
        /// </summary>
        STY,

        /// <summary>
        /// Transfer Accumulator to X
        /// </summary>
        TAX,

        /// <summary>
        /// Transfer Accumulator to Y
        /// </summary>
        TAY,

        /// <summary>
        /// Transfer X to Accumulator
        /// </summary>
        TXA,

        /// <summary>
        /// Transfer Y to Accumulator
        /// </summary>
        TYA,

        /// <summary>
        /// Transfer stack pointer to X
        /// </summary>
        TSX,

        /// <summary>
        /// Transfer X to stack pointer
        /// </summary>
        TXS,

        /// <summary>
        /// Push Accumulator onto stack
        /// </summary>
        PHA,

        /// <summary>
        /// Push Processor Status onto stack
        /// </summary>
        PHP,

        /// <summary>
        /// Pull Accumulator from Stack
        /// </summary>
        PLA,

        /// <summary>
        /// Pull processor status from stack
        /// </summary>
        PLP,

        /// <summary>
        /// Logical AND, bitwise between accumulator and a byte of memory
        /// </summary>
        AND,

        /// <summary>
        /// Exclusive OR
        /// </summary>
        EOR,

        /// <summary>
        /// Logical inclusive OR
        /// </summary>
        ORA,

        /// <summary>
        /// Bit test
        /// </summary>
        BIT,

        /// <summary>
        /// Add with carry
        /// </summary>
        ADC,

        /// <summary>
        /// Subtract with carry
        /// </summary>
        SBC,

        /// <summary>
        /// Compare Accumulator
        /// </summary>
        CMP,

        /// <summary>
        /// Compare X
        /// </summary>
        CPX,

        /// <summary>
        /// Compare Y
        /// </summary>
        CPY,

        /// <summary>
        /// Increment Accumulator
        /// </summary>
        INC,

        /// <summary>
        /// Increment X
        /// </summary>
        INX,

        /// <summary>
        /// Increment Y
        /// </summary>
        INY,

        /// <summary>
        /// Decrement Accumulator
        /// </summary>
        DEC,

        /// <summary>
        /// Decrement X
        /// </summary>
        DEX,

        /// <summary>
        /// Decrement Y
        /// </summary>
        DEY,

        /// <summary>
        /// Arithmetic shift left
        /// </summary>
        ASL,

        /// <summary>
        /// Logical shift right
        /// </summary>
        LSR,

        /// <summary>
        /// Rotate left
        /// </summary>
        ROL,

        /// <summary>
        /// Rotate right
        /// </summary>
        ROR,

        /// <summary>
        /// Jump
        /// </summary>
        JMP,

        /// <summary>
        /// Jump, set return (subroutine)
        /// </summary>
        JSR,

        /// <summary>
        /// Return from subroutine
        /// </summary>
        RTS,

        /// <summary>
        /// Branch if carry flag clear
        /// </summary>
        BCC,

        /// <summary>
        /// Branch if carry flag set
        /// </summary>
        BCS,

        /// <summary>
        /// Branch if zero flag set
        /// </summary>
        BEQ,

        /// <summary>
        /// Branch if negative flag set
        /// </summary>
        BMI,

        /// <summary>
        /// Branch if zero flag clear
        /// </summary>
        BNE,

        /// <summary>
        /// Branch if negative flag clear
        /// </summary>
        BPL,

        /// <summary>
        /// Branch if overflow flag clear
        /// </summary>
        BVC,

        /// <summary>
        /// Branch if overflow flag set
        /// </summary>
        BVS,

        /// <summary>
        /// Clear carry flag
        /// </summary>
        CLC,

        /// <summary>
        /// Clear decimal mode flag
        /// </summary>
        CLD,

        /// <summary>
        /// Clear interrupt disable flag
        /// </summary>
        CLI,

        /// <summary>
        /// Clear overflow flag
        /// </summary>
        CLV,

        /// <summary>
        /// Set carry flag
        /// </summary>
        SEC,

        /// <summary>
        /// Set decimal mode flag
        /// </summary>
        SED,

        /// <summary>
        /// Set interrupt disable flag
        /// </summary>
        SEI,

        /// <summary>
        /// Break, force an interrupt
        /// </summary>
        BRK,

        /// <summary>
        /// No operation
        /// </summary>
        NOP,

        /// <summary>
        /// Return from interrupt
        /// </summary>
        RTI,
    }
}
