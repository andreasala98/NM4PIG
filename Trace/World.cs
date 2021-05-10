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
    public class World
    {

        /// <summary>
        ///  Main field of the World class. It is a list of <see cref="Shape"/>
        ///  objects present in the environment.
        /// </summary>
        public List<Trace.Shape> shapes;

        public World()
        {
            this.shapes = new List<Shape>();
        }

        public World(List<Shape> ShapeList)
        {
            this.shapes = ShapeList;
        }

        public World(Shape sh)
        {
            this.shapes = new List<Shape>();
            this.addShape(sh);
        }

        public void addShape(Shape sh)
          => shapes.Add(sh);


        /// <summary>
        /// It calculates all intersections between shapes and a ray,
        /// and it outputs the hit record of the closest object.
        /// </summary>
        /// <param name="intRay"> <see cref="Ray"/> object potentially intersecating
        /// <see cref="Shape"/> objects in the world</param>.
        /// <returns> The Hit Record of the ray with the closest object </returns>
        public HitRecord? rayIntersection(Ray intRay) 
        {
            HitRecord? closest = null;
            HitRecord? lastIntersection;

            foreach (var shape in shapes){
                lastIntersection = shape.rayIntersection(intRay);
                if (lastIntersection == null) continue;
                if (closest == null || (float)closest?.t > (float)lastIntersection?.t)
                {
                    closest = lastIntersection;
                }
            }
            return closest;
        }

    } // end of World

} // end of Trace