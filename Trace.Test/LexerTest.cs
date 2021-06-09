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

        
        public void AssertIsKeyword(Token token, KeywordEnum keyword)
        {
            Assert.True((KeywordToken)token is KeywordToken, "The token is not a key word");
            Assert.True(((KeywordToken)token).keyword == keyword, $"Token {token} is not equal to keyword {keyword}");
        }

        public void AssertIsIdentifier(Token token, string identifier )
        {
            Assert.True(token is IdentifierToken, "The token is not an identifier");
            Assert.True(((IdentifierToken)token).id == identifier, $"Token {token} is not equal to an identifier {identifier}");
        }
        
        public void AssertIsSymbol(Token token, string symbol)
        {
            Assert.True(token is SymbolToken, "The token is not a symbol");
            Assert.True(((SymbolToken)token).symbol == symbol, $"Token {token} is not equal to a symbol {symbol}");
        }

        public void AssertIsNumber(Token token, float number)
        {
            Assert.True(token is LiteralNumberToken, "The token is not a number");
            Assert.True(((LiteralNumberToken)token).value == number, $"Token {token} is not equal to a number {number}");
        }
        public void AssertIsString(Token token, string frase)
        {
            Assert.True(token is StringToken, "The token is not a string");
            Assert.True(((StringToken)token).str == frase, $"Token {token} is not equal to a string {frase}");
        }

        [Fact]
        public void TestInputFile() {

            byte[] byteArray = Encoding.ASCII.GetBytes("abc   \nd\nef");
            MemoryStream iS = new MemoryStream(byteArray);
            InputStream stream = new InputStream(iS);


            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 1");
            Assert.True(stream.location.colNum == 1, "TestInputFile failed! Assert 2");

            Assert.True(stream.readChar()=='a', "TestInputFile failed! Assert 3");
            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 4");
            Assert.True(stream.location.colNum == 2, "TestInputFile failed! Assert 5");

            stream.unreadChar('A');
            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 6");
            Assert.True(stream.location.colNum == 1, "TestInputFile failed! Assert 7");

            Assert.True(stream.readChar()=='A', "TestInputFile failed! Assert 8");
            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 9");
            Assert.True(stream.location.colNum == 2, "TestInputFile failed! Assert 10");

            Assert.True(stream.readChar()=='b', "TestInputFile failed! Assert 11");
            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 12");
            Assert.True(stream.location.colNum == 3, "TestInputFile failed! Assert 13");

            Assert.True(stream.readChar()=='c', "TestInputFile failed! Assert 14");
            Assert.True(stream.location.lineNum == 1, "TestInputFile failed! Assert 15");
            Assert.True(stream.location.colNum == 4, "TestInputFile failed! Assert 16");

            stream.skipWhitespacesAndComments();

            Assert.True(stream.readChar()=='d', "TestInputFile failed! Assert 17");
            Assert.True(stream.location.lineNum == 2, "TestInputFile failed! Assert 18");
            Assert.True(stream.location.colNum == 2, "TestInputFile failed! Assert 19");

            Assert.True(stream.readChar()=='\n', "TestInputFile failed! Assert 20");
            Assert.True(stream.location.lineNum == 3, "TestInputFile failed! Assert 21");
            Assert.True(stream.location.colNum == 1, "TestInputFile failed! Assert 22");

            Assert.True(stream.readChar()=='e', "TestInputFile failed! Assert 23");
            Assert.True(stream.location.lineNum == 3, "TestInputFile failed! Assert 24");
            Assert.True(stream.location.colNum == 2, "TestInputFile failed! Assert 25");

            Assert.True(stream.readChar()=='f', "TestInputFile failed! Assert 26");
            Assert.True(stream.location.lineNum == 3, "TestInputFile failed! Assert 27");
            Assert.True(stream.location.colNum == 3, "TestInputFile failed! Assert 28");

            Assert.True(stream.readChar() == '\0', "TestInputFile failed! Assert 29");


        }

        [Fact]
        public void TestLexer()
        {
            string test = @" 
                # This is a comment
                # This is another comment
                new material sky_material(
                 diffuse(image(""my file.pfm "")),
                 <5.0, 500.0, 300.0 >
                ) # Comment at the end of the line";
            
            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);

            InputStream inputStream = new InputStream(stream);

            AssertIsKeyword(inputStream.readToken(), KeywordEnum.New);
            AssertIsKeyword(inputStream.readToken(), KeywordEnum.Material);
            AssertIsIdentifier(inputStream.readToken(), "sky_material");
            AssertIsSymbol(inputStream.readToken(), "(");
            AssertIsKeyword(inputStream.readToken(), KeywordEnum.Diffuse);
            AssertIsSymbol(inputStream.readToken(), "(");
            AssertIsKeyword(inputStream.readToken(), KeywordEnum.Image);
            AssertIsSymbol(inputStream.readToken(), "(");
            AssertIsString(inputStream.readToken(), "my file.pfm");
            AssertIsSymbol(inputStream.readToken(), ")");
        }



    }
}