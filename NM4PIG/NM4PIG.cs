using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyImg = new HdrImage(4, 4);

           
            string fileName = "blackFile.pfm";

            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                MyImg.savePfm(fileStream);
            }

            Console.WriteLine($"{fileName} correctly saved!");
        }
    }
}
