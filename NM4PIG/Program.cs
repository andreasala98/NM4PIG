using System;
using Color;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World! I love colors");

            Colore purple = new Colore(1.3, 4.65, 5.22);
            Console.WriteLine("Green component: " + purple.g);

        }
    }
}
