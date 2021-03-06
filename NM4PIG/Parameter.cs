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

#nullable enable
using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using Trace;

namespace NM4PIG
{

    /// <summary>
    /// This class contains the default parameters fro the command line
    /// interface for both the 'demo' and the 'convert' commands.
    /// </summary>
    public class Default
    {
        // Demo
        public static int width = 640;
        public static int height = 480;
        public static int angledeg = 0;
        public static bool orthogonal = false;
        public static float? luminosity = null;
        public static int scene = 1;
        public static int spp = 4;
        public static char render = 'r';

        //Convert
        public static float factor = 0.6f; //0.18f;
        public static float gamma = 1.7f; //1.0f;

        //Render
        public static string file = "";
        public static int maxDepth = 3;
        public static int rrLimit = 2;
        public static int nRays = 10;

        //General
        public static string pfmFile = "demoImage.pfm";
        public static string ldrFile = "demoImage.jpg";

    }

    /// <summary>
    /// This class contains the settable parameters for the aforementioned command line interface.
    /// </summary>
    class Parameters
    {
        public string pfmFile;
        public float factor;
        public float gamma;
        public string ldrFile;
        public int width;
        public int height;
        public int angledeg;
        public bool orthogonal;
        public float? luminosity;
        public int scene;
        public int spp;
        public char render;
        public string file;
        public int maxDepth;
        public int rrLimit;
        public int nRays;

        public Dictionary<string, float> variables;

        public Parameters()
        {
            this.pfmFile = Default.pfmFile;
            this.factor = Default.factor;
            this.gamma = Default.gamma;
            this.ldrFile = Default.ldrFile;
            this.width = Default.width;
            this.height = Default.height;
            this.angledeg = Default.angledeg;
            this.orthogonal = Default.orthogonal;
            this.luminosity = Default.luminosity;
            this.scene = Default.scene;
            this.spp = Default.spp;
            this.render = Default.render;
            this.file = Default.file;
            this.variables = new Dictionary<string, float>();
            this.maxDepth = Default.maxDepth;
            this.rrLimit = Default.rrLimit;
            this.nRays = Default.nRays;

        }

        /// <summary>
        ///  Parse parameters from the command line in 'convert' mode
        /// </summary>
        /// <param name="pfmfile"> name of the .pfm file you want to convert</param>
        /// <param name="ldrfile"> name of the .png/.jpg file you want to save</param>
        /// <param name="factor"> scaling factor for every pixel in the image. Default is 0.18</param>
        /// <param name="gamma"> encoding/decoding factor due to monitor differences </param>
        /// <param name="luminosity"> automatic or manual average luminosity. Default is false (automatic) </param>
        /// <param name="spp"> number of Monte Carlo exctracted samples per pixel. Default is 9</param>
        /// <param name="render"> Tyoe of rendering: on-off, flat, pathtracing or pointlight tracing</param>
        public void parseCommandLineConvert(string? pfmfile, string? ldrfile, string? factor, string? gamma, string? luminosity)
        {

            if (pfmfile != null) this.pfmFile = pfmfile;
            if (ldrfile != null) this.ldrFile = ldrfile;

            _readFactor(factor);
            _readGamma(gamma);
            _readLuminosity(luminosity);

        } //parseCommandLineConvert

        /// <summary>
        ///  Parse parameters from the command line in 'demo' mode
        /// </summary>
        /// <param name="width"> Width of the image</param>
        /// <param name="height"> Height of the image</param>
        /// <param name="angledeg"> Field of view angle in degrees</param>
        /// <param name="orthogonal"> Boolean to switch between orthogonal and perspectivecamera types.</param>
        /// <param name="pfmfile"> Name of the fm output file</param>
        /// <param name="ldrfile"> Name of the .png/.jpg output file</param>
        public void parseCommandLineDemo(string? width, string? height, string? angledeg, string? orthogonal,
                                         string? pfmfile, string? ldrfile, string? luminosity, string? scene,
                                         string? SPP, string? rend)
        {
            if (pfmfile != null) this.pfmFile = pfmfile;
            if (ldrfile != null) this.ldrFile = ldrfile;
            if (orthogonal != null) this.orthogonal = true;

            _readWidth(width);
            _readHeight(height);
            _readScene(scene);
            _readAngleDeg(angledeg);
            _readLuminosity(luminosity);
            _readSPP(SPP);
            _readRend(rend);
        }

        /// <summary>
        ///  Parse parameters from the command line in 'render' mode
        /// </summary>
        /// <param name="file"> File that describes the scene </param>
        /// <param name="width"> Width of the image</param>
        /// <param name="height"> Height of the image</param>
        /// <param name="angledeg"> Field of view angle in degrees</param>
        /// <param name="orthogonal"> Boolean to switch between orthogonal and perspectivecamera types.</param>
        /// <param name="pfmfile"> Name of the fm output file</param>
        /// <param name="ldrfile"> Name of the .png/.jpg output file</param>
        public void parseCommandLineRender(string? file, string? width, string? height, string? pfmfile, string? ldrfile,
                                             string? SPP,  string? rend, List<string> declareFloat, string? factor,
                                              string? gamma, string? maxDep, string? nRay, string? rrL)
        {
            if (pfmfile != null) this.pfmFile = pfmfile;
            if (ldrfile != null) this.ldrFile = ldrfile;
            _readFile(file);
            _readWidth(width);
            _readHeight(height);
            _readSPP(SPP);
            _readRend(rend);
            _readFloat(declareFloat);
            _readFactor(factor);
            _readGamma(gamma);
            _readMaxDepth(maxDep);
            _readNRays(nRay);
            _readRussian(rrL);
        }

