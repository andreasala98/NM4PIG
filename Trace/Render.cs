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
            Material mat = (hit.Value.shape).material;
            return mat.brdf.pigment.getColor(hit.Value.surfacePoint) + mat.emittedRadiance.getColor(hit.Value.surfacePoint);
        }

    }







} // trace