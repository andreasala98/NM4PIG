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
        
         //Sum of two vectors
        public static Vec operator +(Vec a, Vec b)
            => new Vec(a.x + b.x, a.y + b.y, a.z + b.z);

        //Difference of two vectors
        public static Vec operator -(Vec a, Vec b)
            => new Vec(a.x - b.x, a.y - b.y, a.z - b.z);

        // Product and division for a scalar    
        public static Vec operator *(float alfa, Vec a)
            => new Vec (a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator /(float alfa, Vec a){
            if (alfa==0) throw new Exception("DivisionByZero Error");
            return new Vec (a.x / alfa, a.y / alfa, a.z / alfa);
        }

        // Scalar and cross product
        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public static Vec crossProd (Vec a, Vec b)
           => new Vec (x = a.y * b.z - a.z * b.y, 
                       y = a.z * b.x - a.x * b.z,
                       z = a.x * b.y - a.y * b.x);

        // Squared norm and norm
        public float getSquaredNorm ()
            => this * this;

        public float getNorm ()
            => sqrt(this.getSquaredNorm());

        // Normalize vector
        public Vec Normalize()
            => this/norm;
        
        //Method for debugging
        public override string ToString() => $"Vec(x={this.x}, y={this.y}, z={this.z})";

        //Method for checking closeness in tests
        private static bool _isClose(float a, float b, float? epsilon = 1e-8f)
            => Math.Abs(a - b) < epsilon;

        public bool isClose(Vec vector)
            => _isClose(this.x, vector.x) && _isClose(this.y, vector.y) && _isClose(this.z, vector.z);

       
    }

}