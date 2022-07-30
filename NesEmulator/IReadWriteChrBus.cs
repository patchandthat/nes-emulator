namespace NesEmulator
{
    public interface IReadWriteChrBus
    {
        /// <summary>
        ///     Reads the contents of a single memory address
        /// </summary>
        /// <param name="address">The address to read</param>
        /// <returns>Byte value att he requested address</returns>
        byte ReadChr(ushort address);

        /// <summary>
        ///     Used for debugger & tests.
        ///     Reading some memory mapped i/o addresses causes side effects and changes their state.
        /// </summary>
        /// <param name="address">The address to peek</param>
        /// <returns>Byte value at the requested address</returns>
        byte PeekChr(ushort address);

        /// <summary>
        ///     Write a value to a memory address
        /// </summary>
        /// <param name="address">The address of the byte to write</param>
        /// <param name="value">The value of the byte to write at the address</param>
        void WriteChr(ushort address, byte value);
    }
}