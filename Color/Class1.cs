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

        //Sum of two colores
        public static Colore operator +(Colore col1, Colore col2){
            return new Colore(col1.r+col2.r, col1.g+col2.g, col1.b+col2.b);
        }

        //Difference of two colores
        public static Colore operator -(Colore col1, Colore col2){
            return new Colore(col1.r-col2.r, col1.g-col2.g, col1.b-col2.b);
        }
        
        // Scalar product
        public static Colore operator* (Colore a, double alfa)
            => new Colore (a.r * alfa, a.g * alfa, a.b *alfa);

        public static Colore operator* (double alfa, Colore a)
            => new Colore (a.r * alfa, a.g * alfa, a.b *alfa);
        

        //Product of two colores

        public static Colore operator* (Colore A, Colore B)
        {
            return new Colore(A.r * B.r, A.g * B.g, A.b * B.b);
        }

        // are close method

        public bool is_close (double a, double b)
        {
            double epsilon = 1e-8;
            return Math.Abs(a - b) < epsilon;
        }

        public bool are_close (Colore A, Colore B)
        {
            return is_close(A.r, B.r) && is_close(A.b, B.b) && is_close(A.g, B.g); 
        }   

    }
}
