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
using System.Collections.Generic;

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
        /// <summary>
        /// stream we read from
        /// </summary>
        public Stream stream;
        /// <summary>
        /// Current location of the cursor
        /// </summary>
        public SourceLocation location;
        /// <summary>
        /// Char we just read, so that is possible to unread the char
        /// </summary>
        public char savedChar;
        /// <summary>
        /// Old position, so it is possible to unread the char
        /// </summary>
        public SourceLocation savedLocation;
        /// <summary>
        /// Number of spaces equivalent to tab, default is 4
        /// </summary>
        public int tabulations;
        /// <summary>
        /// Token we just read
        /// </summary>
        public Token? savedToken;

        /// <summary>
        /// Basic contructor for the class.
        /// </summary>
        /// <param name="stream">Stream pointing to the file we want to read</param>
        /// <param name="fileName">Name of the file</param>
        /// <param name="tabulations"> How many spaces in a tab</param>
        public InputStream(Stream stream, string fileName = "", int tabulations = 4)
        {
            this.stream = stream;
            this.location = new SourceLocation(fileName: fileName, line: 1, col: 1);
            this.savedChar = '\0';
            this.savedLocation = this.location;
            this.tabulations = tabulations;
            this.savedToken = null;
        }

        /// <summary>
        /// Shift the cursor one position ahead. It also saves the last position into the 
        /// "old" data members
        /// </summary>
        /// <param name="ch"> The new character to be inserted</param>
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
            this.savedLocation = this.location.shallowCopy();
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

        /// <summary>
        /// Read a <see cref="StringToken"/> in a given location
        /// </summary>
        /// <param name="tokenLocation"> The location in the file</param>
        /// <returns> The <see cref="StringToken"/> we just read.</returns>
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

        /// <summary>
        /// Read a <see cref="LiteralNumberToken"/> in a given location
        /// </summary>
        /// <param name="firstChar"> The first character of the token</param>
        /// <returns> The <see cref="LiteralNumberToken"/> we just read.</returns>
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

        /// <summary>
        /// Read either a <see cref="KeywordToken"/> or a <see cref="IdentifierToken"/> in a given location
        /// </summary>
        /// <param name="char"> The first characterof the Token</param>
        /// <returns> The <see cref="Token"/> we just read.</returns>
        private Token _parseKeywordOrIdentifierToken(char firstChar, SourceLocation tokenLocation)
        {
            string token = firstChar.ToString();
            while (true)
            {
                char ch = this.readChar();

                if (!(Char.IsLetterOrDigit(ch) || ch == '_'))
                {
                    this.unreadChar(ch);
                    break;
                }
                token += ch;
            }

            try
            {
                return new KeywordToken(tokenLocation, KeywordToken.dict[token]);
            }
            catch (System.Exception)
            {
                return new IdentifierToken(tokenLocation, token);
            }

        }

        /// <summary>
        /// Read and interpetate the token
        /// </summary>
        public Token readToken()
        {
            if (this.savedToken != null)
            {
                Token result = this.savedToken;
                this.savedToken = null;
                return result;
            }

            this.skipWhitespacesAndComments();

            char ch = this.readChar();

            if (ch == '\0') return new StopToken(this.location);

            char[] SYMBOLS = { '(', ')', '<', '>', '[', ']', ',', '*' };
            char[] OPERATIONS = { '+', '-', '.' };
            if (SYMBOLS.Contains(ch))
                return new SymbolToken(this.location, ch.ToString());
            else if (ch == '"')
                return this._parseStringToken(this.location);
            else if (Char.IsDigit(ch) || OPERATIONS.Contains(ch))
                return this._parseFloatToken(ch, this.location);
            else if (Char.IsLetter(ch) || ch == '_')
                return this._parseKeywordOrIdentifierToken(ch, this.location);
            else
                throw new GrammarError(this.location, $"Invalid character {ch}");
        }



        /// <summary>
        /// Read a token from input file and check that it is one of the keywords in keywords.
        /// Return the keyword as a  <see cref="KeywordEnum"/> object.
        /// </summary>
        /// <param name="keywords"> The list of accepted keywords</param>
        /// <returns> The parsed keyword in KeywordEnum format</returns>
        public KeywordEnum expectKeywords(List<KeywordEnum> keywords)
        {
            Token token = this.readToken();

            if (!(token is KeywordToken))
            {
                throw new GrammarError(token.sourceLoc, $"expected keyword instead of {token} at {token.sourceLoc.ToString()}");
            }
            KeywordToken keyToken = (KeywordToken)token;
            if (!keywords.Contains(keyToken.keyword)) //keyToken is not in keywords
            {
                throw new GrammarError(keyToken.sourceLoc, "unexpected keyword");
            }

            return keyToken.keyword;
        }

        /// <summary>
        /// Read a token from inputFile and check that it matches a certain symbol.
        /// </summary>
        /// <param name="symbol"> The symbol to be parsed</param>
        public void expectSymbol(string symbol)
        {
            Token token = this.readToken();
            if (!(token is SymbolToken) || ((SymbolToken)token).symbol != symbol)
            {
                throw new GrammarError(token.sourceLoc, $"I was expecting {symbol} but i got {token}");
            }
        }

        /// <summary>
        /// Read a token from inputFile and check that it is either a literal number or a variable in scene.
        /// Return the number as a float.
        /// </summary>
        /// <param name="scene"> The scene to be analysed</param>
        /// <returns> The float correpsonding to the variable OR the float we just read.</returns>
        public float expectNumber(Scene scene)
        {
            Token token = this.readToken();

            if (token is LiteralNumberToken)
                return ((LiteralNumberToken)token).value;
            else if (token is IdentifierToken)
            {
                string variableName = ((IdentifierToken)token).id;
                if (!(scene.floatVariables.ContainsKey(variableName)))
                    throw new GrammarError(token.sourceLoc, $"unknown variable {token}");

                return scene.floatVariables[variableName];
            }
            throw new GrammarError(token.sourceLoc, $"got {token} instead of a number");
        }

        /// <summary>
        /// Read a token from inputFile and check that it is a literal string.
        /// Return the value of the string (a string).
        /// </summary>
        /// <returns> The parsed string</returns>
        public string expectString()
        {
            Token token = this.readToken();
            if (!(token is StringToken))
                throw new GrammarError(token.sourceLoc, $"got {token} instead of a string");
            return ((StringToken)token).str;
        }

        /// <summary>
        /// Read a token from inputFile and check that it is an identifier token.
        /// Return the value of the string (a string).
        /// </summary>
        /// <returns> The parsed identifier in string format</returns>
        public string expectIdentifier()
        {
            Token token = this.readToken();
            if (!(token is IdentifierToken))
                throw new GrammarError(token.sourceLoc, $"got {token}, instead of an Identifier");
            return ((IdentifierToken)token).id;
        }

        /// <summary>
        /// Put a <see cref="Token"/> back in place
        /// </summary>
        /// <param name="token"> The token to unread</param>
        public void unreadToken(Token token)
        {
            this.savedToken = token;
        }
    }

}


