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
    /// Abstract class that implements a generic shape. This class is used to generate
    /// some basic shapes such as sphere and plane.
    /// </summary>
    public abstract class Shape
    {
        // Fields 

        /// <summary>
        /// <see cref="Transformation associated to the shape">
        /// </summary>
        public Transformation transformation;


        // Methods

        /// <summary>
        /// Create a default shape, potentially associating a transformation to it.
        /// This is an abstract constructor,
        /// therefore it cannot be directly used in the code
        /// </summary>
        /// <param name="transformation"><see cref="Transformation"> associated to the sphere</param>
        public Shape(Transformation? transformation = null)
        {
            this.transformation = transformation ?? new Transformation(1);
        }

        /// <summary>
        /// Compute the intersection between a ray and this shape. This method has to be redefined in
        /// derived classes
        /// </summary>
        public abstract HitRecord? rayIntersection(Ray ray);
    }

    /// <summary>
    /// A 3D unit sphere centered on the origin of the axes
    /// </summary>
    public class Sphere : Shape
    {

        /// <summary>
        /// Create a unit sphere, potentially associating a transformation to it
        /// </summary>
        /// <param name="transformation"><see cref="Transformation"> associated to the sphere</param>
        public Sphere(Transformation? transformation = null) : base(transformation) { }

        /// <summary>
        /// Checks if a ray intersects the sphere and return the hit record
        /// </summary>
        /// <param name="ray"><see cref="Ray"> that you want to check if intersect the sphere</param>
        /// <returns><see cref="HitRecord"> or <see cref="null"> if no intersection was found.</returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            Vec originVec = invRay.origin.toVec();
            float a = invRay.dir.getSquaredNorm();
            float b = 2.0f * originVec * invRay.dir;
            float c = originVec.getSquaredNorm() - 1.0f;

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
                return null;

            float sqrtDelta = (float)Math.Sqrt((float)delta);
            float tmin = (-b - sqrtDelta) / (2.0f * a);
            float tmax = (-b + sqrtDelta) / (2.0f * a);

            float firstHitT;
            if (tmin > invRay.tmin && tmin < invRay.tmax)
                firstHitT = tmin;
            else if (tmax > invRay.tmin && tmax < invRay.tmax)
                firstHitT = tmax;
            else
                return null;

            Point hitPoint = invRay.at(firstHitT);
            return new HitRecord(
                this.transformation * hitPoint,
                this.transformation * _sphereNormal(hitPoint, ray.dir),
                _spherePointToUV(hitPoint),
                firstHitT,
                ray
            );

        }

        /// <summary>
        /// Checks if a ray intersects the sphere and return the hit record with the biggest t
        /// </summary>
        /// <param name="ray"><see cref="Ray"> that you want to check if intersect the sphere</param>
        /// <returns><see cref="HitRecord"> or <see cref="null"> if no intersection was found.</returns>
        public HitRecord? rayIntersectionMax(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            Vec originVec = invRay.origin.toVec();
            float a = invRay.dir.getSquaredNorm();
            float b = 2.0f * originVec * invRay.dir;
            float c = originVec.getSquaredNorm() - 1.0f;

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
                return null;

            float sqrtDelta = (float)Math.Sqrt((float)delta);
            float tmax = (-b + sqrtDelta) / (2.0f * a);

            float firstHitT;
            
            if (tmax > invRay.tmin && tmax < invRay.tmax)
                firstHitT = tmax;
            else
                return null;

            Point hitPoint = invRay.at(firstHitT);
            return new HitRecord(
                this.transformation * hitPoint,
                this.transformation * _sphereNormal(hitPoint, ray.dir),
                _spherePointToUV(hitPoint),
                firstHitT,
                ray
            );
        }

        /// <summary>
        /// Compute the normal of a unit sphere<br/>
        /// The normal is computed for <see cref="Point"> (a point on the surface of the
        /// sphere), and it is chosen so that it is always in the opposite
        /// direction with respect to `ray_dir`.
        /// </summary>
        /// <param name="point"><see cref="Point"> on the surface of the sphere</param>
        /// <param name="rayDir"><see cref="Vec"> radius of the sphere</param>
        /// <returns><see cref="Normal"> object</returns>
        private static Normal _sphereNormal(Point point, Vec rayDir)
        {
            Normal result = new Normal(point.x, point.y, point.z);
            if (point.toVec() * rayDir > 0.0f)
                result = -result;
            return result;
        }

        /// <summary>
        /// Convert a 3D point on the surface of the unit sphere into a (u, v) 2D point
        /// </summary>
        /// <param name="point">3D <see cref="Point"/></param>
        /// <returns><see cref="Vec2D"/></returns>
        private static Vec2D _spherePointToUV(Point point)
            => new Vec2D(
                    (((float)Math.Atan2(point.y, point.x) + (2f * Constant.PI)) % (2f * Constant.PI)) / (2.0f * Constant.PI),
                    (float)Math.Acos(point.z) / Constant.PI
                );

        /// <summary>
        /// Method that assert if a Point is inside of the Unitary Shape.
        /// It is needed to implement CSG Shape
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public bool isPointInside(Point a)
        {
            a = this.transformation.getInverse() * a;
            return a.x * a.x + a.y * a.y + a.z * a.z <= 1;
        } 
    }

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
    }
    /// <summary>
    /// A 3D Shape created by the difference 
    /// of two Shapes (Constructive Solid Geometry).
    /// 
    /// Datamembers: Shape firstsShape, Shape secondShape.
    /// </summary>
    public class CSGDifference : Shape
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
    }



    public class Plane : Shape
    {
        public Plane(Transformation? transformation = null) : base(transformation) { }
        public override HitRecord? rayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
           // Vec invRayOrigin = invRay.origin.toVec();

            if (invRay.dir.z == 0) return null;
            else
            {
                float tHit = -invRay.origin.z / invRay.dir.z;
                if (tHit > invRay.tmin && tHit < invRay.tmax)
                {
                    Point hitPoint = invRay.at(tHit);
                    return new HitRecord(
                        wp: this.transformation * hitPoint,
                        nm: this.transformation * _stdPlaneNormal(ray),
                        sp: _stdPlanePointToUV(hitPoint),
                        tt: tHit,
                        r: ray
                    );
                    
                }
                else return null;

            }
        }

        private static Normal _stdPlaneNormal(Ray r, Point? hitPoint = null) {

            // hitPoint is a nullable parameter because it is not needed. Yet, someone could be used to 
            // call it because it is needed on a Sphere, so I unclude it to avoid unnecessary errors

            Normal res = new Normal(0.0f,0.0f,1.0f);

            if (r.dir * res <0.0f) return res;
            else return -res;

        }

        /// <summary>
        /// Convert a point on the z=0 plane into the (u,v) space using tile pattern
        /// In this way, u and v are in [0,1)
        /// </summary>
        /// <param name="point"> Point on the standard plane </param>
        /// <returns> Vec2D with u and v as coordinates</returns>
        private static Vec2D _stdPlanePointToUV(Point point)
        {
            float u = point.x - Convert.ToInt32(point.x);
            float v = point.y - Convert.ToInt32(point.y);

            return new Vec2D(u, v);
        }

        public bool isPointOnPlane(Point p)
        {
            Point invtransfpoint = this.transformation.getInverse() * p;
            if (Utility.areClose(invtransfpoint.z, 0.0f)) return true;
            else return false;
        }

    }

    public class Box : Shape
    {
        public Point min;
        public Point max;

        public Box(Point? min = null, Point? max = null, Transformation? transformation = null) : base(transformation)
        {
            // Controllo min max

            this.min = min ?? new Point(-1f, -1f, -1f);
            this.max = max ?? new Point(1f, 1f, 1f);
        }

        public bool isPointInside(Point a)
            => a.x > min.x && a.x < max.x && a.y > min.y && a.y < max.y && a.z > min.z && a.z < max.z;

        public override HitRecord? rayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            float t1x, t2x, t1y, t2y, t1z, t2z;

            t1x = (min.x - invRay.origin.x) / invRay.dir.x;
            t2x = (max.x - invRay.origin.x) / invRay.dir.x;
            t1y = (min.y - invRay.origin.y) / invRay.dir.y;
            t2y = (max.y - invRay.origin.y) / invRay.dir.y;
            t1z = (min.z - invRay.origin.z) / invRay.dir.z;
            t2z = (max.z - invRay.origin.z) / invRay.dir.z;

            float tEnter = Utility.Max(Math.Min(t1x, t2x), Math.Min(t1y, t2y), Math.Min(t1z, t2z));
            float tExit = Utility.Min(Math.Max(t1x, t2x), Math.Max(t1y, t2y), Math.Max(t1z, t2z));

            if (tEnter < 0 && tExit < 0) return null;
            if (tEnter < 0) tEnter = tExit;

            Point hitPoint = invRay.at(tEnter);
            return new HitRecord(
                this.transformation * hitPoint,
                this.transformation * this._boxNormal(hitPoint, ray.dir),
                this._boxPointToUV(hitPoint),
                tEnter,
                ray
            );
        }

        private Normal _boxNormal(Point point, Vec rayDir)
        {
            Normal result = new Normal();
            if (Utility.areClose(point.x, this.min.x)) result = new Normal(-1f, point.y, point.z);
            if (Utility.areClose(point.x, this.max.x)) result = new Normal(1f, point.y, point.z);
            if (Utility.areClose(point.y, this.min.y)) result = new Normal(point.x, -1f, point.z);
            if (Utility.areClose(point.y, this.max.y)) result = new Normal(point.x, 1f, point.z);
            if (Utility.areClose(point.z, this.min.z)) result = new Normal(point.x, point.y, -1f);
            if (Utility.areClose(point.z, this.max.z)) result = new Normal(point.x, point.y, 1f);
            return result.Normalize();
        }


        // To be completed
        private Vec2D _boxPointToUV(Point point)
        {
            return new Vec2D(0.5f, 0.5f);
        }


    }


}