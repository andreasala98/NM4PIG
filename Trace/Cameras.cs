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
    public struct Ray
    {

    }

    /// <summary>
    /// An abstract class representing an observer
    /// Concrete subclasses are `OrthogonalCamera` and `PerspectiveCamera`.
    /// </summary>
    public abstract class Camera
    {
        public float aspectRatio = 1.0f;
        public Transformation transformation = new Transformation(1);

        public Camera(float aspRat, Transformation transf)
        {
            this.aspectRatio = aspRat;
            this.transformation = transf;
        }
        public abstract Ray fireRay(float u, float v);
    }

    /// <summary>
    /// 
    /// </summary>
    public class OrthogonalCamera : Camera
    {
        public OrthogonalCamera(float aspRat, Transformation transf) : base(aspRat, transf) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-1.0f, (1.0f - 2f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            Vec direction = new Vec(1.0f, 0.0f, 0.0f);
            return new Ray(origin: origin, dir: direction, tmin: 1.0f).transform(this.transformation);
        }
    }

    public class PerspectiveCamera : Camera
    {
        public float screenDistance = 1.0f;
        public PerspectiveCamera(float screenDist, float aspRat, Transformation transf) : base(aspRat, transf)
        {
            this.screenDistance = screenDist;
        }

        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-this.screenDistance, 0.0f, 0.0f);
            Vec direction = new Vec(this.screenDistance, (1.0f - 2.0f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            return new Ray(origin: origin, dir: direction, tmin: 1.0f).transform(this.transformation);
        }
    }
}