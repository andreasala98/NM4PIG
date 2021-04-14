using System;

namespace Trace
{
    public struct Point
    {
        public float x;
        public float y;
        public float z;

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // convert to string
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        // isClose method to Tests purposes
        public static bool isClose(float a, float b)
        {
            var epsilon = 1e-8F;
            return Math.Abs(a - b) < epsilon;
        }

        public bool isClose(Point A)
        {
            return isClose(this.x, A.x) && isClose(this.y, A.y) && isClose(this.z, A.z);
        }

        // sum between Point and Vector, which gives back a Point
        public static Point operator +(Point p, Vec v) 
            => new Point(p.x + v.x, p.y + v.y, p.z + v.z);

        // difference between Point and Vector, which gives back a Point
        public static Point operator +(Point p, Vec v) 
            => new Point(p.x - v.x, p.y - v.y, p.z - v.z);
            
        // difference between two Points, which gives back a Vector
        public static Point operator -(Point p, Point v) 
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

        //Method for debugging
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        //Method for checking closeness in tests
        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        public bool isClose(Vec vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);

        //Sum of two vectors
        public static Vec operator +(Vec a, Vec b)
            => new Vec(a.x + b.x, a.y + b.y, a.z + b.z);

        //Difference of two vectors
        public static Vec operator -(Vec a, Vec b)
            => new Vec(a.x - b.x, a.y - b.y, a.z - b.z);
    }

}