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
    /// A class implementing a solver of the rendering equation.
    /// This is an abstract class; you should use a derived concrete class.
    /// </summary>
    public abstract class Render
    {

        /// <summary>
        /// <see cref="World"> object to be rendered
        /// </summary>
        public World world;

        /// <summary>
        /// <see cref="Color"> of the background. Default is black (0,0,0).
        /// </summary>
        public Color backgroundColor;

        public Render(World world, Color? background = null)
        {
            this.world = world;
            this.backgroundColor = background ?? Constant.Black;
        }

        /// <summary>
        /// Estimate the radiance brought by a <see cref="Ray"/>.
        /// </summary>
        public abstract Color computeRadiance(Ray r);
    }


    /// <summary>
    /// A on/off renderer<br/>
    /// This renderer is mostly useful for debugging purposes, as it is really fast, but it produces boring images.
    /// </summary>
    public class OnOffRender : Render
    {
        /// <summary>
        /// <see cref="Color"> of the world. Default is white
        /// </summary>
        public Color color;

        public OnOffRender(World world, Color? background = null, Color? color = null) : base(world, background)
        {
            this.color = color ?? Constant.White;
        }

        /// <summary>
        /// Radiance computing algorithm. If there is any intersection, the output color is white,
        /// otherwise it is the background color.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public override Color computeRadiance(Ray r)
            => this.world.rayIntersection(r) != null ? this.color : this.backgroundColor;
    }


    /// <summary>
    /// A «flat» renderer.<br/>
    /// This renderer estimates the solution of the rendering equation by neglecting any contribution of the light.
    /// It just uses the pigment of each surface to determine how to compute the final radiance.
    /// </summary>
    public class FlatRender : Render
    {
        /// <summary>
        ///  Basic constructor for the class. It is fully inherited by the <see cref="Render"/> class.
        /// </summary>
        /// <param name="world">World to be rendered</param>
        /// <param name="background">Background color</param>
        public FlatRender(World world, Color? background = null) : base(world, background) { }


        /// <summary>
        ///  Radiance computing algorithm for the flat renderer.
        ///  It calculates the radiance brought by a <see cef="Ray"/>, but it actually
        ///  takes the color of the last shape the ray encountered.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public override Color computeRadiance(Ray r)
        {
            HitRecord? hit = this.world.rayIntersection(r);
            if (hit == null) return backgroundColor;

            Material mat = hit.Value.shape?.material!;
            return mat.brdf.pigment.getColor(hit.Value.surfacePoint) + mat.emittedRadiance.getColor(hit.Value.surfacePoint);
        }

    }

    /// <summary>
    /// A path-tracing renderer
    /// The algorithm implemented here allows the caller to tune number of rays thrown at each iteration, as well as the
    /// maximum depth. It implements Russian roulette, so in principle it will take a finite time to complete the
    /// calculation even if you set max_depth to infinity.
    /// </summary>
    public class PathTracer : Render
    {


        /// <summary>
        /// Random number generator
        /// </summary>
        public PCG pcg;

        /// <summary>
        /// Number of rays to be fired at each iteration
        /// </summary>
        public int numOfRays { get; set; }

        /// <summary>
        /// Maximum number of reflections for any <see cref="Ray"/>
        /// </summary>
        public int maxDepth { get; set; }

        /// <summary>
        /// Minimum number of reflections for the Russian Roulette algorith to start.
        /// </summary>
        public int russianRouletteLimit { get; set; }


        /// <summary>
        /// Basic contructor for the class. It inherits the constructor of the <see cref="Render"/> class.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="bkg"></param>
        /// <param name="pcg"></param>
        /// <param name="numOfRays"></param>
        /// <param name="maxDepth"></param>
        /// <param name="russianRouletteLimit"></param>
        public PathTracer(World world, Color? bkg = null, PCG? pcg = null, int numOfRays = 10, int maxDepth = 3, int russianRouletteLimit = 2) : base(world, bkg)
        {
            this.pcg = pcg ?? new PCG();
            this.numOfRays = numOfRays;
            this.maxDepth = maxDepth;
            this.russianRouletteLimit = russianRouletteLimit;
        }

        /// <summary>
        ///  Radiance computing algortihm. This is the fundamental algorithm for the whole library.
        ///  It calculates the radiance brought by a <see cref="Ray"/> object when it hits the screen.
        /// </summary>
        /// <param name="ray"> The travelling Ray.</param>
        /// <returns> The computed rafiance in form of a <see cref="Color"/> object.</returns>
        public override Color computeRadiance(Ray ray)
        {
            if (ray.depth > this.maxDepth)
                return new Color(0f, 0f, 0f);

            HitRecord? hitRecord = this.world.rayIntersection(ray);
            if (hitRecord == null)
                return this.backgroundColor;

            Material hitMaterial = hitRecord.Value.shape?.material!;
            Color hitColor = hitMaterial.brdf.pigment.getColor(hitRecord.Value.surfacePoint);
            Color emittedRadiance = hitMaterial.emittedRadiance.getColor(hitRecord.Value.surfacePoint);

            float hitColorLum = MathF.Max(hitColor.r, MathF.Max(hitColor.g, hitColor.b));

            // Russian roulette
            if (ray.depth >= this.russianRouletteLimit)
            {
                if (this.pcg.randomFloat() > hitColorLum)
                    // Keep the recursion going, but compensate for other potentially discarded rays
                    hitColor *= 1.0f / (1.0f - hitColorLum);
                else
                    // Terminate prematurely
                    return emittedRadiance;
            }

            Color cumRadiance = new Color(0f, 0f, 0f);
            if (hitColorLum > 0f)
            { // Only do costly recursions if it's worth it -- it's not (most of the times)
                for (int rayIndex = 0; rayIndex < this.numOfRays; rayIndex++)
                {
                    Ray newRay = hitMaterial.brdf.scatterRay(
                        pcg: this.pcg,
                        incomingDir: hitRecord.Value.ray.dir,
                        interactionPoint: hitRecord.Value.worldPoint,
                        normal: hitRecord.Value.normal,
                        depth: ray.depth + 1
                    );
                    // Recursive call
                    Color newRadiance = this.computeRadiance(newRay);
                    cumRadiance += hitColor * newRadiance;
                }
            }
            return emittedRadiance + cumRadiance * (1.0f / this.numOfRays);
        }
    }

    /// <summary>
    /// Class that implements a Point Light renderer. The solid angle integral of the rendering equations
    /// can be simplified, because the integrand contains some localized Dirac deltas
    /// </summary>
    public class PointLightRender : Render
    {
        public Color ambientColor;

        /// <summary>
        /// Basic constructor for the class.
        /// </summary>
        /// <param name="world"></param>
        /// <param name="background"></param>
        /// <param name="ambient"></param>
        public PointLightRender(World world, Color? background = null, Color? ambient = null) : base(world, background)
        {
            this.ambientColor = ambient ?? new Color(0.0f, 0.0f, 0.0f);
        }

        /// <summary>
        /// Radiance computing algorith for the Point Light renderer.
        /// </summary>
        /// <param name="ray"> Travelling <see cref="Ray"/></param>
        /// <returns> THe computed radiance, in form of a <see cref="Color"/> object</returns>
        public override Color computeRadiance(Ray ray)
        {
            HitRecord? hitRecord = this.world.rayIntersection(ray);
            if (hitRecord == null) return this.backgroundColor;

            Material hitMaterial = hitRecord?.shape?.material!;

            Color resultColor = this.ambientColor;

            foreach (PointLight curLight in world.lightSources)
            {
                if (this.world.isPointVisible(curLight.position, (Point)hitRecord?.worldPoint!))
                {
                    Vec distanceVec = (Point)hitRecord?.worldPoint! - curLight.position;
                    float distance = distanceVec.getNorm();
                    Vec inDir = distanceVec * (1f / distance);
                    float cosTheta = MathF.Max(0f, Utility.NormalizedDot(-ray.dir, (Vec)hitRecord?.normal.toVec()!));

                    float distanceFactor;
                    if (curLight.linearRadius > 0)
                        distanceFactor = (curLight.linearRadius / distance) * (curLight.linearRadius / distance);
                    else
                        distanceFactor = 1f;

                    Color emittedColor = (Color)hitMaterial?.emittedRadiance.getColor(hitRecord.Value.surfacePoint)!;
                    Color brdfColor = (Color)hitMaterial?.brdf.Eval(
                                                                    (Normal)hitRecord?.normal!,
                                                                    inDir,
                                                                    -ray.dir,
                                                                    (Vec2D)hitRecord?.surfacePoint!)!;
                    resultColor += (emittedColor + brdfColor) * curLight.color * cosTheta * distanceFactor;
                }
            }

            return resultColor;

        }
    } 

} // trace