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
using System.Numerics;

namespace Trace
{
    /// <summary>
    /// A class which contains useful functions used inside the library
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Check if two floating-point numbers are closer than epsilon
        /// </summary>
        /// <param name="a"> First number</param>
        /// <param name="b">< Second number/<param>
        /// <param name="epsilon"> Max distance allowed (default: 1e-6)</param>
        public static bool areClose(float a, float b, float? epsilon = 1e-6f)
            => Math.Abs(a - b) < epsilon;

        /// <summary>
        /// This function checks if all the elements of two Matrix4x4 are closer than epsilon
        /// </summary>
        /// <param name="a"> First matrix</param>
        /// <param name="b">< Second matrix</param>
        /// <param name="epsilon"> Max distance allowed (default: 1e-6)</param>
        public static bool areMatricesClose(Matrix4x4 a, Matrix4x4 b)
            => areClose(a.M11, b.M11) && areClose(a.M12, b.M12) && areClose(a.M13, b.M13) && areClose(a.M14, b.M14) &&
               areClose(a.M21, b.M21) && areClose(a.M22, b.M22) && areClose(a.M23, b.M23) && areClose(a.M24, b.M24) &&
               areClose(a.M31, b.M31) && areClose(a.M32, b.M32) && areClose(a.M33, b.M33) && areClose(a.M34, b.M34) &&
               areClose(a.M41, b.M41) && areClose(a.M42, b.M42) && areClose(a.M43, b.M43) && areClose(a.M44, b.M44);
    }

    /// <summary>
    /// A class that contains some useful constants, which are used many times inside the library.
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// A float version of Math.PI
        /// </summary>
        public static float PI = (float)Math.PI;

        /// <summary>
        /// The unit vector for the x-axis
        /// </summary>
        public static Vec VEC_X = new Vec(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The unit vector for the y-axis
        /// </summary>
        public static Vec VEC_Y = new Vec(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The unit vector for the z-axis
        /// </summary>
        public static Vec VEC_Z = new Vec(0.0f, 0.0f, 1.0f);

        // Hello!
    }

}