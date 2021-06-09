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


using System.Collections.Generic;
using System;

namespace Trace
{

    /// <summary>
    ///  A lexical token, used when parsing a scene file.
    /// </summary>
    public class Token
    {

        /// <summary>
        /// The location in the source file
        /// </summary>
        public SourceLocation sourceLoc;

        public Token(SourceLocation sL)
        {
            this.sourceLoc = sL;
        }

    }

    /// <summary>
    /// A token representing the end of a file: this is its only use.
    /// </summary>
    public class StopToken : Token
    {

        public StopToken(SourceLocation sourceLoc) : base(sourceLoc) { }

    }


    public enum KeywordEnum
    {

        New = 1, Material, Plane, Sphere, Diffuse, Specular, Uniform, Checkered,
        Image, Identity, Translation, RotationX, RotationY, RotationZ, Scaling,
        Camera, Orthogonal, Perspective, Float

    }






    public class KeywordToken : Token
    {
        /// <summary>
        /// A token containing a keyword
        /// </summary>

        public KeywordEnum keyword;

        public static Dictionary<string, KeywordEnum> dict = new Dictionary<string, KeywordEnum>();
        dict.Add("new",KeywordEnum.New);

        public KeywordToken(SourceLocation sourceLoc, KeywordEnum key) : base(sourceLoc) 
        {
            this.keyword = key;
        }

        public string GetStringKeyword(){

            return Convert.ToString(this.keyword);
        }

    }


    public class IdentifierToken : Token
    {

        /// <summary>
        /// A token containing an identifier
        /// </summary>

        public string id;

        public IdentifierToken(SourceLocation sourceLoc, string s) : base(sourceLoc)
        {
            this.id = s;
        }


        public override string ToString()
        {
            return this.id;
        }
       


    }

}



