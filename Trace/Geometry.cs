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