using System.IO;

namespace NesEmulator
{
    abstract class ROM
    {
        public static ROM Create(Stream stream)
        {
            // Parse. Determine mapper.

            return null;
        }
    }

    class NROM : ROM
    {

    }
}
