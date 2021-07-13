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
    /// An interface to describe a pigment. Concrete subclasses are
    /// <see cref="UniformPigment"/>, <see cref="Checkered Pigment"/> and <see cref="ImagePigment">.
    /// </summary>
    public interface IPigment
    {  
        /// <summary>
        /// Retrieve the color at a certain point of a surface
        /// </summary>
        /// <param name="uv"> <see cref="Vec2D"/> object to identify a point on a surface</param>
        /// <returns> The parsed <see cref="Color"/>.</returns>
        Color getColor(Vec2D uv);
    }

    /// <summary>
    /// A simple uniform pigment. It puts a uniform color over the whole shape.
    /// </summary>
    public class UniformPigment : IPigment
    {
        /// <summary>
        /// The only color of the pigment
        /// </summary>
        public Color c;
        /// <summary>
        /// Basic contructor for Uniform Pigment
        /// </summary>
        /// <param name="color"> The only color</param>
        public UniformPigment(Color color)
        {
            this.c = color;
        }

        /// <summary>
        /// Returns color of the pigment in a certain point
        /// </summary>
        /// <param name="vec">Point on a surface</param>
        /// <returns>The parsed Color.</returns>
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
        /// <summary>
        /// The first color of the checkboard
        /// </summary>
        public Color color1;
        /// <summary>
        /// The second color of the checkboard
        /// </summary>
        public Color color2;
        /// <summary>
        /// Number of repetition along the sides
        /// </summary>
        public int nSteps;

        /// <summary>
        /// Basic constructor for the class
        /// </summary>
        /// <param name="col1">color 1</param>
        /// <param name="col2">color 2</param>
        /// <param name="nS">number of squares</param>
        public CheckeredPigment(Color? col1 = null, Color? col2 = null, int nS = 10)
        {
            this.color1 = col1 ?? Constant.White;
            this.color2 = col2 ?? Constant.Black;
            this.nSteps = nS;
        }
        /// <summary>
        /// Retrieve the Color at acertain point of the surface
        /// </summary>
        /// <param name="uv"> Point on the surface</param>
        /// <returns>A Color object</returns>
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

        /// <summary>
        /// Basic contructor for the class
        /// </summary>
        /// <param name="i"> An HDR image in pfm format</param>
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

            // elegant bilinear interpolation

            float x = v.u * (this.image.width - 1);
            int x1 = (int)Math.Floor(v.u * (this.image.width - 1));
            if (x1 >= this.image.width - 1) x1 = (int)v.u * (this.image.width - 2);
            int x2 = x1 + 1;

            float y = v.v * (this.image.height - 1);
            int y1 = (int)Math.Floor(v.v * (this.image.height - 1));
            if (y1 >= this.image.height - 1) y1 = (int)v.v * (this.image.height - 2);
            int y2 = y1 + 1;

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
    /// Use only derivate classes in the program (DiffuseBRDF, SPecularBRDF)
    /// </summary>

    public abstract class BRDF
    {
        /// <summary>
        /// The pigment present on the surface
        /// </summary>
        public IPigment pigment;

        /// <summary>
        ///  Evaluate the BRDF for a given <see cref="Ray"/> coming in and out at given directions,
        ///  given also the <see cref"Normal"/> to the surface point (given as well)
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="inDir"></param>
        /// <param name="outDir"></param>
        /// <param name="uv"></param>
        /// <returns></returns>
        public abstract Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv);

        /// <summary>
        /// Reflect (or diffuse) a ray coming into a surface. This class is reimplemented in concrete subclasses
        /// </summary>
        /// <param name="pcg"></param>
        /// <param name="incomingDir"></param>
        /// <param name="interactionPoint"></param>
        /// <param name="normal"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public abstract Ray scatterRay(PCG pcg, Vec incomingDir, Point interactionPoint, Normal normal, int depth);

        /// <summary>
        /// Trivial constructor for BRDF
        /// </summary>
        /// <param name="pig"></param>
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
        /// <summary>
        /// Reflective coefficient (must be 0< rho < 1)
        /// </summary>
        public float reflectance;
        /// <summary>
        ///  Basic contructor for DiffuseBRDF class.
        /// </summary>
        /// <param name="pig"> Pigment to cover the surface</param>
        /// <param name="refl"> Reflectance coefficient</param>
        public DiffuseBRDF(IPigment? pig = null, float refl = 1.0f)
        {
            this.pigment = pig ?? new UniformPigment(Constant.White);
            this.reflectance = refl;
        }

        /// <summary>
        /// Evaluate the DiffuseBRDF. It is used in Flat renderers
        /// </summary>
        /// <param name="normal"></param>
        /// <param name="inDir"></param>
        /// <param name="outDir"></param>
        /// <param name="uv"></param>
        /// <returns> The computed Color</returns>
        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            return this.pigment.getColor(uv) * (this.reflectance / Constant.PI);
        }

        /// <summary>
        /// Diffuse an incoming ray, i.e. generate a new output ray
        /// </summary>
        /// <param name="r"></param>
        /// <param name="incomingDir"></param>
        /// <param name="interactionPoint"></param>
        /// <param name="normal"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
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

            return new Ray(interactionPoint, dir, tm: 1e-5f, dep: depth);

        }
    }


    /// <summary>
    /// A class representing an ideal specular BRDF. This BRDF completely reflects every ray coming 
    /// along its direction 
    /// </summary>
    public class SpecularBRDF : BRDF
    {

        /// <summary>
        /// Angle (in radians) beyonds which rays are not reflected
        /// </summary>
        public float thresholdAngleRad;

        /// <summary>
        ///  Basic contrusctor for the class. 
        /// </summary>
        /// <param name="pigment"> Pigment to cover the surface with.</param>
        /// <param name="thresholdAngleRad"> Threshold angle. Default is 1/10 of a degree</param>
        public SpecularBRDF(IPigment? pigment = null, float? thresholdAngleRad = null) : base(pigment)
        {
            this.thresholdAngleRad = thresholdAngleRad ?? Constant.PI / 1800.0f;
        }

        /// <summary>
        /// Evaluate the BRDF as three floating point numbers, i.e. sa <see cref="Color">.
        /// </summary>
        /// <param name="normal"> <see cref="Normal"/> to the surface</param>
        /// <param name="inDir"> Incoming direction</param>
        /// <param name="outDir"> Outgoing direction</param>
        /// <param name="uv"> Point located on the surface</param>
        /// <returns> The computed Color</returns>
        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            float thetaIn = MathF.Acos(Utility.NormalizedDot(normal.ToVec(), inDir));
            float thetaOut = MathF.Acos(Utility.NormalizedDot(normal.ToVec(), outDir));

            if (MathF.Abs(thetaIn - thetaOut) < this.thresholdAngleRad)
                return this.pigment.getColor(uv);
            else
                return new Color(0f, 0f, 0f);
        }

        /// <summary>
        /// Reflect a <see cref="Ray"/> coming into a surface. This class is reimplemented in concrete subclasses
        /// </summary>
        /// <param name="pcg"></param>
        /// <param name="incomingDir"></param>
        /// <param name="interactionPoint"></param>
        /// <param name="normal"></param>
        /// <param name="depth"></param>
        /// <returns></returns>

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
    /// A class that implements a material and that contains all the information
    /// about how a shape interacts with a ray.
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
        /// Basic constructor
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