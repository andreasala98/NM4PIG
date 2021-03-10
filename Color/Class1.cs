using System;

namespace Color
{
    public struct Colore
    {
        public double r;
        public double g;
        public double b;


        public Colore (double Red, double Green, double Blue)
        {
            this.r = Red;
            this.g = Green;
            this.b = Blue;
        }

        // Add, Mul, Scalar, Diff da fare



        //Product of two colores

        public static Colore operator* (Colore A, Colore B)
        {
            return new Colore(A.r * B.r, A.g * B.g, A.b * B.b);
        }






    }
}
