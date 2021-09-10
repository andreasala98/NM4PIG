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
using System.Collections.Generic;

namespace Trace
{
    /// <summary>
    /// A class which contains useful functions used inside the library
    /// </summary>
    public class Utility
    {
        public static float epsilon = 1e-5f;

        /// <summary>
        /// Check if two floating-point numbers are closer than epsilon
        /// </summary>
        /// <param name="a"> First number</param>
        /// <param name="b">< Second number/<param>
        /// <param name="epsilon"> Max distance allowed (default: 1e-6)</param>
        public static bool areClose(float a, float b, float? err = null)
        {
            err = err ?? Utility.epsilon;
            return Math.Abs(a - b) < err;
        }

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

        /// <summary>
        /// Convert an angle from degrees to radians
        /// </summary>
        /// <param name="deg"> Angle in degrees</param>
        /// <returns> Angle in radians</returns>
        public static float DegToRad(int deg)
            => (float)deg * Constant.PI / 180f;

        /// <summary>
        /// Convert an angle from radians to degrees
        /// </summary>
        /// <param name="deg"> Angle in radians</param>
        /// <returns> Angle in degrees</returns>
        public static float RadToDeg(float rad)
            => rad * 180f / Constant.PI;

        /// <summary>
        /// Minimum between three floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float Min(float a, float b, float c)
            => Math.Min(a, Math.Min(b, c));

        /// <summary>
        /// Maximum between three floats
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static float Max(float a, float b, float c)
            => Math.Max(a, Math.Max(b, c));

        /// <summary>
        /// Dot product between two normalized <see cref="Vec"/>s.
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float NormalizedDot(Vec v1, Vec v2)
            => v1.Normalize() * v2.Normalize();

        public static HitRecord? CorrectInvRayEffect(HitRecord? hit, Transformation trans)
        {
            if (!hit.HasValue) return null;
            return new HitRecord(
                    wp: trans * hit.Value.worldPoint,
                    nm: trans * hit.Value.normal,
                    sp: hit.Value.surfacePoint,
                    tt: hit.Value.t,
                    r: hit.Value.ray.Transform(trans),
                    shape: hit.Value.shape
                );
        }

        public static List<HitRecord?> CorrectInvRayEffect(List<HitRecord?> hits, Transformation trans)
        {
            List<HitRecord?> correctedHits = new List<HitRecord?>();
            foreach (HitRecord? hit in hits)
            {
                correctedHits.Add(CorrectInvRayEffect(hit, trans));
            }
            return correctedHits;
        }
    }

}

