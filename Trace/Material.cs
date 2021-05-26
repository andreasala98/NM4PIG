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
using System.Collections.Generic;
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

        /// <summary>
        /// Returns color of the object
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
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

        public Color getColor(Vec2D uv)
        {

            int u = (int)Math.Floor(uv.u * this.nSteps);
            int v = (int)Math.Floor(uv.v * this.nSteps);
            return (u + v) % 2 == 0 ? this.color1 : this.color2;
        }
    }

    /// <summary>
    /// A pigment to project an Equirectangular projection (in form of a HdrImage) onto a shape. 
    /// </summary>
    public class ImagePigment : IPigment
    {
        public HdrImage image;

        public ImagePigment(HdrImage i)
        {
            this.image = i;
        }

        /// <summary>
        /// Bilinear interpolation to get the Color at coordinates specified by a Vec2D
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public Color getColor(Vec2D v)
        {
            // simple interpolation
            // int col = (int)(v.u * this.image.width);
            // int row = (int)(v.v * this.image.height);

            // if (col >= this.image.width) col = this.image.width - 1;
            // if (row >= this.image.height) row = this.image.height - 1;

            // return image.getPixel(col, row);

            // elegant bilinear interpolation, but not working

            float x = v.u * (this.image.width - 1);
            int x1 = (int)Math.Floor(v.u * (this.image.width - 1));
            if (x1 >= this.image.width - 1) x1 = (int)v.u * (this.image.width - 2);
            int x2 = x1 + 1;

            float y = v.v * (this.image.height - 1);
            int y1 = (int)Math.Floor(v.v * (this.image.height - 1));
            if (y1 >= this.image.height - 1) y1 = (int)v.v * (this.image.height - 2);
            int y2 = y1 + 1;

            // Console.WriteLine("(x1,y2) = "  + x1 + " " + y2 + "   (x2,y2) = " + x2 + " " + y2);
            // Console.WriteLine("(x1,y1) = "  + x1 + " " + y1 + "   (x2,y1) = " + x2 + " " + y1);

            Color c11 = this.image.getPixel(x1, y1);
            Color c12 = this.image.getPixel(x1, y2);
            Color c21 = this.image.getPixel(x2, y1);
            Color c22 = this.image.getPixel(x2, y2);

            float den = 1.0f / ((x2 - x1) * (y2 - y1));

            return (x2 - x) * (c11 * (y2 - y) + c12 * (y - y1)) + (x - x1) * (c21 * (y2 - y) + c22 * (y - y1)) * den;

        }
    }

    /// <summary>
    /// An abstract class representing a Bidirectional Reflectance Distribution Function.
    /// Use only derivate classes in the program.
    /// </summary>

    public abstract class BRDF
    {

        public IPigment pigment;

        public abstract Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv);

        public abstract Ray scatterRay(PCG r, Vec incomingDir, Point interactionPoint, Normal normal, int depth);

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

        public float reflectance;
        public DiffuseBRDF(IPigment? pig = null, float refl = 1.0f)
        {
            this.pigment = pig ?? new UniformPigment(Constant.White);
            this.reflectance = refl;
        }

        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            return this.pigment.getColor(uv) * (this.reflectance / Constant.PI);
        }

        public override Ray scatterRay(PCG r, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
        {
            List<Vec> a = normal.createONBfromZ();
            Vec e1 = a[0];
            Vec e2 = a[1];
            Vec e3 = a[2];
            float cosThetaSq = r.randomFloat();
            float cosTheta = MathF.Sqrt(cosThetaSq);
            float sinTheta = MathF.Sqrt(1.0f - cosThetaSq);
            float phi = 2 * MathF.PI * r.randomFloat();

            Vec dir = e1 * sinTheta * MathF.Cos(phi) + e2 * sinTheta * MathF.Sin(phi) + e3 * cosTheta;

            return new Ray(interactionPoint, dir, tm : 1e-3f, dep : depth);
                
        }
    }


    /// <summary>
    /// A class representing an ideal mirror BRDF
    /// </summary>
    public class SpecularBRDF : BRDF
    {
        public float thresholdAngleRad;
        public SpecularBRDF(IPigment? pigment = null, float? thresholdAngleRad = null) : base(pigment)
        {
            this.thresholdAngleRad = thresholdAngleRad ?? Constant.PI / 1800.0f;
        }

        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            float thetaIn = MathF.Acos(Utility.NormalizedDot(normal.ToVec(), inDir));
            float thetaOut = MathF.Acos(Utility.NormalizedDot(normal.ToVec(), outDir));

            if (MathF.Abs(thetaIn - thetaOut) < this.thresholdAngleRad)
                return this.pigment.getColor(uv);
            else
                return new Color(0f, 0f, 0f);
        }

        public override Ray scatterRay(PCG r, Vec incomingDir, Point interactionPoint, Normal normal, int depth)
        {
            Vec rayDir = new Vec(incomingDir.x, incomingDir.y, incomingDir.z).Normalize();
            Vec normalVec = normal.ToVec().Normalize();
            float dotProd = normalVec * rayDir;

            return new Ray(
                            origin: interactionPoint,
                            dir: rayDir - normalVec * 2f * dotProd,
                            tm: 1e-5f,
                            tM: Single.PositiveInfinity,
                            dep: depth
                        );
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