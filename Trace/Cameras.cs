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
        public Point origin;
        public Vec dir;
        public float tmin;
        public float tmax; 
        public int depth;

        public Ray (Point or, Vec d, float? tm = 1e-5f, float? tM = System.Single.PositiveInfinity, int? dep=0)
        {
            this.origin = or;
            this.dir = d;
            this.tmin = (float)tm;
            this.tmax = (float)tM;
            this.depth = (int)dep; 
        }
        
        public Point at (float t)
            => this.origin + (this.dir * t);

        
        public bool isClose(Ray r)
        {
            return this.origin.isClose(r.origin) && this.dir.isClose(r.dir);
        }

        public Ray Transform(Transformation T) 
            => new Ray(T * this.origin, T * this.dir, this.tmin, this.tmax, this.depth);
        

    }


    class Camera
    {

    }

}