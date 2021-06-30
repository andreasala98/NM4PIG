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

    } //StopToken


    public enum KeywordEnum
    {

        New = 1, Material, Plane, Sphere, Cylinder, Cone, CSGUnion, CSGIntersection, 
        CSGDifference, Box,  Diffuse, Specular, Uniform, Checkered,
        Image, Identity, Translation, RotationX, RotationY, RotationZ, Scaling,
        Camera, Orthogonal, Perspective, Float, String, Wikishape

    }


    public class KeywordToken : Token
    {
        /// <summary>
        /// A token containing a keyword
        /// </summary>

        public KeywordEnum keyword;

        public static Dictionary<string, KeywordEnum> dict = new Dictionary<string, KeywordEnum>(){

            {  "new", KeywordEnum.New },
            {  "material", KeywordEnum.Material },
            {  "plane" , KeywordEnum.Plane},
            {  "sphere" , KeywordEnum.Sphere},
            {  "diffuse" , KeywordEnum.Diffuse},
            {  "specular" , KeywordEnum.Specular},
            {  "uniform" , KeywordEnum.Uniform},
            {  "checkered" , KeywordEnum.Checkered},
            {  "image" , KeywordEnum.Image},
            {  "identity" , KeywordEnum.Identity},
            {  "translation" , KeywordEnum.Translation},
            {  "rotationX" , KeywordEnum.RotationX},
            {  "rotationY" , KeywordEnum.RotationY},
            {  "rotationZ" , KeywordEnum.RotationZ},
            {  "scaling" , KeywordEnum.Scaling},
            {  "camera" , KeywordEnum.Camera},
            {  "orthogonal" , KeywordEnum.Orthogonal},
            {  "perspective" , KeywordEnum.Perspective},
            {  "float" , KeywordEnum.Float},
            {  "string", KeywordEnum.String}
        };


        public KeywordToken(SourceLocation sourceLoc, KeywordEnum key) : base(sourceLoc)
        {
            this.keyword = key;
        }

        public override string ToString()
        {
            if (!dict.ContainsValue(this.keyword)) throw new ArgumentException("Keyword not found");
            else return this.keyword.ToString();
        }

    } //KeywordToken


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

    } //IdentifierToken


    /// <summary>
    /// A token containing a literal string
    /// </summary>
    public class StringToken : Token
    {

        public string str;

        public StringToken(string s, SourceLocation sL) : base(sL)
        {

            this.str = s;

        }

        public override string ToString()
        {
            return this.str;
        }

    }

    /// <summary>
    /// A token containing a literal number
    /// </summary>
    public class LiteralNumberToken : Token
    {
        public float value;

        public LiteralNumberToken(SourceLocation sL, float val) : base(sL)
        {

            this.value = val;

        }

        public override string ToString()
        {
            return this.value.ToString();
        }

    }

    public class SymbolToken : Token
    {
        public string symbol;

        public SymbolToken(SourceLocation sL, string sym) : base(sL)
        {
            this.symbol = sym;
        }

    }

}



