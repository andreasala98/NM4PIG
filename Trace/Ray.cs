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
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Trace
{

     /// <summary>
    /// An efficient value type representing a ray to be fired.
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// The origin <see cref="Point"/>, where the observer lies.
        /// </summary>
        public Point origin;

        /// <summary>
        /// The <see cref="Vec"/> orthogonally connecting the observer to the center of the screen 
        /// </summary>
        public Vec dir;

        /// <summary>
        /// Minimum travelling distance of the ray. <br/>
        /// Default setting: 1e-5f
        /// </summary>
        public float tmin;

        /// <summary>
        /// Minimum travelling distance of the ray. <br/>
        /// Default setting: Infinity
        /// 
        /// </summary>
        public float tmax;

        /// <summary>
        /// Number of reflections allowed <br/>
        /// Default setting: 0
        /// </summary>
        public int depth;

        /// <summary>
        ///  Default constructor for Ray.
        /// </summary>
        /// <param name="or"> Origin <see cref="Point"/> (observer) </param>
        /// <param name="d"> <see cref="Vec"/> direction </param>
        /// <param name="tm"> Minimum distance </param>
        /// <param name="tM"> Maximum distance </param>
        /// <param name="dep"> Number of reflections </param>
        public Ray(Point origin, Vec dir, float? tm = 1e-5f, float? tM = System.Single.PositiveInfinity, int? dep = 0)
        {
            this.origin = origin;
            this.dir = dir;
            this.tmin = (float)tm;
            this.tmax = (float)tM;
            this.depth = (int)dep;
        }

        /// <summary>
        /// Calculate ray position at (origin + dir*t).
        /// </summary>
        /// <param name="t"> Running paramter between tmin and tmax.</param>
        /// <returns> A <see cref="Point"/> object </returns>
        public Point at(float t)
            => this.origin + (this.dir * t);

        /// <summary>
        ///  Boolean to check if two <see cref="Ray"/>s are equal.
        /// </summary>
        /// <param name="r"> The other <see cref="Ray"/>.</param>
        /// <returns> True if <see cref="Ray"/>s are close enough.</returns>
        public bool isClose(Ray r)
            => this.origin.isClose(r.origin) && this.dir.isClose(r.dir);

        /// <summary>
        /// Apply <see cref="Transformation"/> to the <see cref="Ray"/>.
        /// </summary>
        /// <param name="T"> A <see cref="Transformation"/> object. </param>
        /// <returns> The transformed <see cref="Ray"/>.</returns>
        public Ray Transform(Transformation T)
            => new Ray(T * this.origin, T * this.dir, this.tmin, this.tmax, this.depth);

    } // end of Ray


} //namespace