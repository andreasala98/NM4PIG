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
        public float x;
        public float y;
        public float z;

        public Vec(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

}