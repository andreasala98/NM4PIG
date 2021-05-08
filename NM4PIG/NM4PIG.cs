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
using System.Collections.Generic;

namespace NM4PIG
{

    class Program
    {

        public static void Main(params string[] args)
        {

            // foreach (var item in args)
            //     Console.WriteLine(item.ToString());

            CommandLineApplication CLI = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                FullName = "Numerical Methods For Photorealistic Image Generation",
                Description = "Insert Description here"
            };

            CLI.Command("demo",
            command =>
            {
                command.Description = "Enter demo mode and generate a simple image";
                var width = command.Option("--width|-W <WIDTH>", "width of the generated image, default is 640", CommandOptionType.SingleValue);
                var height = command.Option("--heigth|-H <HEIGHT>", "height of the generated image, default is 480", CommandOptionType.SingleValue);
                var angledeg = command.Option("--angle|-a <ANGLE>", "field-of-view angle, default is 0", CommandOptionType.SingleValue);
                var orthogonal = command.Option("--orthogonal|-o", "Use an orthogonal camera", CommandOptionType.NoValue);
                var pfmfile = command.Option("--pfmfile|-pfm <FILENAME>", "name of .pfm output file", CommandOptionType.SingleValue);
                var ldrfile = command.Option("--ldrfile|-ldr <FILENAME>", "name of .png/.jpg output file", CommandOptionType.SingleValue);

                command.HelpOption("-?|-h|--help");
                command.OnExecute(() =>
                {
                    if (orthogonal.Value() == null)
                        Console.WriteLine("null");
                    else
                        Console.WriteLine(orthogonal.Value());
                    Demo(
                        width.HasValue() ? Int32.Parse(width.Value()) : DefaultDemo.width,
                        height.HasValue() ? Int32.Parse(height.Value()) : DefaultDemo.height,
                        angledeg.HasValue() ? Int32.Parse(angledeg.Value()) : DefaultDemo.angledeg,
                        orthogonal.HasValue() ? true : false,
                        pfmfile.HasValue() ? pfmfile.Value() : DefaultDemo.pfmFile,
                        ldrfile.HasValue() ? ldrfile.Value() : DefaultDemo.ldrFile
                    );
                    return 0;
                });
            });

            CLI.Command("convert",
            command =>
            {
                command.Description = "Enter convert mode and convert an input pfm file into a jpg/ png file";
                command.HelpOption("-?|-h|--help");
                command.OnExecute(() =>
                {
                    Convert(args);
                    return 0;
                });
            });

            CLI.HelpOption("-? | -h | --help");

            CLI.OnExecute(() =>
            {
                CLI.ShowHelp();
                return 0;
            }
            );
            CLI.Execute(args);

            return;

        } //Main

        // ############################################# //

        public static void Demo(int width, int height, int angle, bool orthogonal, string pfmFile, string ldrFile)
        {

            Console.WriteLine("Starting Demo\n\n");

            HdrImage image = new HdrImage(width, height);

            Console.WriteLine("Creating the scene...");
            World world = new World();
            List<float> Vertices = new List<float>() { -0.5f, 0.5f };
            foreach (var x in Vertices)
            {
                foreach (var y in Vertices)
                {
                    foreach (var z in Vertices)
                    {

                        world.addShape(new Sphere(Transformation.Translation(new Vec(x, y, z))
                                            * Transformation.Scaling(new Vec(0.1f, 0.1f, 0.1f))));
                    } // z
                } // y
            }// x

            world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0f, -0.5f))
                                     * Transformation.Scaling(new Vec(0.1f, 0.1f, 0.1f))));
            world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0.5f, 0f))
                                     * Transformation.Scaling(new Vec(0.1f, 0.1f, 0.1f))));


            // Camera initialization
            Console.WriteLine("Creating the camera...");
            var cameraTransf = Transformation.RotationZ(Utility.ToRadians(angle)) * Transformation.Translation(new Vec(-1.0f, 0.0f, 0.0f));
            Camera camera;
            if (orthogonal) { camera = new OrthogonalCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }
            else { camera = new PerspectiveCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }

            // Ray tracing
            Console.WriteLine("Rendering the scene...");
            var rayTracer = new ImageTracer(image, camera);
            rayTracer.fireAllRays((Ray r) => world.rayIntersection(r) != null ? Constant.White : Constant.Black);

            // Write PFM image
            Console.WriteLine("Saving in pfm format...");
            using (FileStream outpfmstream = File.OpenWrite(pfmFile))
            {
                image.savePfm(outpfmstream);
                Console.WriteLine($"Image saved in {pfmFile}");
            }

            //Convert(args);

        }//Demo

        // ############################################# //
        public static void Convert(params string[] args)
        {

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

    public class DefaultDemo
    {
        public static int width = 640;
        public static int height = 480;

        public static int angledeg = 0;

        public static string pfmFile = "demoImage.pfm";

        public static string ldrFile = "demoImage.jpg";

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


