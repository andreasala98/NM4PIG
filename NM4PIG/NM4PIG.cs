/*
The MIT License (MIT)

Copyright © 2021 Tommaso Armadillo, Pietro Klausner, Andrea Sala

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the “Software”), to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of
the Software. THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/

using System;
using System.IO;
using Trace;
using System.Globalization;
using System.Numerics;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {

            /*
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
            */



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


