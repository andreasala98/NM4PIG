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
        /// <see cref="Color"> of the background. Default is black.
        /// </summary>
        public Color backgroundColor;

        public Render(World world, Color? background = null)
        {
            this.world = world;
            this.backgroundColor = background ?? Constant.Black;
        }

        /// <summary>
        /// Estimate the radiance along a ray
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
        public FlatRender(World world, Color? background = null) : base(world, background) { }

        public override Color computeRadiance(Ray r)
        {
            HitRecord? hit = this.world.rayIntersection(r);
            if (hit == null) return backgroundColor;
            Material mat = hit.Value.shape?.material!;
            return mat.brdf.pigment.getColor(hit.Value.surfacePoint) + mat.emittedRadiance.getColor(hit.Value.surfacePoint);
        }

    }

    /// <summary>
    /// A simple path-tracing renderer
    /// The algorithm implemented here allows the caller to tune number of rays thrown at each iteration, as well as the
    /// maximum depth.It implements Russian roulette, so in principle it will take a finite time to complete the
    /// calculation even if you set max_depth to `math.inf`.
    /// </summary>
    public class PathTracer : Render
    {

        public PCG pcg;
        public int numOfRays;
        public int maxDepth;
        public int russianRouletteLimit;

        public PathTracer(World world, Color? background = null, PCG? pcg = null, int numOfRays = 10, int maxDepth = 2, int russianRouletteLimit = 3) : base(world, background)
        {
            this.pcg = pcg ?? new PCG();
            this.numOfRays = numOfRays;
            this.maxDepth = maxDepth;
            this.russianRouletteLimit = russianRouletteLimit;
        }

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
                if (this.pcg.randomFloat() > hitColorLum)
                    // Keep the recursion going, but compensate for other potentially discarded rays
                    hitColor *= 1.0f / (1.0f - hitColorLum);
                else
                    // Terminate prematurely
                    return emittedRadiance;

            Color cumRadiance = new Color(0f, 0f, 0f);
            if (hitColorLum > 0f)
            { // Only do costly recursions if it's worth it
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
    /// Class that implements a Point Light renderer.
    /// </summary>
    public class PointLightRender : Render
    {
        public Color ambientColor;

        public PointLightRender(World world, Color? background = null, Color? ambient = null) : base(world, background)
        {
            this.ambientColor = ambient ?? new Color(0.1f, 0.1f, 0.1f);
        }

        public override Color computeRadiance(Ray ray)
        {
            HitRecord? hitRecord = this.world.rayIntersection(ray);
            if (hitRecord == null) return this.backgroundColor;

            Material hitMaterial = hitRecord?.shape?.material!;

            Color resultColor = this.ambientColor;

            foreach (var curLight in world.lightSources)
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