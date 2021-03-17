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

            using (Stream fileStream = File.OpenWrite("file.pfm"))
            {
                img.savePfm(fileStream);
            }
        }
    }
}
    