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
using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace NM4PIG
{

    class Program
    {

        public static void Main(params string[] args)
        {

            CommandLineApplication CLI = new CommandLineApplication(throwOnUnexpectedArg: false)
            {
                FullName = "\n***********************************************************\n" +
                           "*  Numerical Methods For Photorealistic Image Generation  *\n" +
                           "*                 ( shortly NM4PIG 🐷 )                   *\n" +
                           "*                                                         *\n" +
                           "*      for more info visit the GitHub repository at       *\n" +
                           "*         https://github.com/andreasala98/NM4PIG          *\n" +
                           "***********************************************************\n",
                Name = "dotnet run"
            };

            CLI.Command("demo",
            command =>
            {
                command.FullName = "\nThis is demo mode. Use this mode to generate some test images and\n" +
                                    "see that everything works as expected";
                command.Description = "Enter demo mode and generate a simple image";
                var width = command.Option("--width|-W <WIDTH>", "width of the generated image, default is 640", CommandOptionType.SingleValue);
                var height = command.Option("--height|-H <HEIGHT>", "height of the generated image, default is 480", CommandOptionType.SingleValue);
                var angledeg = command.Option("--angle|-a <ANGLE>", "field-of-view angle, default is 0", CommandOptionType.SingleValue);
                var orthogonal = command.Option("--orthogonal|-o", "Use an orthogonal camera instead of perspective", CommandOptionType.NoValue);
                var pfmfile = command.Option("--pfmfile|-pfm <FILENAME>", "name of .pfm output file", CommandOptionType.SingleValue);
                var luminosity = command.Option("--luminosity|-lum", "Force average luminosity to 0.5 instead of calculating it", CommandOptionType.NoValue);
                var ldrfile = command.Option("--ldrfile|-ldr <FILENAME>", "name of .png/.jpg output file", CommandOptionType.SingleValue);
                var scene = command.Option("--scene|-s <scene>", "number of the scene", CommandOptionType.SingleValue);

                command.HelpOption("-?|-h|--help");
                command.OnExecute(() =>
                {

                    Console.WriteLine(CLI.FullName);
                    Parameters readParam = new Parameters();
                    try
                    {
                        readParam.parseCommandLineDemo(
                                                        width.Value(),
                                                        height.Value(),
                                                        angledeg.Value(),
                                                        orthogonal.Value(),
                                                        pfmfile.Value(),
                                                        luminosity.Value(),
                                                        ldrfile.Value(),
                                                        scene.Value()
                                                        );
                    }
                    catch (CommandLineException e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }

                    Demo(
                        readParam.width,
                        readParam.height,
                        readParam.angledeg,
                        readParam.orthogonal,
                        readParam.pfmFile,
                        readParam.luminosity,
                        readParam.ldrFile,
                        readParam.scene
                    );
                    return 0;
                });
            });

            CLI.Command("convert",
            command =>
            {
                command.FullName = "\nThis is convert mode. Use this mode to convert a pfm file generated\n" +
                                    "by other modes to a jpg/png file. The purpuse of this command is to let\n" +
                                    "the user to perform again the tone mapping step without re-rendering the scene.";
                command.Description = "Enter convert mode and convert an input pfm file into a jpg/ png file";

                var pfmfile = command.Option("--pfmfile|-pfm <FILENAME>", "name of .pfm output file", CommandOptionType.SingleValue);
                var ldrfile = command.Option("--ldrfile|-ldr <FILENAME>", "name of .png/.jpg output file", CommandOptionType.SingleValue);
                var luminosity = command.Option("--luminosity|-lum", "Force average luminosity to 0.5 instead of calculating it", CommandOptionType.NoValue);
                var factor = command.Option("--factor|-f <FACTOR>", "scaling factor", CommandOptionType.SingleValue);
                var gamma = command.Option("--gamma|-g <GAMMA>", "gamma correction", CommandOptionType.SingleValue);

                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {

                    Console.WriteLine(CLI.FullName);
                    Parameters readParam = new Parameters();
                    try
                    {
                        readParam.parseCommandLineConvert(pfmfile.Value(), ldrfile.Value(), factor.Value(), gamma.Value(), luminosity.Value());
                    }
                    catch (CommandLineException e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }
                    Convert(
                            readParam.pfmFile,
                            readParam.ldrFile,
                            readParam.factor,
                            readParam.gamma,
                            readParam.luminosity
                            );
                    return 0;
                });
            });

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

        public static void Demo(int width, int height, int angle, bool orthogonal, string pfmFile, string ldrFile, int scene)
        {

            Console.WriteLine("Starting Demo with these parameters:\n");

            Console.WriteLine("Width: " + width);
            Console.WriteLine("Height: " + height);
            Console.WriteLine("Angle: " + angle);
            Console.WriteLine(orthogonal ? "Orthogonal Camera" : "Perspective Camera");
            Console.WriteLine("pfmFile: " + pfmFile);
            Console.WriteLine("ldrFile: " + ldrFile);

            Console.WriteLine("\n");

            HdrImage image = new HdrImage(width, height);

            Console.WriteLine("Creating the scene...");
            World world = new World();

            switch (scene)
            {
                case 1:
                    List<float> Vertices = new List<float>() { -0.5f, 0.5f };

                    //One sphere for each vertex of the cube
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

                    //Adding two more spheres to break simmetry
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0f, -0.5f))
                                             * Transformation.Scaling(0.1f)));
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0.5f, 0f))
                                             * Transformation.Scaling(0.1f)));
                    break;

                case 2:
                    world.addShape(new CSGDifference(new Sphere(Transformation.RotationX(Constant.PI) * Transformation.Translation(0f, 0f, -0.4f)),
                                                     new Sphere(Transformation.Scaling(0.9f) * Transformation.RotationX(Constant.PI)
                                                                * Transformation.Translation(0f, 0f, 0.1f))));
                    break;
                case 4:
                    world.addShape(new CSGUnion(new Sphere(Transformation.Translation(0f, 0.3f, 0f)),
                                                 new Sphere(Transformation.Translation(0f, -0.3f, 0.0f))));
                    break;
                case 5:
                    world.addShape(new Box(transformation: Transformation.Scaling(0.3f)*Transformation.RotationZ(Utility.DegToRad(30)), material: new Material(new DiffuseBRDF(new CheckeredPigment(new Color(0f,1f,0f), new Color(0f,0f,1f),40),0.7f))));
                    world.addShape(new Plane(transformation: Transformation.RotationY(-0.1f), material: new Material(new DiffuseBRDF(new UniformPigment(new Color(0f,0.5f,0.5f))))));
                    break;
                case 6:
                    world.addShape(new Box(transformation: Transformation.Scaling(0.5f)));
                    break;
                case 7:
                    world.addShape(new CSGUnion(
                                            new Sphere(
                                                    Transformation.Translation(new Vec(0f, 0f, 1.2f))
                                                    * Transformation.Scaling(0.635f)),
                                            new Box(transformation: Transformation.Scaling(0.5f,0.5f,1f))
                                            )
                                    );
                    break;
                default:
                    break;
            }


            // Camera initialization
            Console.WriteLine("Creating the camera...");
            var cameraTransf = Transformation.RotationZ(Utility.DegToRad(angle)) * Transformation.Translation(-1.0f, 0.0f, 0.0f);
            Camera camera;
            if (orthogonal) { camera = new OrthogonalCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }
            else { camera = new PerspectiveCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }

            // Ray tracing
            Console.WriteLine("Rendering the scene...");
            var rayTracer = new ImageTracer(image, camera);
            rayTracer.fireAllRays(new FlatRender(world));
            //rayTracer.fireAllRays(OnOff());

            // Write PFM image
            Console.WriteLine("Saving in pfm format...");
            using (FileStream outpfmstream = File.OpenWrite(pfmFile))
            {
                image.savePfm(outpfmstream);
                Console.WriteLine($"Image saved in {pfmFile}");
            }

            Convert(pfmFile, ldrFile, Default.factor, Default.gamma, Default.luminosity);

        }//Demo

        // ############################################# //
        public static void Convert(string inputpfm, string outputldr, float factor, float gamma, bool luminosity)
        {
            string fmt = outputldr.Substring(outputldr.Length - 3, 3);

            Console.WriteLine("\n\nStarting file conversion using these parameters:\n");

            Console.WriteLine("pfmFile: " + inputpfm);
            Console.WriteLine("ldrFile: " + outputldr);
            Console.WriteLine("Format: " + fmt);
            Console.WriteLine("Factor: " + factor);
            Console.WriteLine("Gamma: " + gamma);
            Console.WriteLine(luminosity ? "Manual luminosity" : "Average luminosity");

            Console.WriteLine("\n");

            HdrImage myImg = new HdrImage();

            try
            {
                using (FileStream inputStream = File.OpenRead(inputpfm))
                {
                    myImg.readPfm(inputStream);
                    Console.WriteLine($"File {inputpfm} has been correctly read from disk.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine("Starting Tone Mapping...");
            try
            {
                Console.WriteLine("Normalizing image...");

                if (luminosity) myImg.normalizeImage(factor, 0.5f);
                else myImg.normalizeImage(factor);

                Console.WriteLine("Clamping image...");
                myImg.clampImage();

                Console.WriteLine("Saving LDR image...");
                myImg.writeLdrImage(outputldr, fmt, gamma);

                Console.WriteLine($"File {outputldr} has been correctly written to disk.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

        }


    } //Program class

} //NM4PIG namespace


