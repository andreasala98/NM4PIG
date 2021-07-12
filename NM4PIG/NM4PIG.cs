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
using Trace;
using Microsoft.Extensions.CommandLineUtils;

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

            CLI.Command("render",
            command =>
            {
                command.FullName = "\nThis is render mode and it is the core functionality of the program.";
                command.Description = "Enter render mode and generate a complex image";
                var file = command.Option("--scene|-s <FILENAME>", "File that contains the description of the scene. Default is Examples/Inputs/dummy.txt", CommandOptionType.SingleValue);
                var width = command.Option("--width|-W <WIDTH>", "width of the generated image, default is 640", CommandOptionType.SingleValue);
                var height = command.Option("--height|-H <HEIGHT>", "height of the generated image, default is 480", CommandOptionType.SingleValue);
                var pfmfile = command.Option("--pfmfile|-pfm <FILENAME>", "name of .pfm output file. Default is demoImage.pfm", CommandOptionType.SingleValue);
                var ldrfile = command.Option("--ldrfile|-ldr <FILENAME>", "name of .png/.jpg output file. Default is demoImage.jpg", CommandOptionType.SingleValue);
                var spp = command.Option("--samples-per-pixel|-spp <SAMPLES>", "number of extracted samples per pixel. Default is 4", CommandOptionType.SingleValue);
                var rendType = command.Option("--render-type|-rnd <CHAR>", "Type of rendering - choose among (o,f,p,r). Default is r (path tracer)", CommandOptionType.SingleValue);
                var declareFloat = command.Option("--declare-float|-d", "Declare a variable. The syntax is «--declare-float VAR:VALUE». Example: --declare-float clock:150 --declare-float dummy:5.6 ...", CommandOptionType.MultipleValue);
                var factor = command.Option("--factor|-f <FACTOR>", "scaling factor. Deafult is 0.6", CommandOptionType.SingleValue);
                var gamma = command.Option("--gamma|-g <GAMMA>", "gamma correction. Default is 1.7", CommandOptionType.SingleValue);

                command.HelpOption("-?|-h|--help");
                command.OnExecute(() =>
                {

                    Console.WriteLine(CLI.FullName);
                    Parameters readParam = new Parameters();
                    try
                    {
                        readParam.parseCommandLineRender(
                                                        file.Value(),
                                                        width.Value(),
                                                        height.Value(),
                                                        pfmfile.Value(),
                                                        ldrfile.Value(),
                                                        spp.Value(),
                                                        rendType.Value(),
                                                        declareFloat.Values,
                                                        factor.Value(),
                                                        gamma.Value()
                                                        );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }

                    RenderScene.ExecuteRender(
                        readParam.file,
                        readParam.width,
                        readParam.height,
                        readParam.pfmFile,
                        readParam.ldrFile,
                        readParam.spp,
                        readParam.render,
                        readParam.variables,
                        readParam.factor,
                        readParam.gamma
                    );
                    return 0;
                });
            });

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
                var luminosity = command.Option("--luminosity|-l <LUMINOSITY>", "Force average luminosity to some value instead of calculating it", CommandOptionType.SingleValue);
                var ldrfile = command.Option("--ldrfile|-ldr <FILENAME>", "name of .png/.jpg output file", CommandOptionType.SingleValue);
                var scene = command.Option("--scene|-s <scene>", "number of the scene", CommandOptionType.SingleValue);
                var spp = command.Option("--samples-per-pixel|-spp <SAMPLES>", "number of extracted samples per pixel", CommandOptionType.SingleValue);
                var rendType = command.Option("--render-type|-rnd <CHAR>", "Type of rendering - choose among (o,f,p,r)", CommandOptionType.SingleValue);
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
                                                        ldrfile.Value(),
                                                        luminosity.Value(),
                                                        scene.Value(),
                                                        spp.Value(),
                                                        rendType.Value()
                                                            );
                    }
                    catch (CommandLineException e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }

                    Demo.ExecuteDemo(
                        readParam.width,
                        readParam.height,
                        readParam.angledeg,
                        readParam.orthogonal,
                        readParam.pfmFile,
                        readParam.ldrFile,
                        readParam.scene,
                        readParam.luminosity,
                        readParam.spp,
                        readParam.render
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
                var luminosity = command.Option("--luminosity|-l <LUMINOSITY>", "Force average luminosity to some value instead of calculating it", CommandOptionType.SingleValue);
                var factor = command.Option("--factor|-f <FACTOR>", "scaling factor", CommandOptionType.SingleValue);
                var gamma = command.Option("--gamma|-g <GAMMA>", "gamma correction", CommandOptionType.SingleValue);

                command.HelpOption("-?|-h|--help");

                command.OnExecute(() =>
                {

                    Console.WriteLine(CLI.FullName);
                    Parameters readParam = new Parameters();
                    try
                    {
                        readParam.parseCommandLineConvert(
                                                        pfmfile.Value(),
                                                        ldrfile.Value(),
                                                        factor.Value(),
                                                        gamma.Value(),
                                                        luminosity.Value());
                    }
                    catch (CommandLineException e)
                    {
                        Console.WriteLine(e.Message);
                        return 0;
                    }
                    Convert.ExecuteConvert(
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

            try
            {
                CLI.Execute(args);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                Console.WriteLine(exc.GetType());
                Console.WriteLine(">>> Exiting from the program. ");
                return;
            }

            return;

        } //Main

    } //Program class



} //NM4PIG namespace


