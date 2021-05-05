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

namespace Trace
{


/// <summary>
///  A class holding information about a ray-shape intersection
/// </summary>
    public struct HitRecord{

        /// <summary>
        /// a <see cref="Point"/> object holding the world coordinates of the hit point
        /// </summary>
        Point worldPoint;
        /// <summary>
        /// a <see cref="Normal"/> object holding the orientation of the normal to the surface where the hit happened
         /// </summary>
        Normal normal;
        /// <summary>
        /// a <see cref="Vec2D"> object holding the position of the hit point on the surface of the object
        /// </summary>
        Vec2D surfacePoint;
        /// <summary>
        /// a floating-point value specifying the distance from the origin of the <see cref="Ray"> where the hit happened
        /// </summary>
        float t;
        /// <summary>
        /// The <see cref="Ray"/> that hit the surface
        /// </summary>
        Ray ray;


        public HitRecord(Point wp, Normal nm, Vec2D sp, float tt, Ray r) 
        {
            this.worldPoint = wp;
            this.normal = nm;
            this.surfacePoint = sp;
            this.t = tt;
            this.ray = r;
        }


        public bool isClose(HitRecord? other) 
        {

            if (other == null) return false;

            else return (   this.worldPoint.isClose((Point)other.worldPoint)
                         && this.normal.isClose((Normal)other.normal)
                         && this.surfacePoint.isClose((Vec2D)other.surfacePoint)
                         && this.ray.isClose((Ray)other.ray)
                        );



        }



    }



    /// <summary>
    /// Class to represent the rendering environment. It is a collection of all the present 3D shapes.
    /// This class is used to calculate ray intersections with objects and to determine the closest ones.
    /// </summary>
    public class World
    {

        /// <summary>
        ///  Main field of the World class. It is a list of <see cref="Shape"/>
        ///  objects present in the environment.
        /// </summary>
        public List<Trace.Shape> shapes;

        public void addShape(Shape sh)
          => shapes.Add(sh);

        public HitRecord? rayIntersection(Ray intRay) 
        {
            HitRecord? closest = null;
            HitRecord? lastIntersection;

            foreach (var shape in this.shapes){
                lastIntersection = shape.rayIntersection(intRay);
                if (lastIntersection==null) continue;
                if (closest==null || closest.t > lastIntersection.t){
                    closest = lastIntersection;
                }
             }

            return closest;
        }




    } // end of World

} // end of Trace