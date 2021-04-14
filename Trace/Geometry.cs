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
        public float x;
        public float y;
        public float z;

        public Vec(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vec operator *(float alfa, Vec a)
            => new Vec (a.x * alfa, a.y * alfa, a.z * alfa);

        public static Vec operator /(float alfa, Vec a)
            => new Vec (a.x / alfa, a.y / alfa, a.z / alfa);

        public static float operator *(Vec a, Vec b)
            => a.x * b.x + a.y * b.y + a.z * b.z;

        public static Vec crossProd (Vec a, Vec b)
           => new Vec (x = a.y * b.z - a.z * b.y, 
                       y = a.z * b.x - a.x * b.z,
                       z = a.x * b.y - a.y * b.x);

        public float getSquaredNorm ()
            => this * this;

        public float getNorm ()
            => sqrt(this.getSquaredNorm());

        public Vec Normalize()
            => this/norm;
        

    }

}