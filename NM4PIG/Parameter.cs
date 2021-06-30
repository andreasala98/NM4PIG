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
using Trace;
using System.Globalization;

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
        public static float factor = 0.9f; //0.18f;
        public static float gamma = 2.0f; //1.0f;

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

            if (SPP != null) 
            {
                try {
                    int TEMP_SPP = Int32.Parse(SPP);
                    this.spp = (int)Math.Pow((int)Math.Sqrt(TEMP_SPP), 2);
                }
                catch { 
                    throw new CommandLineException("Samples per pixel: argument passed is not an integer");
                }
            }
            if (rend!=null) 
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
    } //Parameters class


} //namespace