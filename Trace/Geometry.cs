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
            => new Vec (a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator *(Vec a, float alfa)
            => new Vec (a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator /(Vec a, float alfa)
        {
            if (alfa==0) throw new DivideByZeroException("You cannot divide a vector by zero!");
            return new Vec (a.x / alfa, a.y / alfa, a.z / alfa);
        }

        // Scalar and cross product
        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public Vec crossProd (Vec b)
          => new Vec ( this.y * b.z - this.z * b.y, 
                       this.z * b.x - this.x * b.z,
                       this.x * b.y - this.y * b.x );
        

        // Squared norm and norm
        public float getSquaredNorm ()
            => this * this;

        public float getNorm ()
            => (float)Math.Sqrt(this.getSquaredNorm());

        // Normalize vector
        public Vec Normalize()
            => this/this.getNorm();
        
        //Method for debugging
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        //Method for checking closeness in tests
        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        public bool isClose(Vec vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);

       
    }

}