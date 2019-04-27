using System;
using System.Runtime.Serialization;

namespace NesEmulator.Memory
{
    [Serializable]
    public class MissingRomException : Exception
    {
        public MissingRomException() : this("No ROM loaded")
        {
        }

        public MissingRomException(string message) : base(message)
        {
        }

        public MissingRomException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MissingRomException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}