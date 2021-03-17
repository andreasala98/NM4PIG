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

            string fileName = "blackFile.pfm";

            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                MyImg.savePfm(fileStream);
            }

                Console.WriteLine($"Saved {fileName}");
        }
    }
}
    