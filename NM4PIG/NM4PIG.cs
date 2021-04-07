using System;
using System.IO;
using Trace;
using System.Globalization;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {

            Parameters readParam = new Parameters();
            try
            {
                readParam.parseCommandLine(args);

            }
            catch (CommandLineException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            string inputPfmFileName = readParam.inputPfmFileName;
            float factor = readParam.factor;
            float gamma = readParam.gamma;
            string outputFileName = readParam.outputFileName;
            string fmt = readParam.outputFormat;

            HdrImage myImg = new HdrImage();

            try
            {
                using (FileStream inputStream = File.OpenRead(inputPfmFileName))
                {
                    myImg.readPfm(inputStream);
                    Console.WriteLine($"File {inputPfmFileName} has been correctly read from disk.");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            try
            {
                myImg.normalizeImage(factor);
                myImg.clampImage();
                myImg.writeLdrImage(outputFileName, fmt, gamma);

                Console.WriteLine($"File {outputFileName} has been correctly written to disk.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            return;
        }

        class Parameters
        {
            public string inputPfmFileName;
            public float factor;
            public float gamma;
            public string outputFileName;
            public string outputFormat;

            public Parameters()
            {
                this.inputPfmFileName = "";
                this.factor = 0.2f;
                this.gamma = 1.0f;
                this.outputFileName = "";
                this.outputFormat = "";
            }

            public void parseCommandLine(string[] args)
            {
                if (args.Length != 4)
                {
                    throw new CommandLineException("Invalid arguments specified.\nUsage: dotnet run <inputFile.pfm> <factor> <gamma> <outputFile.png/jpg>");
                }

                this.inputPfmFileName = args[0];
                this.outputFileName = args[3];

                try
                {
                    this.factor = float.Parse(args[1], CultureInfo.InvariantCulture);
                    this.gamma = float.Parse(args[2], CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException("Factor or gamma argument is not a float. Please enter some numbers");
                }


                this.outputFormat = this.outputFileName.Substring(outputFileName.Length - 3, 3);

            }

        }

    }
}


