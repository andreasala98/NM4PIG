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
    public class Default
    {
        // Demo
        public static int width = 640;
        public static int height = 480;
        public static int angledeg = 0;
        public static bool orthogonal = false;

        //Convert
        public static float factor = 0.18f;
        public static float gamma = 1.0f;

        //General
        public static string pfmFile = "demoImage.pfm";
        public static string ldrFile = "demoImage.jpg";
    }

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
        }

        public void parseCommandLineConvert(string? pfmfile, string? ldrfile, string? factor, string? gamma)
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

        } //parseCommandLineConvert

        public void parseCommandLineDemo(string? width, string? height, string? angledeg, string? orthogonal, string? pfmfile, string? ldrfile)
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
                    throw new CommandLineException("Width argument is not an int. Please enter integers numbers");
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
                    throw new CommandLineException("Height argument is not an int. Please enter integers numbers");
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
                    throw new CommandLineException("Angle argument is not an int. Please enter integers numbers");
                }
            }
        }
    } //Parameters class


}