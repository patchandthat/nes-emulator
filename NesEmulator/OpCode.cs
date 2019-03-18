namespace NesEmulator
{
    internal struct OpCode
    {
        public OpCode(
            byte hex,
            Operation operation,
            AddressMode addressMode,
            byte bytes,
            int cycles,
            StatusFlags affectsFlags)
        {
            Hex = hex;
            Operation = operation;
            AddressMode = addressMode;
            Bytes = bytes;
            Cycles = cycles;
            AffectsFlags = affectsFlags;
        }

        public byte Hex { get; }
        public Operation Operation { get; }
        public AddressMode AddressMode { get; }
        public byte Bytes { get; }
        public int Cycles { get; }
        public StatusFlags AffectsFlags { get; }
        
        // Todo: Additional cycle conditions for non-constant time operations
    }
}