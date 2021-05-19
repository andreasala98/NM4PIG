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
#nullable enable

namespace Trace
{

    /// <summary>
    /// An interface to describe a pigment
    /// </summary>
    public interface IPigment
    {
        Color getColor(Vec2D uv);
    }

    /// <summary>
    /// A simple uniform pigment. It puts a uniform color over the whole object
    /// </summary>
    public class UniformPigment : IPigment
    {
        public Color c;
        public UniformPigment(Color color)
        {
            this.c = color;
        }

        public Color getColor(Vec2D vec)
        {
            return this.c;
        }
    }

    /// <summary>
    /// A checkered Pigment. This class is derived form the <see cref="IPigment"/> interface.
    /// The number of repetitions is tunable, but it must be the same for both directions
    /// </summary>
    public class CheckeredPigment : IPigment
    {
        public Color color1;
        public Color color2;
        public int nSteps;

        public CheckeredPigment(Color? col1 = null, Color? col2 = null, int nS = 10)
        {
            this.color1 = col1 ?? Constant.White;
            this.color2 = col2 ?? Constant.Black;
            this.nSteps = nS;
        }

        public Color getColor(Vec2D uv) {

            int u = (int)Math.Floor(uv.u * this.nSteps);
            int v = (int)Math.Floor(uv.v * this.nSteps);
            return (u + v) % 2 == 0 ? this.color1 : this.color2;
        }
    }

    public class ImagePigment : IPigment
    {
        public HdrImage image;

        public ImagePigment(HdrImage i)
        {
            this.image = i;
        }

        public Color getColor(Vec2D v)
        {
            // simple interpolation
            int col = (int)(v.u * this.image.width);
            int row = (int)(v.v * this.image.height);

            if (col >= this.image.width) col = this.image.width - 1;
            if (row >= this.image.height) row = this.image.height - 1;

            return image.getPixel(col, row);

            // elegant bilinear interpolation, but not working

            // float x = v.u * this.image.width;
            // int x1 = (int) Math.Floor(v.u * this.image.width);
            // int x2 = (int) Math.Ceiling(v.u * this.image.width);

            // float y = v.v * this.image.height;
            // int y1 = (int) Math.Floor(v.v * this.image.height);
            // int y2 = (int) Math.Ceiling(v.v * this.image.height);

            // float denominator = 1.0f / ((x2 - x1) * (y2 - y1));

            // float r = (x2 - x) * (this.image.getPixel(x1, y1).r * (y2 - y) + this.image.getPixel(x1, y2).r * (y - y1));
            // r += (x - x1) * (this.image.getPixel(x2, y1).r * (y2 - y) + this.image.getPixel(x2, y2).r * (y - y1));

            // float g = (x2 - x) * (this.image.getPixel(x1, y1).g * (y2 - y) + this.image.getPixel(x1, y2).g * (y - y1));
            // g += (x - x1) * (this.image.getPixel(x2, y1).g * (y2 - y) + this.image.getPixel(x2, y2).g * (y - y1));

            // float b = (x2 - x) * (this.image.getPixel(x1, y1).b * (y2 - y) + this.image.getPixel(x1, y2).b * (y - y1));
            // b += (x - x1) * (this.image.getPixel(x2, y1).b * (y2 - y) + this.image.getPixel(x2, y2).b * (y - y1));

            // return new Color(r, g, b);


        }
    }

    /// <summary>
    /// An abstract class representing a Bidirectional Reflectance Distribution Function.
    /// Use only derivate classes in the program.
    /// </summary>

    public abstract class BRDF
    {

        public IPigment pigment;
        public float reflectance;
        public abstract Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv);

        public BRDF(IPigment? pig = null) 
        {
            this.pigment = pig ?? new UniformPigment(Constant.White);
        }

    }


    /// <summary>
    ///   A derivate class of BRDF representing an ideal diffusive BRDF (also called «Lambertian»)
    /// </summary>
    public class DiffuseBRDF : BRDF
    {

        public DiffuseBRDF(IPigment? pig = null, float refl = 1.0f)
        {
            this.pigment = pig ?? new UniformPigment(Constant.White);
            this.reflectance = refl;
        }

        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            return this.pigment.getColor(uv) * (this.reflectance / Constant.PI);
        }

    }

    /// <summary>
    /// A class that implements a material
    /// </summary>

    public class Material
    {
        /// <summary>
        /// A <see cref="BRDF"/> object
        /// </summary>
        public BRDF brdf;

        /// <summary>
        /// A <see cref="Pigment"/> object
        /// </summary>
        public IPigment emittedRadiance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Brdf">A <see cref="BRDF"/> object, default is DiffuseBRDF()</param>
        /// <param name="EmittedRadiance">A <see cref="Pigment"/> object, default is UniformPigment(Constant.Black)</param>
        public Material(BRDF? Brdf = null, IPigment? EmittedRadiance = null)
        {
            this.brdf = Brdf ?? new DiffuseBRDF();
            this.emittedRadiance = EmittedRadiance ?? new UniformPigment(Constant.Black);
        }
    }



} // Trace