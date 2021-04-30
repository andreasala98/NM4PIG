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

    /// <summary>
    /// Exception InvalidPfmFileFormat. This Exception is intended to be raised whenever there is something 
    /// wrong while reading an image in the PFM format.
    /// </summary>
    [Serializable]
    public class InvalidPfmFileFormat : Exception
    {
        /// <summary>
        /// Constructor for InvalidPfmFileFormat exception. This raise a generic exception, i.e. does not contain an error message.
        /// </summary>
        public InvalidPfmFileFormat() : base() { }

        /// <summary>
        /// /// Constructor for InvalidPfmFileFormat exception. This raise an exception with an error message
        /// </summary>
        /// <param name="Message">The error message</param>
        public InvalidPfmFileFormat(string Message) : base(Message) { }
    }

    /// <summary>
    /// Exception CommandLineException. This Exception is intended to be raised whenever the user pass meaningless argouments 
    /// while executing the executable
    /// </summary>
    [Serializable]
    public class CommandLineException : Exception
    {
        /// <summary>
        /// Constructor for CommandLineException exception. This raise a generic exception, i.e. does not contain an error message
        /// </summary>
        public CommandLineException() : base() { }

        /// <summary>
        /// /// Constructor for CommandLineException exception. This raise an exception with an error message
        /// </summary>
        /// <param name="Message">The error message</param>
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
        } 
        catch (InvalidPfmFileFormat ex)
        {
            Console.WriteLine(ex.Message);
        }
    */
}