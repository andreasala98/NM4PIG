using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine(HdrImage.isLittleEndian("1.0"));
            Console.WriteLine(HdrImage.isLittleEndian("-1.0"));
            Console.WriteLine("OK");
        }
    }
}

