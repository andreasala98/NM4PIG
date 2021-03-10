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

        public static Colore operator +(Colore col1, Colore col2){
            return new Colore(col1.r+col2.r, col1.g+col2.g, col1.b+col2.b);
        }

        public static Colore operator -(Colore col1, Colore col2){
            return new Colore(col1.r-col2.r, col1.g-col2.g, col1.b-col2.b);
        }
        // Add, Mul, Scalar, Diff da fare
    }
}