        private void _readWidth(string? width)
        {
            if (width != null)
            {
                try
                {
                    this.width = Int32.Parse(width);
                }
                catch
                {
                    throw new CommandLineException("Width argument is not an int. Please enter an integer");
                }
            }
        }

        private void _readFactor(string? factor)
        {
            if (factor != null)
            {
                try
                {
                    this.factor = float.Parse(factor, CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException("Factor argument is not a float. Please enter floating-point numbers");
                }
            }
        }

        private void _readGamma(string? gamma)
        {
            if (gamma != null)
            {
                try
                {
                    this.gamma = float.Parse(gamma, CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException("Gamma argument is not a float. Please enter floating-point numbers");
                }
            }
        }

        private void _readLuminosity(string? luminosity)
        {
            if (luminosity != null)
            {
                try
                {
                    this.luminosity = float.Parse(luminosity, CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException("Luminosity argument is not a float. Please enter a float");
                }
            }
        }

        private void _readScene(string? scene)
        {
            if (scene != null)
            {
                try
                {
                    this.scene = Int32.Parse(scene);
                }
                catch
                {
                    throw new CommandLineException("Scene argument is not an int. Please enter an integer");
                }
            }
        }

        private void _readHeight(string? height)
        {
            if (height != null)
            {
                try
                {
                    this.height = Int32.Parse(height);
                }
                catch
                {
                    throw new CommandLineException("Height argument is not an int. Please enter an integer");
                }
            }
        }

        private void _readAngleDeg(string? angledeg)
        {
            if (angledeg != null)
            {
                try
                {
                    this.angledeg = Int32.Parse(angledeg);
                }
                catch
                {
                    throw new CommandLineException("Angle argument is not an int. Please enter an integer");
                }
            }
        }

        private void _readSPP(string? SPP)
        {
            if (SPP != null)
            {
                try
                {
                    int TEMP_SPP = Int32.Parse(SPP);
                    this.spp = (int)Math.Pow((int)Math.Sqrt(TEMP_SPP), 2);
                    if (TEMP_SPP != this.spp) Console.WriteLine($"The number of samples per pixel ({TEMP_SPP}) must be a perfect square. It has been corrected to {this.spp}");
                }
                catch
                {
                    throw new CommandLineException("Samples per pixel: argument passed is not an integer");
                }
            }
        }

        private void _readRend(string? rend)
        {
            if (rend != null)
            {
                if (rend == "o" | rend == "p" | rend == "f" | rend == "r")
                {
                    try
                    {
                        this.render = Char.Parse(rend);
                    }
                    catch
                    {
                        throw new CommandLineException("Render type: argument passed is not a valid char");
                    }
                }
                else throw new CommandLineException("Render type: argument passed is not a valid char");
            }
        }

        private void _readFile(string? file)
        {
            if (file == null) throw new CommandLineException("Insert a scene to render!");
            try
            {
                using (FileStream inputStream = File.OpenRead(file)) { }
            }
            catch
            {
                throw new CommandLineException($"File {file} not found!");
            }
            this.file = file;
        }

        private void _readMaxDepth(string? mD) 
        {
            if (mD != null) 
            {
                try 
                {
                    this.maxDepth = Int32.Parse(mD);
                }
                catch
                {
                    throw new CommandLineException("maxDepth is not an int. Please enter an integer");
                }
            }
        }

        private void _readNRays(string? nR) 
        {
            if (nR != null) 
            {
                try 
                {
                    this.nRays = Int32.Parse(nR);
                }
                catch
                {
                    throw new CommandLineException("nRays is not an int. Please enter an integer");
                }
            }
        }


        private void _readRussian(string? RR) 
        {
            if (RR != null) 
            {
                try 
                {
                    this.rrLimit = Int32.Parse(RR);
                }
                catch
                {
                    throw new CommandLineException("RussianRouletteLowerLimit is not an int. Please enter an integer");
                }
            }
        }
        
        private void _readFloat(List<string> declareFloat)
        {
            Dictionary<string, float> variables = new Dictionary<string, float>();
            foreach (string declaration in declareFloat)
            {
                string[] tmp = declaration.Split(':', 2);
                if (tmp.Length != 2) throw new CommandLineException($"error, the definition «{ declaration }» does not follow the pattern NAME:VALUE");
                string name = tmp[0];
                float value;
                try
                {
                    value = float.Parse(tmp[1], CultureInfo.InvariantCulture);
                }
                catch
                {
                    throw new CommandLineException($"Value for variable {name} is not a float ({tmp[0]}). Please enter a float");
                }
                variables.Add(name, value);
            }
            this.variables = variables;
        }
    } //Parameters class


} //namespace