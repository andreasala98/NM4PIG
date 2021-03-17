using System;
using Trace;
using System.IO;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            var img = new HdrImage(7, 4);

            igm.setPixel(0, 1, Color(3, 3, 3));

            using (Stream fileStream = File.OpenWrite("file.pfm"))
            {
                img.savePfm(fileStream);
            }

            Console.WriteLine("Saved file.pfm")
        }
    }
}
    