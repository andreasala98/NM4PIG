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
    /// Abstract class that implements a generic shape
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
        /// Create a unit sphere, potentially associating a transformation to it
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