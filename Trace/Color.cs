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
    ///  A very simple class: three floats r,g,b to represent a color.
    /// </summary>
    public struct Color
    {
        public float r;
        public float g;
        public float b;


        public Color(float Red, float Green, float Blue)
        {
            this.r = Red;
            this.g = Green;
            this.b = Blue;
        }


    /// <summary>
    /// Element-wise sum of two Colors.
    /// </summary>
    /// <param name="col1"> The first Color</param>
    /// <param name="col2"> The second Color</param>
    /// <returns> A new color.</returns>


        public static Color operator +(Color col1, Color col2)
            => new Color(col1.r + col2.r, col1.g + col2.g, col1.b + col2.b);


        //Difference of two Colors
        public static Color operator -(Color col1, Color col2)
            => new Color(col1.r - col2.r, col1.g - col2.g, col1.b - col2.b);


        // Scalar products
        public static Color operator *(Color a, float alfa)
            => new Color(a.r * alfa, a.g * alfa, a.b * alfa);


        public static Color operator *(float alfa, Color a)
            => new Color(a.r * alfa, a.g * alfa, a.b * alfa);


        //Product of two Colors
        public static Color operator *(Color A, Color B)
            => new Color(A.r * B.r, A.g * B.g, A.b * B.b);


                public static bool isClose(float a, float b)
        {
            var epsilon = 1e-8F;
            return Math.Abs(a - b) < epsilon;
        }


        /// <summary>
        /// Boolean method to check if 
        /// two Colors are equal
        /// </summary>
        /// <param name="a"> A Color. </param>
        /// <param name="b"> Another Color. </param>
        /// <returns></returns>

        public bool isClose(Color A)
        {
            return isClose(this.r, A.r) && isClose(this.b, A.b) && isClose(this.g, A.g);
        }

        public float Luminosity()
            => (float)(Math.Max(Math.Max(r, b), g) + Math.Min(Math.Min(r, b), g)) / 2;




    }

}