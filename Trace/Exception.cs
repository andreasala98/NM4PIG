/*
The MIT License (MIT)

Copyright © 2021 Tommaso Armadillo, Pietro Klausner, Andrea Sala

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the “Software”), to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of
the Software. THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/

using System;

namespace Trace
{

    [Serializable]
    public class InvalidPfmFileFormat : Exception
    {
        public InvalidPfmFileFormat() : base() { }
        public InvalidPfmFileFormat(string Message) : base(Message) { }
    }

    [Serializable]
    public class CommandLineException : Exception
    {
        public CommandLineException() : base() { }
        public CommandLineException(string Message) : base(Message) { }
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