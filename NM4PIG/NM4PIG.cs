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

            if (args.Length != 4) throw new ArgumentException("Wrong number of parameters!");

            string inputPfmFileName="", outputFileName="";
            float factor, gamma;
            HdrImage myImg = new HdrImage();

            try
            {
                inputPfmFileName = args[0];
                factor = System.Convert.ToSingle(args[1]);
                gamma = System.Convert.ToSingle(args[2]);
                outputFileName = args[3];
            }

            catch (FormatException ex)
            {
                Console.WriteLine("Invalid arguments specified.");
                Console.WriteLine("Usage: dotnet run <inputFile.pfm> <factor> <gamma> <outputFile.yourformat>");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("Hello world!");

            using (FileStream inputStream = File.OpenRead(inputPfmFileName))
            {
                myImg.readPfm(inputStream);
            }

            Console.WriteLine($"File {inputPfmFileName} correctly read from disk.");
            myImg.normalizeImage(factor);
            myImg.clampImage();

            using (FileStream outputStream = File.OpenWrite(outputFileName))
            {
                myImg.writeLdrImage(outputStream);
            }
            Console.WriteLine($"File {outputFileName} correctly written to disk.");


        }
    }
}

