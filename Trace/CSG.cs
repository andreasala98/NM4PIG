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
        /// <summary>
        /// First Shape to add
        /// </summary>
        public Shape firstShape;
        /// <summary>
        /// Second Shape to add
        /// </summary>
        public Shape secondShape;

        /// <summary>
        /// Basic consturctor of the class
        /// </summary>
        /// <param name="a">First Shape</param>
        /// <param name="b">Second Shape</param>
        public CSGUnion (Shape a, Shape b)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        /// <summary>
        /// Method that computes whether a Ray intersects the new union Shape.
        /// 
        /// It's overriden from the abstract class Shape
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override HitRecord? rayIntersection (Ray ray) 
        {
            HitRecord? a = this.firstShape.rayIntersection(ray);
            HitRecord? b = this.secondShape.rayIntersection(ray);
            
            if (a == null)
                return b;
            
            else if (b == null)
                return a;

            /// the second conditions are nececcary if the ray originates inside the Union Shape.
            else if (a?.t < b?.t && !(this.secondShape.isPointInside((Point) a?.worldPoint))) 
                return a;

            else if (a?.t < b?.t && (this.secondShape.isPointInside((Point) a?.worldPoint)))
                return b;

            else if (b?.t < a?.t && !(this.firstShape.isPointInside((Point) b?.worldPoint)))
                return b;
            else //equivalent to esleif(b?.t < a?.t && (this.firstShape.isPointInside((Point) b?.worldPoint)))
                return a;

        }

        /// <summary>
        /// Method that creates a list of all the legal intersections (in form of a HitRecord object) with a given Ray.
        ///
        ///It's overriden from the abstract Shape class. 
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Method that computes whether a Point is inside the Shape.
        /// 
        /// It's overriden from the abstract Shape class.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override bool isPointInside(Point a)
        {
            return firstShape.isPointInside(a) || secondShape.isPointInside(a);
        }
    }

    /// <summary>
    /// A 3D Shape created by the difference 
    /// of two Shapes (Constructive Solid Geometry).
    /// You subtract FORM the first Shape the second Shape.
    /// 
    /// Datamembers: Shape firstShape, Shape secondShape.
    /// </summary>
    public class CSGDifference : Shape
    {
        /// <summary>
        /// First Shape.
        /// </summary>
        public Shape firstShape;

        /// <summary>
        /// Second Shape.
        /// </summary>
        public Shape secondShape;

        /// <summary>
        /// Trivial constructor.
        /// </summary>
        /// <param name="a">first Shape (the one FROM which you subtract)</param>
        /// <param name="b">second Shape (the one you subtract)</param>
        public CSGDifference (Shape a, Shape b)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        /// <summary>
        /// Given a Ray, this method computes whether it intersects the new Difference Shape.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override HitRecord? rayIntersection (Ray ray) 
        {
            List<HitRecord?> a = this.firstShape.rayIntersectionList(ray);
            if (a[0] == null)
                    return null;
            List<HitRecord?> b = this.secondShape.rayIntersectionList(ray);
            List<HitRecord?> legalHits = new List<HitRecord?>();

            for (int i = 0; i < a.Count; i++)
            {
                if (!(this.secondShape.isPointInside((Point) a[i]?.worldPoint)))
                {
                    legalHits.Add(a[i]);
                }
            }

            if (b[0] != null)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside((Point)b[i]?.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            if (legalHits.Count == 0)
                return null;

            int iHit = 0;
            for (int i = 1; i < legalHits.Count; i++) 
                if (legalHits[i]?.t < legalHits[iHit]?.t)
                    iHit = i;
            

            return legalHits[iHit];
        }

        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> legalHits = new List<HitRecord?>();
            List<HitRecord?> a = this.firstShape.rayIntersectionList(ray);
            if (a[0] == null)
            {
                legalHits.Add(null);
                return legalHits;
            }
            List<HitRecord?> b = this.secondShape.rayIntersectionList(ray);

            for (int i = 0; i < a.Count; i++)
            {
                if (!(this.secondShape.isPointInside((Point) a[i]?.worldPoint)))
                {
                    legalHits.Add(a[i]);
                }
            }

            if (b[0] != null)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside((Point)b[i]?.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            return legalHits;
        }

        public override bool isPointInside(Point a)
        {
            return this.firstShape.isPointInside(a) && !this.secondShape.isPointInside(a);
        }
        
    } //CSGDifference

} // Trace