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
    /// Value type to represent a point in 3D space.
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// x coordinate of the point.
        /// </summary>
        public float x;
        /// <summary>
        /// y coordinate of the point.
        /// </summary>
        public float y;
        /// <summary>
        /// z coordinate of the point.
        /// </summary>
        public float z;

        // constructor
        /// <summary>
        /// Default constructor for Point. It takes x,y,z in this order.
        /// </summary>
        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Converts a Point to a string for printing.
        /// </summary>
        /// <returns> A string in the format Point(x=" ",y=" ",z=" ")</returns>
        public override string ToString() => $"Point(x={this.x}, y={this.y}, z={this.z})";

        // isClose method to Tests purposes
        private static bool isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        /// <summary>
        /// Boolean to check if two Points are close enough
        /// </summary>
        /// <param name="A"> The other Point</param>
        /// <returns>True if the Points are close</returns> 
        public bool isClose(Point A)
           => isClose(this.x, A.x) && isClose(this.y, A.y) && isClose(this.z, A.z);


        /// <summary>
        /// Adds a point and a vec
        /// </summary>
        /// <param name="p"> A <see cref="Point"/> object</param>
        /// <param name="v"> A <see cref="Vec"/> object</param>
        /// <returns> A <see cref="Point"/> object</returns>
        public static Point operator +(Point p, Vec v)
            => new Point(p.x + v.x, p.y + v.y, p.z + v.z);

        /// <summary>
        /// Subtracts a vec from a point.
        /// </summary>
        /// <param name="p"> A <see cref="Point"/> object.</param>
        /// <param name="v"> A <see cref="Vec"/> object.</param>
        /// <returns> A <see cref="Point"/> object.</returns>
        public static Point operator -(Point p, Vec v)
            => new Point(p.x - v.x, p.y - v.y, p.z - v.z);

        /// <summary>
        /// Calculate the vector connecting two Points
        /// </summary>
        /// <param name="p"> A <see cref="Point"/> object.</param>
        /// <param name="v"> A <see cref="Point"/> object.</param>
        /// <returns> A <see cref="Vec"/> object.</returns>
        public static Vec operator -(Point p, Point v)
            => new Vec(p.x - v.x, p.y - v.y, p.z - v.z);

        /// <summary>
        /// Divide a Point by a scaling factor.
        /// </summary>
        /// <param name="a"> The point.</param>
        /// <param name="alfa"> Scaling factor. </param>
        /// <returns> The scaled Point.</returns>
        public static Point operator /(Point a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a point by zero!");
            return new Point(a.x / alfa, a.y / alfa, a.z / alfa);
        }

    }

    /// <summary>
    ///  Value type to represent a vector in a 3D space.
    /// </summary>
    public struct Vec
    {
        /// <summary>
        ///  x coordinate of the vector.
        /// </summary>
        public float x;
        /// <summary>
        ///  y coordinate of the vector.
        /// </summary>
        public float y;
        /// <summary>
        ///  z coordinate of the vector.
        /// </summary>
        public float z;

        /// <summary>
        /// Default constructor for Vec
        /// </summary>
        /// <param name="x"> x coord </param>
        /// <param name="y"> y coord </param>
        /// <param name="z"> z coord </param>
        public Vec(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// Sum of two vectors
        /// </summary>
        /// <param name="a"> First vector</param>
        /// <param name="b"> Second vector</param>
        /// <returns> A vector sum </returns>
        public static Vec operator +(Vec a, Vec b)
            => new Vec(a.x + b.x, a.y + b.y, a.z + b.z);

        /// <summary>
        /// Difference of two vectors
        /// </summary>
        /// <param name="a"> First vector</param>
        /// <param name="b"> Second vector</param>
        /// <returns> A vector difference </returns>
        public static Vec operator -(Vec a, Vec b)
            => new Vec(a.x - b.x, a.y - b.y, a.z - b.z);
   
        /// <summary>
        /// Multiplication vector - scalar
        /// </summary>
        /// <param name="a">  Vector</param>
        /// <param name="alfa">  Scaling factor </param>
        /// <returns> A scaled vector. </returns>
        public static Vec operator *(float alfa, Vec a)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);
        /// <summary>
        /// Multiplication vector - scalar
        /// </summary>
        /// <param name="a">  Vector</param>
        /// <param name="alfa">  Scaling factor </param>
        /// <returns> A scaled vector. </returns>
        public static Vec operator *(Vec a, float alfa)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);

        /// <summary>
        /// Divide a Vec by a scaling factor.
        /// </summary>
        /// <param name="a"> The Vec.</param>
        /// <param name="alfa"> Scaling factor. </param>
        /// <returns> The scaled Vec.</returns>
        public static Vec operator /(Vec a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a vector by zero!");
            return new Vec(a.x / alfa, a.y / alfa, a.z / alfa);
        }

        /// <summary>
        ///  Euclidean scalar product between two vectors.
        /// </summary>
        /// <param name="a"> First vector </param>
        /// <param name="b"> Second vector</param>
        /// <returns> Scalar product in float format.</returns>
        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        /// <summary>
        ///  Cross product between two 3D vectors.
        /// </summary>
        /// <param name="a"> First vector </param>
        /// <param name="b"> Second vector</param>
        /// <returns> Cross product in Vec format.</returns>
        public Vec crossProd(Vec b)
          => new Vec  (this.y * b.z - this.z * b.y,
                       this.z * b.x - this.x * b.z,
                       this.x * b.y - this.y * b.x);


        public float getSquaredNorm()
            => this * this;

        public float getNorm()
            => (float)Math.Sqrt(this.getSquaredNorm());

        // Normalize vector
        public Vec Normalize()
            => this / this.getNorm();

        // This could be useful for debugging
        public bool isNormalized()
            => this.getNorm() == 1.0f;

        /// <summary>
        /// Converts a Vec to a string for printing.
        /// </summary>
        /// <returns> A string in the format Vec(x=" ",y=" ",z=" ")</returns>
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        //Method for checking closeness in tests
        private static bool _isClose(float a, float b, float? epsilon = 1e-7f)
            => Math.Abs(a - b) < epsilon;

        /// <summary>
        /// Boolean to check if two Vec are close enough
        /// </summary>
        /// <param name="vector"> The other Vec</param>
        /// <returns>True if the Vecs are close</returns> 
        public bool isClose(Vec vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);


    }

    /// <summary>
    ///  Value type represnting a Normal vector
    /// </summary>
    public struct Normal
    {
        /// <summary>
        /// Basic data members
        /// </summary>
        public float x, y, z;

        /// <summary>
        ///  Default constructor
        /// </summary>
        /// <param name="ax"> x coordinate </param>
        /// <param name="ay"> y coordinate </param>
        /// <param name="az"> z coordinate </param>
        public Normal(float ax, float ay, float az)
        {
            this.x = ax;
            this.y = ay;
            this.z = az;
        }

        /// <summary>
        /// Converts a Normal to a string for printing.
        /// </summary>
        /// <returns> A string in the format Normal(x=" ",y=" ",z=" ")</returns>
        public override string ToString() => $"Norm(x={this.x}, y={this.y}, z={this.z})";

        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        /// <summary>
        /// Boolean to check if two Normals are close enough
        /// </summary>
        /// <param name="vector"> The other Normal</param>
        /// <returns>True if the Normals are close</returns> 
        public bool isClose(Normal vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);
    }

    /// <summary>
    ///  Affine transformation. It is represented by a 4x4 matrix. 
    ///  It can be used to create translations, rotations and scalings.
    /// </summary>
    public struct Transformation
    {
        /// <summary>
        /// 4x4 Matrix representing the transformation.
        /// </summary>
        public Matrix4x4 M;
        /// <summary>
        /// 4x4 Matrix representing the inverse transformation.
        /// </summary>
        public Matrix4x4 Minv;

        /// <summary>
        /// Constructor for Identity transformation.
        /// </summary>
        /// <param name="a"> Any integer is fine.</param>
        public Transformation(int a)
        {
            this.M = Matrix4x4.Identity;
            this.Minv = Matrix4x4.Identity;
        }

        /// <summary>
        /// Constructor taking a 4x4 matrix and its inverse as inputs.
        /// </summary>
        /// <param name="myMat"> The matrix representing the transformation. </param>
        /// <param name="myInvMat"> The inverse matrix.</param>
        public Transformation(Matrix4x4 myMat, Matrix4x4 myInvMat)

        {
            this.M = myMat;
            this.Minv = myInvMat;
        }

        /// <summary>
        /// Switches the M and Minv fields. Practically, inverts the transformation.
        /// </summary>
        /// <returns> The inverse Transformation. </returns>
        public Transformation getInverse ()
        {
            return new Transformation (this.Minv, this.M);
        }


        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;


        /// <summary>
        /// Method to check if a Transformation and a Matrix4x4 are close.
        /// </summary>
        /// <param name="a"> The matrix to be compared with. </param>
        /// <returns> True if Transformation and Matrix are close</returns>
        public bool areClose(Matrix4x4 a)
            => _isClose(this.M.M11, a.M11) && _isClose(this.M.M12, a.M12) && _isClose(this.M.M13, a.M13) && _isClose(this.M.M14, a.M14) &&
               _isClose(this.M.M21, a.M21) && _isClose(this.M.M22, a.M22) && _isClose(this.M.M23, a.M23) && _isClose(this.M.M24, a.M24) &&
               _isClose(this.M.M31, a.M31) && _isClose(this.M.M32, a.M32) && _isClose(this.M.M33, a.M33) && _isClose(this.M.M34, a.M34) &&
               _isClose(this.M.M41, a.M41) && _isClose(this.M.M42, a.M42) && _isClose(this.M.M43, a.M43) && _isClose(this.M.M44, a.M44);

        /// <summary>
        /// Method to check if the Minv field actually contains the inverse matrix.
        /// </summary>
        /// <returns> True if Minv is the inverse matrix of M</returns>
        public bool isConsistent()
        {
            Transformation a = new Transformation(this.M * this.Minv, this.M * this.Minv);
            return a.areClose(Matrix4x4.Identity);
        }

        // The Matrix4x4 methods CreateTransletion and CreateRotatoin actually produce the
        // the transpose of the matrices we wanna use.
        
        /// <summary>
        /// Translate a Vec in 3D
        /// </summary>
        /// <param name="a"> The vector generating the translation. </param>
        /// <returns> The translation transformation. </returns>
        public static Transformation Translation(Vec a)
        {
            return new Transformation(  Matrix4x4.Transpose(Matrix4x4.CreateTranslation(a.x, a.y, a.z)),
                                        Matrix4x4.Transpose(Matrix4x4.CreateTranslation(-a.x, -a.y, -a.z)));
        }


        public static Transformation Scaling(Vec a)
        {
            Transformation b = new Transformation(1);
            b.M.M11 = a.x;
            b.M.M22 = a.y;
            b.M.M33 = a.z;

            b.Minv.M11 = 1.0f/a.x;
            b.Minv.M22 = 1.0f/a.y;
            b.Minv.M33 = 1.0f/a.z;
            
            return b;
        }
        /// <summary>
        /// Rotation along the x axis.
        /// </summary>
        /// <param name="a"> The rotation angle in radians </param>
        /// <returns> The rotation transformation. </returns>
        public static Transformation rotationX(float theta)
        {
            return new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationX(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationX(-theta))
            );
        }
        /// <summary>
        /// Rotation along the y axis.
        /// </summary>
        /// <param name="a"> The rotation angle in radians </param>
        /// <returns> The rotation transformation. </returns>
        public static Transformation rotationY(float theta)
        {
            return new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationY(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationY(-theta))
            );
        }

        /// <summary>
        /// Rotation along the z axis.
        /// </summary>
        /// <param name="a"> The rotation angle in radians </param>
        /// <returns> The rotation transformation. </returns>
        public static Transformation rotationZ(float theta)
        {
            return new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(-theta))
            );
        }

        /// <summary>
        /// Composition of transformations
        /// </summary>
        /// <param name="A"> Transformation you want to operate last</param>
        /// <param name="B"> Transformation you want to operate first</param>
        /// <returns></returns>
        public static Transformation operator *(Transformation A, Transformation B)
            => new Transformation(Matrix4x4.Multiply(A.M, B.M), Matrix4x4.Multiply(B.Minv, A.Minv));


        public static Point operator *(Transformation A, Point p)
        {
            Point pnew = new Point(p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13 + A.M.M14,
                                    p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23 + A.M.M24,
                                    p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33 + A.M.M34);

            float w = p.x * A.M.M41 + p.y * A.M.M42 + p.z * A.M.M43 + A.M.M44;

            if (w == 1.0) return pnew;
            else return pnew / w;
        }

        public static Vec operator *(Transformation A, Vec p)
            => new Vec(p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13,
                         p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23,
                         p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33);
        
         public static Normal operator * (Transformation A, Normal p)
             => new Normal ( p.x * A.Minv.M11 + p.y * A.Minv.M21 + p.z * A.Minv.M31,
                             p.x * A.Minv.M12 + p.y * A.Minv.M22 + p.z * A.Minv.M32,
                             p.x * A.Minv.M13 + p.y * A.Minv.M23 + p.z * A.Minv.M33 );
         
        

    } // end of Transformation


} // end of Geometry