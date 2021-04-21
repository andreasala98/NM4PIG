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
    public struct Point
    {
        // data members
        public float x;
        public float y;
        public float z;

        // constructor
        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // convert to string
        public override string ToString() => $"Point(x={this.x}, y={this.y}, z={this.z})";

        // isClose method to Tests purposes
        private static bool isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        public bool isClose(Point A)
           => isClose(this.x, A.x) && isClose(this.y, A.y) && isClose(this.z, A.z);


        // sum between Point and Vec, which gives back a Point
        public static Point operator +(Point p, Vec v)
            => new Point(p.x + v.x, p.y + v.y, p.z + v.z);

        // difference between Point and Vec, which gives back a Point
        public static Point operator -(Point p, Vec v)
            => new Point(p.x - v.x, p.y - v.y, p.z - v.z);

        // difference between two Points, which gives back a Vec
        public static Vec operator -(Point p, Point v)
            => new Vec(p.x - v.x, p.y - v.y, p.z - v.z);

        public static Point operator /(Point a, float alfa)
        {
            if (alfa==0) throw new DivideByZeroException("You cannot divide a point by zero!");
            return new Point (a.x / alfa, a.y / alfa, a.z / alfa);
        }

    }

    public struct Vec
    {
        //Data members
        public float x;
        public float y;
        public float z;

        //Constructor
        public Vec(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        //Sum of two vectors
        public static Vec operator +(Vec a, Vec b)
            => new Vec(a.x + b.x, a.y + b.y, a.z + b.z);

        //Difference of two vectors
        public static Vec operator -(Vec a, Vec b)
            => new Vec(a.x - b.x, a.y - b.y, a.z - b.z);

        // Product and division for a scalar    
        public static Vec operator *(float alfa, Vec a)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator *(Vec a, float alfa)
            => new Vec(a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator /(Vec a, float alfa)
        {
            if (alfa == 0) throw new DivideByZeroException("You cannot divide a vector by zero!");
            return new Vec(a.x / alfa, a.y / alfa, a.z / alfa);
        }

        // Scalar and cross product
        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public Vec crossProd(Vec b)
          => new Vec(this.y * b.z - this.z * b.y,
                       this.z * b.x - this.x * b.z,
                       this.x * b.y - this.y * b.x);


        // Squared norm and norm
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

        //Method for debugging
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        //Method for checking closeness in tests
        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        public bool isClose(Vec vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);


    }

    public struct Normal
    {
        public float x, y, z;

        public Normal (float ax, float ay, float az)
        {
            this.x = ax;
            this.y = ay;
            this.z = az;
        }
    }

    public struct Transformation
    {
        public Matrix4x4 M;
        public Matrix4x4 Minv;


        public Transformation(int a)
        {
            this.M = Matrix4x4.Identity;
            this.Minv = Matrix4x4.Identity;
        }

        public Transformation(Matrix4x4 myMat, Matrix4x4 myInvMat)

        {
            this.M = myMat;
            this.Minv = myInvMat;
        }


        public bool isConsistent()
        {
            return (this.M * this.Minv).isClose(Matrix4x4.Identity);
        }

        public static Transformation rotationX(float theta)
        {
            return new Transformation(
                Matrix4x4.CreateRotationX(theta),
                Matrix4x4.CreateRotationX(-theta)
            );
        }

        public static Transformation rotationY(float theta)
        {
            return new Transformation(
                Matrix4x4.CreateRotationY(theta),
                Matrix4x4.CreateRotationY(-theta)
            );
        }

        public static Transformation rotationZ(float theta)
        {
            return new Transformation(
                Matrix4x4.CreateRotationZ(theta),
                Matrix4x4.CreateRotationZ(-theta)
            );
        }
        
        public static Transformation operator * (Transformation A, Transformation B)
            => new Transformation(Matrix4x4.Multiply(A.M, B.M) , Matrix4x4.Multiply(B.Minv, A.Minv));
        

        public static Point operator * (Transformation A, Point p)
        {
            Point pnew = new Point (p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13 + A.M.M14,
                                    p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23 + A.M.M24,
                                    p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33 + A.M.M34 );

            float w = p.x * A.M.M41 + p.y * A.M.M42 + p.z * A.M.M43 + A.M.M44;

            if (w == 1.0) return pnew;
            else return pnew / w;
        }

        public static Vec operator * (Transformation A, Vec p)
            => new Vec ( p.x * A.M.M11 + p.y * A.M.M12 + p.z * A.M.M13,
                         p.x * A.M.M21 + p.y * A.M.M22 + p.z * A.M.M23,
                         p.x * A.M.M31 + p.y * A.M.M32 + p.z * A.M.M33 );

        
         public static Normal operator * (Transformation A, Normal p)
             => new Normal ( p.x * A.Minv.M11 + p.y * A.Minv.M21 + p.z * A.Minv.M31,
                             p.x * A.Minv.M12 + p.y * A.Minv.M22 + p.z * A.Minv.M32,
                             p.x * A.Minv.M13 + p.y * A.Minv.M23 + p.z * A.Minv.M33 );
         
        

        
    } // end of Transformation


} // end of Geometry