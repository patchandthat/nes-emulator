namespace NesEmulator.Processor
{
    internal struct OpCode
    {
        public OpCode(
            byte value,
            Operation operation,
            AddressMode addressMode,
            byte bytes,
            int cycles,
            StatusFlags affectsFlags, 
            CPU.ExecutionStrategyBase executionStrategy)
        {
            Value = value;
            Operation = operation;
            AddressMode = addressMode;
            Bytes = bytes;
            Cycles = cycles;
            AffectsFlags = affectsFlags;
            ExecutionStrategy = executionStrategy;
        }

        public int Cycles { get; }
        public Operation Operation { get; }
        public AddressMode AddressMode { get; }
        public StatusFlags AffectsFlags { get; }
        public CPU.ExecutionStrategyBase ExecutionStrategy { get; }
        public byte Bytes { get; }
        public byte Value { get; }

        public override string ToString()
        {
            return $"{Value:X2} : {Operation} {AddressMode}";
        }
    }
}