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
using Trace;

namespace Trace
{
    /// <summary>
    /// A specific position in a source file
    /// </summary>
    public class SourceLocation
    {
        /// <summary>
        /// the name of the file, or the empty string if there is no file associated with this location
        /// (e.g., because the source code was provided as a memory stream, or through a network connection)
        /// </summary>
        public char fileName;
        /// <summary>
        /// number of the line (starting from 1)
        /// </summary>
        public int lineNum = 0;
        /// <summary>
        /// number of the column (starting from 1)
        /// </summary>
        public int colNum = 0;

        public SourceLocation(char fileName, int line = 1, int col = 1)
        {
            this.fileName = fileName;
            this.lineNum = line;
            this.colNum = col;
        }
    }
}