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
        public override string ToString() => $"Point=(x={this.x}, y={this.y}, z={this.z})";

        /// <summary>
        /// Boolean to check if two Points are close enough
        /// </summary>
        /// <param name="A"> The other Point</param>
        /// <returns>True if the Points are close</returns> 
        public bool isClose(Point A)
           => Utility.areClose(this.x, A.x) && Utility.areClose(this.y, A.y) && Utility.areClose(this.z, A.z);

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
        /// Calculate the vector connecting two <see cref="Point"/> objects.
        /// </summary>
        /// <param name="p"> A <see cref="Point"/> object.</param>
        /// <param name="v"> A <see cref="Point"/> object.</param>
        /// <returns> A <see cref="Vec"/> object.</returns>
        public static Vec operator -(Point p, Point v)
            => new Vec(p.x - v.x, p.y - v.y, p.z - v.z);

        /// <summary>
        /// Divide a <see cref="Point"/> by a scaling factor.
        /// </summary>
        /// <param name="a"> The <see cref="Point"/></param>
        /// <param name="alfa"> Scaling factor. </param>
        /// <returns> The scaled <see cref="Point"/>.</returns>
        public static Point operator /(Point a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a point by zero!");
            return new Point(a.x / alfa, a.y / alfa, a.z / alfa);
        }

        /// <summary>
        /// Convert a <see cref="Point"/> to <see cref="Vec"/>
        /// e.g. Point(1,2,3) to Vec(1,2,3)
        /// </summary>
        /// <returns><see cref="Vec"/> version of <see cref="Point"/></returns>
        public Vec toVec()
           => new Vec(this.x, this.y, this.z);

        /// <summary>
        /// Convert a <see cref="Point"/> to <see cref="Vec"/>
        /// e.g. Point(1,2,3) to List<float>(1,2,3)
        /// </summary>
        /// <returns><see cref="List"/> version of <see cref="Point"/></returns>
        public List<float> ToList()
            => new List<float>() { this.x, this.y, this.z };
    }

    /// <summary>
    ///  Value type to represent a vector in a 3D space.
    /// </summary>
    public struct Vec
    {
        /// <summary>
        /// x coordinate of the vector.
        /// </summary>
        public float x;
        /// <summary>
        /// y coordinate of the vector.
        /// </summary>
        public float y;
        /// <summary>
        /// z coordinate of the vector.
        /// </summary>
        public float z;

        /// <summary>
        /// Default constructor for <see cref="Vec"/>.
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
        /// Sum of two <see cref="Vec"/> objects.
        /// </summary>
        /// <param name="a"> First <see cref="Vec"/></param>
        /// <param name="b"> Second <see cref="Vec"/></param>
        /// <returns> A <see cref="Vec"/> sum </returns>
        public static Vec operator +(Vec a, Vec b)
            => new Vec(a.x + b.x, a.y + b.y, a.z + b.z);

        /// <summary>
        /// Difference of two <see cref="Vec"/> objects
        /// </summary>
        /// <param name="a"> First <see cref="Vec"/></param>
        /// <param name="b"> Second vec<see cref="Vec"/>tor</param>
        /// <returns> A <see cref="Vec"/> difference </returns>
        public static Vec operator -(Vec a, Vec b)
            => new Vec(a.x - b.x, a.y - b.y, a.z - b.z);

        /// <summary>
        /// Multiplication <see cref="Vec"/> - scalar
        /// </summary>
        /// <param name="a">  <see cref="Vec"/> object</param>
        /// <param name="alfa">  Scaling factor </param>
        /// <returns> A scaled <see cref="Vec"/> object </returns>
        public static Vec operator *(float alfa, Vec a)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);

        /// <summary>
        /// Multiplication <see cref="Vec"/> - scalar
        /// </summary>
        /// <param name="a">  <see cref="Vec"/> object </param>
        /// <param name="alfa">  Scaling factor </param>
        /// <returns> A scaled <see cref="Vec"/>. </returns>
        public static Vec operator *(Vec a, float alfa)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);

        /// <summary>
        /// Divide a <see cref="Vec"/> by a scaling factor.
        /// </summary>
        /// <param name="a"> The <see cref="Vec"/>.</param>
        /// <param name="alfa"> Scaling factor. </param>
        /// <returns> The scaled <see cref="Vec"/>.</returns>
        public static Vec operator /(Vec a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a vector by zero!");
            return new Vec(a.x / alfa, a.y / alfa, a.z / alfa);
        }

        /// <summary>
        ///  Euclidean scalar product between two <see cref="Vec"/>s.
        /// </summary>
        /// <param name="a"> First <see cref="Vec"/> </param>
        /// <param name="b"> Second <see cref="Vec"/></param>
        /// <returns> Scalar product in float format.</returns>
        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public static float operator *(Vec a, Normal b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        /// <summary>
        /// Change sign to all the components
        /// </summary>
        public static Vec operator -(Vec vec)
            => new Vec(-vec.x, -vec.y, -vec.z);

        /// <summary>
        ///  Cross product between two 3D <see cref="Vec"/>s.
        /// </summary>
        /// <param name="b"> Second <see cref="Vec"/></param>
        /// <returns> Cross product in <see cref="Vec"/> format.</returns>
        public Vec crossProd(Vec b)
            => new Vec(this.y * b.z - this.z * b.y,
                       this.z * b.x - this.x * b.z,
                       this.x * b.y - this.y * b.x);

        /// <summary>
        /// Squared norm of the <see cref="Vec"/>
        /// </summary>
        /// <returns> The squared norm of the <see cref="Vec"/> as float</returns>
        public float getSquaredNorm()
            => this * this;

        /// <summary>
        /// Norm of the <see cref="Vec"/>
        /// </summary>
        /// <returns> The norm of the <see cref="Vec"/> as float</returns>
        public float getNorm()
            => (float)Math.Sqrt(this.getSquaredNorm());

        /// <summary>
        /// Normalize the <see cref="Vec"/>
        /// </summary>
        /// <returns>The <see cref="Vec"/>, normalized</returns>
        public Vec Normalize()
            => this / this.getNorm();

        /// <summary>
        /// It checks if the <see cref="Vec"/> is normalized. It is used for debugging purpose
        /// </summary>
        public bool isNormalized()
            => this.getNorm() == 1.0f;

        /// <summary>
        /// Converts a <see cref="Vec"/> to a string for printing.
        /// </summary>
        /// <returns> A string in the format Vec(x=" ",y=" ",z=" ")</returns>
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        /// <summary>
        /// Boolean to check if two <see cref="Vec"/>s are close enough
        /// </summary>
        /// <param name="vector"> The other <see cref="Vec"/></param>
        /// <returns>True if the <see cref="Vec"/>s are close</returns> 
        public bool isClose(Vec vector)
            => Utility.areClose(this.x, vector.x) && Utility.areClose(this.y, vector.y) && Utility.areClose(this.z, vector.z);

        public List<float> ToList()
            => new List<float>() { this.x, this.y, this.z };

        public List<Vec> createONBfromZ()
        {
            Vec e3 = this.Normalize();
            float sign = MathF.CopySign(1f, e3.z);
            float a = -1.0f / (sign + e3.z);
            float b = e3.x * e3.y * a;

            Vec e1 = new Vec(1.0f + sign * e3.x * e3.x * a, sign * b, -sign * e3.x);
            Vec e2 = new Vec(b, sign + e3.y * e3.y * a, -e3.y);

            return new List<Vec>() { e1, e2, e3 };
        }

        public Point ToPoint()
        {
            return new Point(this.x, this.y, this.z);
        }

    }


    /// <summary>
    /// A 2D vector used to represent a point on a surface
    /// The fields are named `u` and `v` to distinguish them from the usual 3D coordinates `x`, `y`, `z`.
    /// </summary>
    public struct Vec2D
    {
        public float u, v;

        public Vec2D(float a, float b)
        {
            this.u = a;
            this.v = b;
        }

        public bool isClose(Vec2D vector)
           => Utility.areClose(this.u, vector.u) && Utility.areClose(this.v, vector.v);

        public override string ToString()
        {
            return "Vec2D :(" + this.u + ", " + this.v + ")";
        }

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
        /// Divide a <see cref="Normal"/> by a scaling factor.
        /// </summary>
        /// <param name="a"> The <see cref="Normal"/>.</param>
        /// <param name="alfa"> Scaling factor. </param>
        /// <returns> The scaled <see cref="Normal"/>.</returns>
        public static Normal operator /(Normal a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a vector by zero!");
            return new Normal(a.x / alfa, a.y / alfa, a.z / alfa);
        }

        public static Normal operator *(Normal a, float alfa)
        {
            return new Normal(a.x * alfa, a.y * alfa, a.z * alfa);
        }

        /// <summary>
        ///  Euclidean scalar product between two <see cref="Normal"/>s.
        /// </summary>
        /// <param name="a"> First <see cref="Normal"/> </param>
        /// <param name="b"> Second <see cref="Normal"/></param>
        /// <returns> Scalar product in float format.</returns>
        public static float operator *(Normal a, Normal b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        /// <summary>
        /// Squared norm of the <see cref="Normal"/>
        /// </summary>
        /// <returns> The squared norm of the <see cref="Normal"/> as float</returns>
        public float getSquaredNorm()
            => this * this;

        /// <summary>
        /// Norm of the <see cref="Normal"/>
        /// </summary>
        /// <returns> The norm of the <see cref="Normal"/> as float</returns>
        public float getNorm()
            => (float)Math.Sqrt(this.getSquaredNorm());

        /// <summary>
        /// Normalize the <see cref="Normal"/>
        /// </summary>
        /// <returns>The <see cref="Normal"/>, normalized</returns>
        public Normal Normalize()
            => this / this.getNorm();

        /// <summary>
        /// Converts a <see cref="Normal"/> to a string for printing.
        /// </summary>
        /// <returns> A string in the format Normal(x=" ",y=" ",z=" ")</returns>
        public override string ToString() => $"Norm(x={this.x}, y={this.y}, z={this.z})";

        /// <summary>
        /// Convert a Normal to a Vec
        /// </summary>
        /// <returns></returns>
        public Vec toVec() => new Vec(this.x, this.y, this.z);

        /// <summary>
        /// Change sign to all the components
        /// </summary>
        public static Normal operator -(Normal normal)
            => new Normal(-normal.x, -normal.y, -normal.z);

        /// <summary>
        /// Boolean to check if two <see cref="Normal"/>s are close enough
        /// </summary>
        /// <param name="vector"> The other <see cref="Normal"/></param>
        /// <returns>True if the <see cref="Normal"/>s are close</returns> 
        public bool isClose(Normal vector)
            => Utility.areClose(this.x, vector.x) && Utility.areClose(this.y, vector.y) && Utility.areClose(this.z, vector.z);

        /// <summary>
        /// Transform <see cref="Normal"/> into <see cref="Vec"/>
        /// </summary>
        public Vec ToVec()
            => new Vec(this.x, this.y, this.z);


        public List<Vec> createONBfromZ()
        {
            Vec e3 = this.toVec().Normalize();

            float sign = MathF.CopySign(1f, e3.z);
            float a = -1.0f / (sign + e3.z);
            float b = e3.x * e3.y * a;

            Vec e1 = new Vec(1.0f + sign * e3.x * e3.x * a, sign * b, -sign * e3.x);
            Vec e2 = new Vec(b, sign + e3.y * e3.y * a, -e3.y);


            return new List<Vec>() { e1, e2, e3 };
        }

    } // end of Normal

    /// <summary>
    ///  Affine transformation. It is represented by a 4x4 matrix. 
    ///  It can be used to create translations, rotations and scalings.
    /// </summary>
    public struct Transformation
    {
        /// <summary>
        /// <see cref="Matrix4x4"/> representing the transformation.
        /// </summary>
        public Matrix4x4 M;

        /// <summary>
        /// <see cref="Matrix4x4"/> representing the inverse transformation.
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
        /// Constructor taking a <see cref="Matrix4x4"/> and its inverse as inputs.
        /// </summary>
        /// <param name="myMat"> The <see cref="Matrix4x4"/> representing the transformation. </param>
        /// <param name="myInvMat"> The inverse <see cref="Matrix4x4"/>.</param>
        public Transformation(Matrix4x4 myMat, Matrix4x4 myInvMat)
        {
            this.M = myMat;
            this.Minv = myInvMat;
        }

        /// <summary>
        /// Switches the M and Minv fields. Practically, inverts the transformation.
        /// </summary>
        /// <returns> The inverse <see cref="Transformation"/>. </returns>
        public Transformation getInverse()
            => new Transformation(this.Minv, this.M);

        /// <summary>
        /// Method to check if a <see cref="Transformation"/> and a <see cref="Matrix4x4"/> are close.
        /// </summary>
        /// <param name="a"> The <see cref="Matrix4x4"/> to be compared with. </param>
        /// <returns> True if Transformation and Matrix are close</returns>
        public bool isClose(Transformation a)
            => Utility.areMatricesClose(this.M, a.M) && Utility.areMatricesClose(this.Minv, a.Minv);

        /// <summary>
        /// Method to check if the Minv field actually contains the inverse matrix.
        /// </summary>
        /// <returns> True if Minv is the inverse matrix of M</returns>
        public bool isConsistent()
            => Utility.areMatricesClose(this.M * this.Minv, Matrix4x4.Identity);


        /// <summary>
        /// Translate a <see cref="Vec"/> in 3D
        /// </summary>
        /// <param name="a"> The <see cref="Vec"/> generating the translation. </param>
        /// <returns> The translation <see cref="Transformation"/>. </returns>
        public static Transformation Translation(Vec a)
            => new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateTranslation(a.x, a.y, a.z)),
                                    Matrix4x4.Transpose(Matrix4x4.CreateTranslation(-a.x, -a.y, -a.z)));


        public static Transformation Translation(float ax, float ay, float az)
        {
            Vec a = new Vec(ax, ay, az);
            return new Transformation(Matrix4x4.Transpose(Matrix4x4.CreateTranslation(a.x, a.y, a.z)),
                                    Matrix4x4.Transpose(Matrix4x4.CreateTranslation(-a.x, -a.y, -a.z)));
        }

        /// <summary>
        /// Return a <see cref="Transformation"/> object encoding a scaling
        /// </summary>
        /// <param name="a"> The <see cref="Vec"/> generating the scaling. </param>
        /// <returns> The scaling <see cref="Transformation"/>. </returns>
        public static Transformation Scaling(Vec a)
            => new Transformation(Matrix4x4.CreateScale(a.x, a.y, a.z),
                                    Matrix4x4.CreateScale(1.0f / a.x, 1.0f / a.y, 1.0f / a.z));



        /// <summary>
        /// Create a sclaing transformation by passing three floats, representing the 
        /// scaling in the tree directions x, y, z.
        /// </summary>
        /// <param name="ax"> X scaling</param>
        /// <param name="ay"> Y scaling</param>
        /// <param name="az"> Z scaling</param>
        /// <returns> A scaling transformation</returns>
        public static Transformation Scaling(float ax, float ay, float az)
           => new Transformation(Matrix4x4.CreateScale(ax, ay, az),
                                   Matrix4x4.CreateScale(1.0f / ax, 1.0f / ay, 1.0f / az));


        /// <summary>
        /// Scale an object isotropically.
        /// </summary>
        /// <param name="a">Saling factor in all directions</param>
        /// <returns> The scaling transformation</returns>
        public static Transformation Scaling(float a)
        {
            return new Transformation(Matrix4x4.CreateScale(a, a, a),
                                     Matrix4x4.CreateScale(1.0f / a, 1.0f / a, 1.0f / a));
        }

        /// <summary>
        /// Rotation along the x axis.
        /// </summary>
        /// <param name="theta"> The rotation angle in radians </param>
        /// <returns> The rotation <see cref="Transformation"/>. </returns>
        public static Transformation RotationX(float theta)
            => new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationX(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationX(-theta)));

        /// <summary>
        /// Rotation along the y axis.
        /// </summary>
        /// <param name="theta"> The rotation angle in radians </param>
        /// <returns> The rotation <see cref="Transformation"/>. </returns>
        public static Transformation RotationY(float theta)
            => new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationY(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationY(-theta)));

        /// <summary>
        /// Rotation along the z axis.
        /// </summary>
        /// <param name="theta"> The rotation angle in radians </param>
        /// <returns> The rotation <see cref="Transformation"/>. </returns>
        public static Transformation RotationZ(float theta)
            => new Transformation(
                Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(theta)),
                Matrix4x4.Transpose(Matrix4x4.CreateRotationZ(-theta)));

        /// <summary>
        /// Composition of transformations
        /// </summary>
        /// <param name="A"> left-side <see cref="Transformation"/> </param>
        /// <param name="B"> right-side <see cref="Transformation"/> </param>
        /// <returns> The composed <see cref="Transformation"/>.</returns>
        public static Transformation operator *(Transformation A, Transformation B)
            => new Transformation(Matrix4x4.Multiply(A.M, B.M), Matrix4x4.Multiply(B.Minv, A.Minv));


        /// <summary>
        /// Apply <see cref="Transformation"/> to a <see cref="Point"/>
        /// </summary>
        /// <param name="A"> <see cref ="Transformation"/> object </param>
        /// <param name="p"> <see cref="Point"/> object </param>
        /// <returns> The tranformed <see cref="Point"/> </returns>
        public static Point operator *(Transformation A, Point p)
        {
            Point pnew = new Point(p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13 + A.M.M14,
                                    p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23 + A.M.M24,
                                    p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33 + A.M.M34);

            float w = p.x * A.M.M41 + p.y * A.M.M42 + p.z * A.M.M43 + A.M.M44;

            if (w == 1.0) return pnew;
            else return pnew / w;
        }

        /// <summary>
        /// Apply <see cref="Transformation"/> to a <see cref="Vec"/>
        /// </summary>
        /// <param name="A"> <see cref="Transformation"/> object </param>
        /// <param name="p"> <see cref="Vec"/> object </param>
        /// <returns> The tranformed <see cref="Vec"/> </returns>
        public static Vec operator *(Transformation A, Vec p)
            => new Vec(p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13,
                         p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23,
                         p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33);

        /// <summary>
        /// Apply <see cref="Transformation"/> to a <see cref="Normal"/>
        /// </summary>
        /// <param name="A"> <see cref="Transformation"/> object </param>
        /// <param name="p"> <see cref="Normal"/> object </param>
        /// <returns> The tranformed <see cref="Normal"/> </returns>
        public static Normal operator *(Transformation A, Normal p)
            => new Normal(p.x * A.Minv.M11 + p.y * A.Minv.M21 + p.z * A.Minv.M31,
                            p.x * A.Minv.M12 + p.y * A.Minv.M22 + p.z * A.Minv.M32,
                            p.x * A.Minv.M13 + p.y * A.Minv.M23 + p.z * A.Minv.M33);



    } // end of Transformation


} // end of Geometry