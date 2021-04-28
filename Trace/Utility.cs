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
        /// This function checks if two floats a and b are closer than epsilon
        /// </summary>
        public static bool areClose(float a, float b, float? epsilon = 1e-6f)
            => Math.Abs(a - b) < epsilon;

        /// <summary>
        /// This function checks if all the elements of two Matrix4x4 are closer than epsilon
        /// </summary>
        public static bool areMatricesClose(Matrix4x4 a, Matrix4x4 b)
            => areClose(a.M11, b.M11) && areClose(a.M12, b.M12) && areClose(a.M13, b.M13) && areClose(a.M14, b.M14) &&
               areClose(a.M21, b.M21) && areClose(a.M22, b.M22) && areClose(a.M23, b.M23) && areClose(a.M24, b.M24) &&
               areClose(a.M31, b.M31) && areClose(a.M32, b.M32) && areClose(a.M33, b.M33) && areClose(a.M34, b.M34) &&
               areClose(a.M41, b.M41) && areClose(a.M42, b.M42) && areClose(a.M43, b.M43) && areClose(a.M44, b.M44);
    }

    /// <summary>
    /// A class that contains some useful constants, which are used many times inside the library
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
    }

}