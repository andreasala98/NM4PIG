using System;

namespace Trace
{

    [Serializable]
    public class InvalidPfmFileFormat : Exception
    {
        public InvalidPfmFileFormat() : base() { }
        public InvalidPfmFileFormat(string Message) : base(Message) { }
    }

    /*
        Example:

        For throwing exception:

        throw new InvalidPfmFileFormat();
        throw new InvalidPfmFileFormat("Message Error");

        For catching exceptions

        try
        {
            //Something
        } catch (InvalidPfmFileFormat ex)
        {
            Console.WriteLine(ex.Message);
        }
    */

}