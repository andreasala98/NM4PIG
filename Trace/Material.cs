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

        public Color GetColor(Vec2D v)
        {
            return image.getColor(v.u, v.v);
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