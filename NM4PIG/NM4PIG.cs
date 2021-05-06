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
using Microsoft.Extensions.CommandLineUtils;

namespace NM4PIG
{

    class Program
    {

        public static void Main(params string[] args)
        {
            
            CommandLineApplication CLI = new CommandLineApplication(throwOnUnexpectedArg: false);
            CommandArgument demo_arg = null;
            CommandArgument conv_arg = null;
            CLI.Command("demo",
            (target) =>
              demo_arg = target.Argument("demo", "Enter demo mode and visualize image", multipleValues: false));

            CLI.Command("convert",
            (target) =>
              conv_arg = target.Argument("convert", "Enter convert mode and convert an input pfm file into a jpg/png file", multipleValues: false));

           /* CommandOption width  = CLI.Option("-w | --width",  "width of the image" , CommandOptionType.SingleValue);
            CommandOption height = CLI.Option("-h | --height", "height of the image", CommandOptionType.SingleValue);
            */
            CommandOption size = CLI.Option("--size", "width an height of the iamge", CommandOptionType.MultipleValue);
            CommandOption angledeg = CLI.Option("--angle-deg | --angle", "field-of-view angle", CommandOptionType.SingleValue);
            CommandOption orthogonal = CLI.Option("--orthogonal | --orth", "Use an orthogonal camera", CommandOptionType.NoValue);
            CommandOption pfmfile = CLI.Option("--pfmfile", "name of .pfm output file", CommandOptionType.SingleValue);
            CommandOption ldrfile = CLI.Option("--ldrfile", "name of .png/.jpg output file", CommandOptionType.SingleValue);

            CLI.HelpOption("-? | -h | --help");

            CLI.OnExecute(
            () => {
                    if (demo_arg!=null) Demo(args);
                    if (conv_arg!=null) Convert(args);
                    return 0;
                  }
            );

            CLI.Execute(args);
            Console.WriteLine("Hello world!");

            return;
            
        } //Main

        // ############################################# //

        public static void Demo(string[] args) {

             Console.WriteLine("Demo branch entered");
             
              }

        // ############################################# //
        public static void Convert(string[] args) {

            Console.WriteLine("Convert branch entered");

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
              
        }


    } //Program class

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
                if (args.Length != 5) //-g counts as an argument
                {
                    throw new CommandLineException("Invalid arguments specified.\nUsage: dotnet run -g <inputFile.pfm> <factor> <gamma> <outputFile.png/jpg>");
                }

                this.inputPfmFileName = args[1];
                this.outputFileName = args[4];

                try
                {
                    this.factor = float.Parse(args[2], CultureInfo.InvariantCulture);
                    this.gamma = float.Parse(args[3], CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException("Factor or gamma argument is not a float. Please enter floating-point numbers");
                }


                this.outputFormat = this.outputFileName.Substring(outputFileName.Length - 3, 3);



            } //parseCommandLine

        } //Parameters class

} //NM4PIG namespace


