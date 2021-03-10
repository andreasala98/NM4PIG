using System;

namespace Trace
{
    public struct Color
    {
        public double r;
        public double g;
        public double b;


        public Color (double Red, double Green, double Blue)
        {
            this.r = Red;
            this.g = Green;
            this.b = Blue;
        }

        //Sum of two Colors
        public static Color operator +(Color col1, Color col2){
            return new Color(col1.r+col2.r, col1.g+col2.g, col1.b+col2.b);
        }

        //Difference of two Colors
        public static Color operator -(Color col1, Color col2){
            return new Color(col1.r-col2.r, col1.g-col2.g, col1.b-col2.b);
        }
        
        // Scalar product
        public static Color operator* (Color a, double alfa)
            => new Color (a.r * alfa, a.g * alfa, a.b *alfa);

        public static Color operator* (double alfa, Color a)
            => new Color (a.r * alfa, a.g * alfa, a.b *alfa);
        

        //Product of two Colors

        public static Color operator* (Color A, Color B)
        {
            return new Color(A.r * B.r, A.g * B.g, A.b * B.b);
        }

        // are close method

        public bool isClose (double a, double b)
        {
            double epsilon = 1e-8;
            return Math.Abs(a - b) < epsilon;
        }

        public bool isClose (Color A)
        {
            return isClose(this.r, A.r) && isClose(this.b, A.b) && isClose(this.g, A.g); 
        }   

    }
}
