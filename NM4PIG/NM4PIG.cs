using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyImg = new HdrImage(3, 2);

            MyImg.setPixel(0, 0, new Color(10.0f, 20.0f, 30.0f));
            MyImg.setPixel(1, 0, new Color(40.0f, 50.0f, 60.0f));
            MyImg.setPixel(2, 0, new Color(70.0f, 80.0f, 90.0f));
            MyImg.setPixel(0, 1, new Color(100.0f, 200.0f, 300.0f));
            MyImg.setPixel(1, 1, new Color(400.0f, 500.0f, 600.0f));
            MyImg.setPixel(2, 1, new Color(700.0f, 800.0f, 900.0f));

            string fileName = "file.pfm";

            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                MyImg.savePfm(fileStream);
            }

            using (MemoryStream memStream = new MemoryStream(84))
            {
                MyImg.savePfm(memStream);
                Console.WriteLine("Buffer:");
                Console.WriteLine(BitConverter.ToString(memStream.ToArray()));

            }

                Console.WriteLine($"Saved {fileName}");
        }
    }
}
    