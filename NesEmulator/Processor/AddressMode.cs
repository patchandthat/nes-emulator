namespace NesEmulator.Processor
{
    internal enum AddressMode
    {
        /// <summary>
        ///     Does not need to be specified,
        ///     ie. CLC clear carry flag infers the source and target of the operation
        /// </summary>
        Implicit,

        /// <summary>
        ///     Operates directly upon the Accumulator,
        ///     ie. ROR A, rotates bits right
        /// </summary>
        Accumulator,

        /// <summary>
        ///     The byte following the opcode is used as the value, rather than an address/offset
        /// </summary>
        Immediate,

        /// <summary>
        ///     8 bit address operand, in the range 0x0000 to 0x00FF. High byte is always 0x00.
        /// </summary>
        ZeroPage,

        /// <summary>
        ///     The address for the instruction is on the zero page,
        ///     calculated by taking the byte following the opcode and adding the value of X index to it.
        ///     Mod 0xFF, wraps back to zero page rather than corrupting the stack
        /// </summary>
        ZeroPageX,

        /// <summary>
        ///     The address for the instruction is on the zero page,
        ///     calculated by taking the byte following the opcode and adding the value of Y index to it.
        ///     Mod 0xFF, wraps back to zero page rather than corrupting the stack
        /// </summary>
        ZeroPageY,

        /// <summary>
        ///     Relative addressing mode is used by branch instructions (e.g. BEQ, BNE, etc.)
        ///     which contain a signed 8 bit relative offset (e.g. -128 to +127) which is
        ///     added to program counter if the condition is true. As the program counter
        ///     itself is incremented during instruction execution by two the effective address
        ///     range for the target instruction must be with -126 to +129 bytes of the branch.
        /// </summary>
        Relative,

        /// <summary>
        ///     Instructions using absolute addressing contain a full 16 bit address to identify the target location.
        /// </summary>
        Absolute,

        /// <summary>
        ///     The address to be accessed by an instruction using X register indexed
        ///     absolute addressing is computed by taking the 16 bit
        ///     address from the instruction and added the contents of the X register.
        ///     For example if X contains $92 then an STA $2000,X instruction will
        ///     store the accumulator at $2092 (e.g. $2000 + $92).
        /// </summary>
        AbsoluteX,

        /// <summary>
        ///     The Y register indexed absolute addressing mode is the same as the
        ///     previous mode only with the contents of the Y register added to the 16
        ///     bit address from the instruction
        /// </summary>
        AbsoluteY,

        /// <summary>
        ///     JMP is the only 6502 instruction to support indirection.
        ///     The instruction contains a 16 bit address which identifies the location
        ///     of the least significant byte of another 16 bit memory address which is the
        ///     real target of the instruction.
        ///     For example if location $0120 contains $FC and location $0121 contains $BA
        ///     then the instruction JMP ($0120) will cause the next instruction execution
        ///     to occur at $BAFC (e.g. the contents of $0120 and $0121).
        /// </summary>
        Indirect,

        /// <summary>
        ///     Indexed indirect addressing is normally used in conjunction with a table
        ///     of address held on zero page. The address of the table is taken from the
        ///     instruction and the X register added to it (with zero page wrap around)
        ///     to give the location of the least significant byte of the target address.
        /// </summary>
        IndirectX,

        /// <summary>
        ///     Indirect indirect addressing is the most common indirection mode used
        ///     on the 6502. In instruction contains the zero page location of the
        ///     least significant byte of 16 bit address. The Y register is dynamically
        ///     added to this value to generated the actual target address for operation.
        /// </summary>
        IndirectY
    }
}