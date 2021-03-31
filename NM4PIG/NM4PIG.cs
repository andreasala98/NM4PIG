using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length != 4)
            {

                Console.WriteLine("Wrong number of parameters. 4 parameters are required.");
                return;

            }


            string inputPfmFileName = "", outputFileName = "";
            float factor = 1f, gamma = 1f;
            HdrImage myImg = new HdrImage();

            try
            {

                inputPfmFileName = args[0];
                outputFileName = args[3];

                try { factor = System.Convert.ToSingle(args[1]); }
                catch (FormatException)
                {
                    Console.WriteLine("Factor argument is not a float. It will be set to 1");
                    factor = 1f;
                }

                try { gamma = System.Convert.ToSingle(args[2]); }
                catch (CommandLineException)
                {
                    Console.WriteLine("Gamma argument is not a float. It will be set to 1");
                    gamma = 1f;
                }

            }
            catch (CommandLineException)
            {
                Console.WriteLine("Invalid arguments specified.");
                Console.WriteLine("Usage: dotnet run <inputFile.pfm> <factor> <gamma> <outputFile.yourformat>");
            }


            try
            {
                using (FileStream inputStream = File.OpenRead(inputPfmFileName))
                {
                    myImg.readPfm(inputStream);
                    Console.WriteLine($"File {inputPfmFileName} correctly read from disk.");
                }

            }
            catch (CommandLineException)
            {
                Console.WriteLine("File or directory not found");
            }


            myImg.normalizeImage(factor);
            myImg.clampImage();

            try
            {

                myImg.writeLdrImage(outputFileName, "jpg", gamma);

                Console.WriteLine($"File {outputFileName} correctly written to disk.");
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (NotSupportedException e)
            {
                Console.WriteLine(e.Message);
            }



            return;
        }
    }
}

