namespace NesEmulator.Processor
{
    internal struct OpCode
    {
        public OpCode(
            byte hex,
            Operation operation,
            AddressMode addressMode,
            byte bytes,
            int cycles,
            StatusFlags affectsFlags, 
            CPU.ExecutionStrategyBase executionStrategy)
        {
            Hex = hex;
            Operation = operation;
            AddressMode = addressMode;
            Bytes = bytes;
            Cycles = cycles;
            AffectsFlags = affectsFlags;
            ExecutionStrategy = executionStrategy;
        }

        public byte Hex { get; }
        public Operation Operation { get; }
        public AddressMode AddressMode { get; }
        public byte Bytes { get; }
        public int Cycles { get; }
        public StatusFlags AffectsFlags { get; }
        
        public CPU.ExecutionStrategyBase ExecutionStrategy { get; }

        public override string ToString()
        {
            return $"{Hex:X2} : {Operation} {AddressMode}";
        }
    }
}