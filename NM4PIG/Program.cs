using System;
using Color;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! I love colors");

            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Colore sum=col1+col2, diff=col1-col2;
            Console.WriteLine("sum=("+sum.r+","+sum.g+","+sum.b+")");
            Console.WriteLine("diff=("+diff.r+","+diff.g+","+diff.b+")");
            Colore purple = new Colore(1.3, 4.65, 5.22);

            Console.WriteLine("Green component: " + purple.g);

          

            Console.WriteLine("Green product component: " + purple.g);

        }
    }
}
