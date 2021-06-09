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
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Xunit;

namespace Trace.Test
{
    public class LexerTest
    {

        //public bool AssertIsKeyword(Token tk, KeywordEnum kw ) {

    

    [Fact]
        public void TestSceneFile() {

            byte[] byteArray = Encoding.ASCII.GetBytes("abc   \nd\nef");
            MemoryStream iS = new MemoryStream(byteArray);
            InputStream stream = new InputStream(iS);


            Assert.True(stream.location.lineNum == 1, "TestSceneFile failed! Assert 1");
            Assert.True(stream.location.colNum == 1, "TestSceneFile failed! Assert 2");

            Assert.True(stream.readChar()=='a', "TestSceneFile failed! Assert 3");
            Assert.True(stream.location.lineNum == 1, "TestSceneFile failed! Assert 4");
            Assert.True(stream.location.colNum == 2, "TestSceneFile failed! Assert 5");

            stream.unreadChar("a");

        }



    }
}