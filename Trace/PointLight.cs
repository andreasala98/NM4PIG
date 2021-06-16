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
    /// <summary>
    /// Class that holds information of a point light source, which in the rendering equation is 
    /// equivalent to a Dirac's delta.
    /// Datamembers: Point position, Color color, float linearRadius.
    /// </summary>
    public class PointLight
    {
        /// <summary>
        /// Position of the light source
        /// </summary>
        public Point position;

        /// <summary>
        /// Color of the light source
        /// </summary>
        public Color color;

        /// <summary>
        /// If non-zero, this linear radius r is used to compute the solid angle 
        /// subtended by the light at a given distance d through the formula (r / d)^2
        /// </summary>
        public float linearRadius;

        public PointLight(Point position, Color color, float? linearRadius = null)
        {
            this.position = position;
            this.color = color;
            this.linearRadius = linearRadius ?? 0f;
        }
    }
}