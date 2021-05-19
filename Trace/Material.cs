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

namespace Trace
{
    public interface IPigment
    {
        Color getColor(Vec2d v);
    }

    public class UniformPigment : IPigment
    {
        public Color c;
        public UniformPigment(Color color)
        {
            this.c = color;
        }

        public Color getColor(Vec2d vec)
        {
            return this.c;
        }
    }

    public class ImagePigment : Pigment
    {

    }
}
#nullable enable
namespace Trace
{

    public abstract class Pigment
    {

        public Color color;
        public Color getColor(Vec2D uv)
        {
            return new Color(0f, 0f, 0f);
        }


    }


    /// <summary>
    /// An abstract class representing a Bidirectional Reflectance Distribution Function.
    /// Use only derivate classes in the program.
    /// </summary>

    public abstract class BRDF
    {

        public Pigment pigment;
        public float reflectance;
        public BRDF() { }
        public abstract Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv);

    }


    /// <summary>
    ///   A derivate class of BRDF representing an ideal diffusive BRDF (also called «Lambertian»)
    /// </summary>
    public class DiffuseBRDF : BRDF
    {

        public DiffuseBRDF(Pigment pig = new UniformPigment(Constant.White), float refl = 1.0f)
        {
            this.pigment = pig;
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
        public Pigment emittedRadiance;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Brdf">A <see cref="BRDF"/> object, default is DiffuseBRDF()</param>
        /// <param name="EmittedRadiance">A <see cref="Pigment"/> object, default is UniformPigment(Constant.Black)</param>
        public Material(BRDF? Brdf = null, Pigment? EmittedRadiance = null)
        {
            this.brdf = Brdf ?? new DiffuseBRDF();
            this.emittedRadiance = EmittedRadiance ?? new UniformPigment(Constant.Black);
        }
    }



} // Trace
