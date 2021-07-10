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
    /// Class that represents a world. It is the variable to which we add the shapes.
    /// </summary>
    public class World
    {

        /// <summary>
        ///  Main field of the World class. It is a list of <see cref="Shape"/>
        ///  objects present in the environment.
        /// </summary>
        public List<Trace.Shape> shapes;

        /// <summary>
        /// List of PointLight, needed for the point-light tracer
        /// </summary>
        public List<Trace.PointLight> lightSources;

        /// <summary>
        /// Basic constructor for World. It does not add any shapes nor light sources.
        /// </summary>
        public World()
        {
            this.shapes = new List<Shape>();
            this.lightSources = new List<PointLight>();
        }

        public World(List<Shape> ShapeList, List<PointLight> PointLightList)
        {
            this.shapes = ShapeList;
            this.lightSources = PointLightList;
        }

        public World(List<Shape> ShapeList)
        {
            this.shapes = ShapeList;
        }

        public World(List<PointLight> PointLightList)
        {
            this.lightSources = PointLightList;
        }

        public World(Shape sh)
        {
            this.shapes = new List<Shape>();
            this.addShape(sh);
        }

        public World(PointLight pl)
        {
            this.lightSources = new List<PointLight>();
            this.addPointLight(pl);
        }

        /// <summary>
        /// Add a <see cref="Shape"/> to the list of shapes present in the world.
        /// </summary>
        /// <param name="sh"></param>
        public void addShape(Shape sh)
          => shapes.Add(sh);

        /// <summary>
        /// Add a <see cref="PointLight"/> to the list of light sources present in the world.
        /// </summary>
        /// <param name="pl"></param>
        public void addPointLight(PointLight pl) 
          => lightSources.Add(pl); 


        /// <summary>
        /// It calculates all intersections between all the shapes and a ray,
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

        /// <summary>
        /// Method that check is a light ray originated in Point point reaches the Point observerPoint.
        /// Needed for the point light renderer.
        /// </summary>
        /// <param name="point">point where the ray originates</param>
        /// <param name="observerPoint">the point the ray should reach</param>
        /// <returns></returns>
        public bool isPointVisible(Point point, Point observerPoint)
        {
            Vec direction = point - observerPoint;
            float directionNorm = direction.getNorm();

            Ray ray = new Ray(observerPoint, direction, tm: ((float) 1e-2 / directionNorm), tM: 1.0f);
            foreach (var shape in shapes)
            {
                if (shape.quickRayIntersection(ray)) return false;
            }
            return true;
        }

    } // end of World

} // end of Trace