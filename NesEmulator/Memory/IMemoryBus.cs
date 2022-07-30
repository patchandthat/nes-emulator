using NesEmulator.RomMappers;

namespace NesEmulator.Memory
{
    internal interface IMemoryBus : IReadWrite
    {
        /// <summary>
        ///     Attach ROM to memory addresses
        /// </summary>
        void Load(ROM rom);
    }
}