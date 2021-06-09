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
using System.Globalization;

#nullable enable
namespace Trace
{
    /// <summary>
    /// A high-level wrapper around a stream, used to parse scene files
    /// This class implements a wrapper around a stream, with the following additional capabilities:
    /// - It tracks the line number and column number;
    /// - It permits to 'un-read' characters and tokens.
    /// </summary>
    public class InputStream
    {

        public Stream stream;
        public SourceLocation location;
        public char savedChar;
        public SourceLocation savedLocation;
        public int tabulations;
        public Token? savedToken;

        public InputStream(Stream stream, string fileName = "", int tabulations = 4)
        {
            this.stream = stream;
            this.location = new SourceLocation(fileName: fileName, line: 1, col: 1);
            this.savedChar = '\0';
            this.savedLocation = this.location;
            this.tabulations = tabulations;
            this.savedToken = null;
        }

        private void _updatePosition(char ch)
        {
            if (ch == '\0')
                return;
            else if (ch == '\n')
            {
                this.location.lineNum += 1;
                this.location.colNum = 1;
            }
            else if (ch == '\t')
                this.location.colNum += this.tabulations;
            else
                this.location.colNum += 1;
        }

        /// <summary>
        /// Read a new character from the stream
        /// </summary>
        public char readChar()
        {
            char ch;
            if (this.savedChar != '\0')
            {
                ch = this.savedChar;
                this.savedChar = '\0';
            }
            else
            {
                int byteRead = this.stream.ReadByte(); // ReadByte returns -1 at the end of stream
                if (byteRead == -1)
                    ch = '\0';
                else
                    ch = Convert.ToChar(byteRead);
            }
            this.savedLocation = this.location;
            this._updatePosition(ch);
            return ch;
        }

        /// <summary>
        /// Push a character back to the stream
        /// </summary>
        public void unreadChar(char ch)
        {
            this.savedChar = ch;
            this.location = this.savedLocation;
        }

        /// <summary>
        /// Keep reading characters until a non-whitespace character is found
        /// </summary>
        public void skipWhitespacesAndComments()
        {
            char[] WHITESPACE = { ' ', '\t', '\n', '\r' };
            char[] ENDLINE = { '\n', '\r' };
            char ch = this.readChar();
            while (WHITESPACE.Contains(ch) || ch == '#')
            {
                if (ch == '#')
                {
                    // It's a comment! Keep reading until the end of the line (include the case "", the end-of-file)
                    while (!ENDLINE.Contains(this.readChar()))
                        continue;
                }


                ch = this.readChar();
                if (ch == '\0')
                    return;
            }
            this.unreadChar(ch);
            return;
        }

        private StringToken _parseStringToken(SourceLocation tokenLocation)
        {
            string token = "";
            while (true)
            {
                char ch = this.readChar();
                if (ch == '"')
                    break;
                if (ch == '\0')
                    throw new GrammarError(tokenLocation, "unterminated string");

                token += ch;
            }

            return new StringToken(token, tokenLocation);
        }

        private LiteralNumberToken _parseFloatToken(char firstChar, SourceLocation tokenLocation)
        {
            string token = firstChar.ToString();
            while (true)
            {
                char ch = this.readChar();
                char[] SCIENTIFIC = { 'e', 'E' };
                if (!(Char.IsDigit(ch) || ch == '.' || SCIENTIFIC.Contains(ch)))
                {
                    this.unreadChar(ch);
                    break;
                }

                token += ch;
            }

            float value;
            try
            {
                value = float.Parse(token, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                throw new GrammarError(tokenLocation, $"'{token}' is an invalid floating-point number");
            }


            return new LiteralNumberToken(tokenLocation, value);
        }

        public Token parseKeywordOrIdentifierToken(char firstChar, SourceLocation tokenLocation)
        {
            string token = firstChar.ToString();
            while (true)
            {
                char ch = this.readChar();

                if (!(Char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    this.unreadChar(ch);
                }

                token += ch;

                try
                {
                    return new KeywordToken(tokenLocation, KeywordToken.dict[token]);
                }
                catch (System.Exception)
                {
                    return new IdentifierToken(tokenLocation, token);
                }

            }

        }

        // public Token readToken()
        // {
        //     if (this.savedToken != null)
        //     {
        //         Token result = this.savedToken;
        //         this.savedToken = null;
        //         return result;
        //     }

        //     this.skipWhitespaces();

        //     char ch = this.readChar();

        //     if (ch == '') return new StopToken(this.location);

        //     if (ch == '#')
        //     {
        //         while(this.readChar() != '\r' && this.readChar() != '\n' && this.readChar() != '\0') 
        //         {
        //             continue;
        //         }

        //         this.skipWhitespaces();
        //     }

        //     else    this.unreadChar(ch);

        // }
    }


}