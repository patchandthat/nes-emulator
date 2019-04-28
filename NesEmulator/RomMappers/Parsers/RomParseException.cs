using System;
using System.Runtime.Serialization;

namespace NesEmulator.RomMappers.Parsers
{
    [Serializable]
    public class RomParseException : Exception
    {
        public RomParseException()
        {
        }

        public RomParseException(string message) : base(message)
        {
        }

        public RomParseException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RomParseException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
