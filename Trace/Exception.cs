using System;

namespace Trace
{

    [Serializable]
    public class InvalidPfmFileFormat : Exception
    {
        public InvalidPfmFileFormat() : base() { }
        public InvalidPfmFileFormat(string message) : base(message) { }
    }

}