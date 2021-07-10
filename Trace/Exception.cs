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
    /// wrong while dealing with the <see cref="HDRImage"/> struct.
    /// </summary>
    [Serializable]
    public class InvalidPfmFileFormat : Exception
    {
        /// <summary>
        /// Constructor for InvalidPfmFileFormat exception. 
        /// This raises a generic exception, i.e. does not contain an error message.
        /// It is inherited from the <see cref="Exception"/> base constructor.
        /// </summary>
        public InvalidPfmFileFormat() : base() { }

        /// <summary>
        /// /// Constructor for InvalidPfmFileFormat exception. This raises an exception with an error message.
        /// </summary>
        /// <param name="Message">The error message</param>
        public InvalidPfmFileFormat(string Message) : base(Message) { }
    }

    /// <summary>
    /// Exception CommandLineException. This Exception is intended to be raised whenever the user
    /// passes meaningless arguments while running the executable.
    /// </summary>
    [Serializable]
    public class CommandLineException : Exception
    {
        /// <summary>
        /// Constructor for CommandLineException exception. 
        /// This raises a generic exception, i.e. does not contain an error message
        /// It is inherited from the <see cref="Exception"/> base constructor.
        /// </summary>
        public CommandLineException() : base() { }

        /// <summary>
        /// /// Constructor for CommandLineException exception. This raises an exception with an error message
        /// </summary>
        /// <param name="Message">The error message</param>
        public CommandLineException(string Message) : base(Message) { }
    }

    /// <summary>
    /// Esception GrammarError. This exception is intended to be raised whenever there is an error while
    /// parsing the scene file into the program in render mode. It is a compiling error, therefore it has to print the exact source
    /// location of the bug.
    /// </summary>
    [Serializable]
    public class GrammarError : Exception
    {
        public SourceLocation sourceLocation;

        /// <summary>
        /// Constructor for GrammarError exception. 
        /// This raises a generic exception, i.e. does not contain an error message
        /// It is inherited from the <see cref="Exception"/> base constructor.
        /// </summary>
        public GrammarError(SourceLocation location) : base()
        {
            this.sourceLocation = location;
        }

        /// <summary>
        /// /// Constructor for GrammarError exception. This raises an exception with an error message
        /// </summary>
        /// <param name="Message">The error message</param>
        public GrammarError(SourceLocation location, string Message) : base(Message)
        {
            this.sourceLocation = location;
        }
    }

    /*
        Example:

        To throw exceptions (Usually in libraries):

        throw new InvalidPfmFileFormat();
        throw new InvalidPfmFileFormat("Message Error");

        To catch exceptions (Usually in main program):

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