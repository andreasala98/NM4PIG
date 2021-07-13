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

using System.Collections.Generic;

namespace Trace
{


    /// <summary>
    /// A 3D Shape created by the union 
    /// of two Shapes (Constructive Solid Geometry).
    /// 
    /// Given a <see cref="Ray"/>, it calculates all intersection of the ray with
    /// the two shapes, and it returns only the closest one.
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
        /// Basic constructor of the class
        /// </summary>
        /// <param name="a">First Shape</param>
        /// <param name="b">Second Shape</param>
        /// <param name="transf">Transformation</param>
        public CSGUnion(Shape a, Shape b, Transformation? transf = null) : base(transf, null)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        /// <summary>
        /// Checks if a Ray intersects the new CSGUnion Shape.
        /// It is an override from the abstract class <see cref="Shape"/>
        /// </summary>
        /// <param name="ray"> The intersecting <see cref="Ray"/> object</param>
        /// <returns> A <see cref="HitRecord"/> if there is an intersection, otherwise null</returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            HitRecord? a = this.firstShape.rayIntersection(invRay);
            HitRecord? b = this.secondShape.rayIntersection(invRay);

            if (a == null) return b;
            else if (b == null) return a;
            else // a!=null, b!=null
            {
                // second condition is needed when the Ray originates inside the Shape
                if (a?.t < b?.t && !(this.secondShape.isPointInside((Point)a?.worldPoint))) return a;
                else if (a?.t < b?.t && (this.secondShape.isPointInside((Point)a?.worldPoint))) return b;
                else if (b?.t < a?.t && !(this.firstShape.isPointInside((Point)b?.worldPoint))) return b;
                else return a;
            }
        }

        /// <summary>
        /// This method checks if a Ray intersects with the CSG Union shape.
        /// It simply return true if there is any intersection.
        /// </summary>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            HitRecord? a = this.firstShape.rayIntersection(invRay);
            HitRecord? b = this.secondShape.rayIntersection(invRay);

            if (a == null && b == null) return false;
            return true;
        }

        /// <summary>
        /// Method that creates a list of all the legal intersections (in form of a <see cref="HitRecord"/> list) with a given Ray.
        /// The HitRecord are then ordered by the t datamember, in ascending order.
        /// It is overriden from the abstract Shape class. 
        /// </summary>
        /// <param name="ray"> The fired <see cref="Ray"/> object. </param>
        /// <returns> A List of all legal <see cref="HitRecord"/>s.</returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            List<HitRecord?> a = this.firstShape.rayIntersectionList(invRay);

            List<HitRecord?> b = this.secondShape.rayIntersectionList(invRay);

            List<HitRecord?> hits = new List<HitRecord?>();

            if (a.Count != 0)
            {
                for (int i = 0; i < a.Count; i++)
                {
                    if (!(this.secondShape.isPointInside((Point)a[i]?.worldPoint)))
                    {
                        hits.Add(a[i]);
                    }
                }
            }

            if (b.Count != 0)
            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (!(this.firstShape.isPointInside((Point)b[i]?.worldPoint)))
                    {
                        hits.Add(b[i]);
                    }
                }
            }

            if (hits.Count == 0)
            {
                return hits;
            }

            hits.Sort();
            return hits;
        }

        /// <summary>
        /// Method that computes whether a Point is inside the CSG Union.
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
    /// You subtract the second Shape from the first Shape.
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
        public CSGDifference(Shape a, Shape b, Transformation? transf = null) : base(transf, null)
        {
            this.firstShape = a;
            this.secondShape = b;
        }

        /// <summary>
        /// Checks if a Ray intersects the new CSGUnion Shape.
        /// It is an override from the abstract class <see cref="Shape"/>
        /// </summary>
        /// <param name="ray"> The intersecting <see cref="Ray"/> object</param>
        /// <returns> A <see cref="HitRecord"/> if there is an intersection, otherwise null</returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            List<HitRecord?> intersections = rayIntersectionList(ray);
            if (intersections.Count == 0) return null;
            return intersections[0];
        }


        /// <summary>
        /// Method that creates a list of all the legal intersections (in form of a <see cref="HitRecord"/> list) with a given Ray.
        /// The HitRecord are then ordered by the t datamember, in ascending order.
        /// It is overriden from the abstract Shape class. 
        /// </summary>
        /// <param name="ray"> The fired <see cref="Ray"/> object. </param>
        /// <returns> A List of all legal <see cref="HitRecord"/>s.</returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            List<HitRecord?> legalHits = new List<HitRecord?>();
            List<HitRecord?> a = this.firstShape.rayIntersectionList(invRay);
            if (a.Count == 0)
            {
                return legalHits;
            }
            List<HitRecord?> b = this.secondShape.rayIntersectionList(invRay);

            for (int i = 0; i < a.Count; i++)
            {
                // Keep only the intersection points not inside the second shape
                if (!this.secondShape.isPointInside(a[i].Value.worldPoint))
                {
                    legalHits.Add(a[i]);
                }
            }

            if (b.Count != 0)
            {
                // Keep only the intersection inside the first shape
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside(b[i].Value.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            legalHits.Sort();
            return legalHits;
        }

        /// <summary>
        /// Method that computes whether a Point is inside the CSG Difference.
        /// 
        /// It's overriden from the abstract Shape class.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override bool isPointInside(Point a)
        {
            return this.firstShape.isPointInside(a) && !this.secondShape.isPointInside(a);
        }

        /// <summary>
        /// This method checks if a Ray intersects with the CSG Difference shape.
        /// </summary>
        /// <param name="ray"> The fired <see cref="Ray"/> object</param>
        /// <returns> True if there is any intersection</returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            List<HitRecord?> legalHits = new List<HitRecord?>();
            List<HitRecord?> a = this.firstShape.rayIntersectionList(invRay);
            if (a.Count == 0)
            {
                return false;
            }
            List<HitRecord?> b = this.secondShape.rayIntersectionList(invRay);

            for (int i = 0; i < a.Count; i++)
            {
                // Keep only the intersection points not inside the second shape
                if (!this.secondShape.isPointInside(a[i].Value.worldPoint))
                {
                    legalHits.Add(a[i]);
                }
            }

            if (b.Count != 0)
            {
                // Keep only the intersection inside the first shape
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside(b[i].Value.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            if (legalHits.Count == 0) return false;
            return true;
        }

    } //CSGDifference

    /// <summary>
    /// A 3D Shape created by the intersection 
    /// of two Shapes (Constructive Solid Geometry).
    /// 
    /// Datamembers: Shape firstShape, Shape secondShape.
    /// </summary>
    public class CSGIntersection : Shape
    {
        public Shape firstShape;

        public Shape secondShape;

        public CSGIntersection(Shape a, Shape b, Transformation? transf = null) : base(transf, null)
        {
            this.firstShape = a;
            this.secondShape = b;
        }


        /// <summary>
        /// Method that creates a list of all the legal intersections (in form of a <see cref="HitRecord"/> list) with a given Ray.
        /// The HitRecord are then ordered by the t datamember, in ascending order.
        /// It is overriden from the abstract Shape class. 
        /// </summary>
        /// <param name="ray"> The fired <see cref="Ray"/> object. </param>
        /// <returns> A List of all legal <see cref="HitRecord"/>s.</returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            List<HitRecord?> legalHits = new List<HitRecord?>();
            List<HitRecord?> a = this.firstShape.rayIntersectionList(invRay);
            if (a.Count == 0)
            {
                return legalHits;
            }
            List<HitRecord?> b = this.secondShape.rayIntersectionList(invRay);
            if (b.Count == 0)
            {
                return legalHits;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (this.secondShape.isPointInside((Point)a[i]?.worldPoint))
                {
                    legalHits.Add(a[i]);
                }
            }

            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside((Point)b[i]?.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            legalHits.Sort();
            return legalHits;
        }

        /// <summary>
        /// Checks if a Ray intersects the new CSGIntersection Shape.
        /// It is an override from the abstract class <see cref="Shape"/>
        /// </summary>
        /// <param name="ray"> The intersecting <see cref="Ray"/> object</param>
        /// <returns> A <see cref="HitRecord"/> if there is an intersection, otherwise null</returns>
        /// 
        public override HitRecord? rayIntersection(Ray ray)
        {
            List<HitRecord?> intersections = rayIntersectionList(ray);
            if (intersections.Count == 0) return null;
            return intersections[0];
        }

        /// <summary>
        /// Method that computes whether a Point is inside the Shape.
        /// 
        /// It's overriden from the abstract Shape class.
        /// </summary>
        /// <param name="a"> The point possibly inside the Shape.</param>
        /// <returns> True if there is any intersecction.</returns>
        public override bool isPointInside(Point a)
        {
            return this.firstShape.isPointInside(a) && this.secondShape.isPointInside(a);
        }


        /// <summary>
        /// This method checks if a Ray intersects the CSG Intersection shape.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            List<HitRecord?> legalHits = new List<HitRecord?>();
            List<HitRecord?> a = this.firstShape.rayIntersectionList(invRay);
            if (a.Count == 0)
            {
                return false;
            }
            List<HitRecord?> b = this.secondShape.rayIntersectionList(invRay);
            if (b.Count == 0)
            {
                return false;
            }

            for (int i = 0; i < a.Count; i++)
            {
                if (this.secondShape.isPointInside((Point)a[i]?.worldPoint))
                {
                    legalHits.Add(a[i]);
                }
            }

            {
                for (int i = 0; i < b.Count; i++)
                {
                    if (this.firstShape.isPointInside((Point)b[i]?.worldPoint))
                    {
                        legalHits.Add(b[i]);
                    }
                }
            }

            if (legalHits.Count == 0) return false;
            return true;
        }
    } // CSGIntersection

} // Trace