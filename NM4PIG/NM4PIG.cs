using System;
using System.IO;
using System.Linq;
using Trace;
using System.Collections.Generic;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {

            HdrImage MyImg = new HdrImage("blackFile.pfm");

            MyImg.setPixel(0, 0, new Color(0f, 0f, 100f));
            string fName = "outFile.pfm";

            using (FileStream fs = File.OpenWrite(fName))
            {
                MyImg.savePfm(fs);
            }

            Console.WriteLine($"File {fName} correctly saved.");
        }
    }
}

