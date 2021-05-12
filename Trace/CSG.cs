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

namespace Trace{ 
   
   
    /// <summary>
    /// A 3D Shape created by the union 
    /// of two Shapes (Constructive Solid Geometry).
    /// 
    /// Datamembers: Shape firstsShape, Shape secondShape.
    /// </summary>
    public class CSGUnion : Shape
    {
        public Shape firstShape;
        public Shape secondShape;

        public CSGUnion (Shape a, Shape b)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        public override HitRecord? rayIntersection (Ray ray) 
        {
            HitRecord? a = this.firstShape.rayIntersection(ray);
            HitRecord? b = this.secondShape.rayIntersection(ray);
            

            if (a?.t < b?.t) 
                return a;
            else
                return b;
            
        }

        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> a = this.firstShape.rayIntersectionList(ray);
            
            List<HitRecord?> b = this.secondShape.rayIntersectionList(ray);

            List<HitRecord?> hits = new List<HitRecord?>();

            for (int i = 0; i < a.Count; i++)
            {
                if (!(this.secondShape.isPointInside((Point) a[i]?.worldPoint)))
                {
                    hits.Add(a[i]);
                }
            }

            for (int i = 0; i < b.Count; i++)
            {
                if (!(this.firstShape.isPointInside((Point) b[i]?.worldPoint)))
                {
                    hits.Add(b[i]);
                }
            }

            return hits;
        }

        public override bool isPointInside(Point a)
        {
            return firstShape.isPointInside(a) || secondShape.isPointInside(a);
        }
    }

    /// <summary>
    /// A 3D Shape created by the difference 
    /// of two Shapes (Constructive Solid Geometry).
    /// 
    /// Datamembers: Shape firstsShape, Shape secondShape.
    /// </summary>
/*    public class CSGDifference : Shape
    {
        public Shape firstShape;
        public Shape secondShape;

        public CSGDifference (Shape a, Shape b)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        public override HitRecord? rayIntersection (Ray ray) 
        {
            HitRecord? a = this.firstShape.rayIntersection(ray);
            HitRecord? b = this.secondShape.rayIntersection(ray);

            return a;
        }

        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> a = this.firstShape.rayIntersectionList(ray);
            return a;
        }
        
    } //CSGDifference
*/
} // Trace