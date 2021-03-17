using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyImg = new HdrImage(3, 2); //(7, 4);

            //img.setPixel(0, 1, new Color(3.0, 3.0, 3.0));

            MyImg.setPixel(0, 0, new Color(10.0, 20.0, 30.0));
            MyImg.setPixel(1, 0, new Color(40.0, 50.0, 60.0));
            MyImg.setPixel(2, 0, new Color(70.0, 80.0, 90.0));
            MyImg.setPixel(0, 1, new Color(100.0, 200.0, 300.0));
            MyImg.setPixel(1, 1, new Color(400.0, 500.0, 600.0));
            MyImg.setPixel(2, 1, new Color(700.0, 800.0, 900.0));

            using (Stream fileStream = File.OpenWrite("file.pfm"))
            {
                MyImg.savePfm(fileStream);
            }

            Console.WriteLine("Saved file.pfm");
        }
    }
}
    