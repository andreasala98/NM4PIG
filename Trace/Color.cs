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
    ///  A very simple struct: three floats r,g,b to represent a color.
    ///  This value type is fundamental to implement more complex structures and classes.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Value for red
        /// </summary>
        public float r;

        /// <summary>
        /// value for green
        /// </summary>
        public float g;

        /// <summary>
        /// value for blue
        /// </summary>
        public float b;

        /// <summary>
        /// Create an instance of the struct Color.
        /// </summary>
        /// <param name="Red">The value for red</param>
        /// <param name="Green">The value for green</param>
        /// <param name="Blue">The value for blue</param>
        public Color(float Red, float Green, float Blue)
        {
            this.r = Red;
            this.g = Green;
            this.b = Blue;
        }

        /// <summary>
        /// Generate a totally random color! This method is for the brave only.
        /// </summary>
        /// <returns> A random Color (we hope it is a pretty one)</returns>
        public static Color random() {
            var rnd = new Random();
            return new Color((float)rnd.NextDouble(),(float)rnd.NextDouble(),(float)rnd.NextDouble() );
        }

        /// <summary>
        /// Element-wise sum of two Colors.
        /// </summary>
        /// <param name="col1"> The first Color</param>
        /// <param name="col2"> The second Color</param>
        /// <returns> A new color.</returns>
        public static Color operator +(Color col1, Color col2)
            => new Color(col1.r + col2.r, col1.g + col2.g, col1.b + col2.b);

        /// <summary>
        /// Element-wise difference of two Colors.
        /// </summary>
        /// <param name="col1"> The first Color</param>
        /// <param name="col2"> The second Color</param>
        /// <returns> A new color.</returns>
        public static Color operator -(Color col1, Color col2)
            => new Color(col1.r - col2.r, col1.g - col2.g, col1.b - col2.b);

        /// <summary>
        /// Multiplies a <see cref="Color"/> with a <see cref="float"/>.
        /// </summary>
        /// <param name="a"> A color.</param>
        /// <param name="alfa">Scaling factor.</param>
        /// <returns> A new color.</returns>
        public static Color operator *(Color a, float alfa)
            => new Color(a.r * alfa, a.g * alfa, a.b * alfa);

        /// <summary>
        /// Multiplies a <see cref="float"/> with a <see cref="Transformation"/>.
        /// </summary>
        /// <param name="a"> A color.</param>
        /// <param name="alfa">Scaling factor.</param>
        /// <returns> A new <see cref="Color"/>.</returns>
        public static Color operator *(float alfa, Color a)
            => new Color(a.r * alfa, a.g * alfa, a.b * alfa);

        /// <summary>
        /// Multiplies a <see cref="Color"/> with another <see cref="Color"/>.
        /// </summary>
        /// <param name="A"> A <see cref="Color"/>.</param>
        /// <param name="B">Another <see cref="Color"/>.</param>
        /// <returns> A new <see cref="Color"/>.</returns>
        public static Color operator *(Color A, Color B)
            => new Color(A.r * B.r, A.g * B.g, A.b * B.b);

        /// <summary>
        /// Boolean method to check if 
        /// two <see cref="Color"/>s are close, used mainly for test purpose
        /// </summary>
        /// <param name="A"> The other <see cref="Color"/>. </param>
        /// <returns> True if the <see cref="Color"/>s are close enough</returns>
        public bool isClose(Color A)
            => Utility.areClose(this.r, A.r) && Utility.areClose(this.b, A.b) && Utility.areClose(this.g, A.g);

        /// <summary>
        /// Calculate luminosity of a <see cref="Color"/> according to Shirley and Morley.
        /// </summary>
        /// <returns> A <see cref="float"/> represeting luminosity of the pixel </returns>
        public float Luminosity()
            => (float)(Math.Max(Math.Max(r, b), g) + Math.Min(Math.Min(r, b), g)) / 2;

        public override string ToString()
        {
            return $"Red: {this.r}, Green: {this.g}, Blue: {this.b}";
        }
    }
}