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
using System.Linq;
using System.Collections.Generic;

#nullable enable
namespace Trace
{
    /// <summary>
    /// Abstract class that implements a generic shape. This class is used to generate
    /// some basic shapes such as sphere and plane. Use only concrete subclasses
    /// </summary>
    public abstract class Shape
    {
        // Fields 

        /// <summary>
        /// <see cref="Transformation"/> associated to the shape">
        /// </summary>
        public Transformation transformation;

        /// <summary>
        ///  The shape's <see ref="Material"/>
        /// </summary>
        public Material material;
        // Methods

        /// <summary>
        /// Create a default shape, potentially associating a transformation to it.
        /// This is an abstract constructor,
        /// therefore it cannot be directly used in the code
        /// </summary>
        /// <param name="transformation"><see cref="Transformation"> associated to the sphere</param>
        public Shape(Transformation? transformation = null, Material? material = null)
        {
            this.transformation = transformation ?? new Transformation(1);
            this.material = material ?? new Material();
        }

        /// <summary>
        /// Compute the intersection between a ray and this shape. This method has to be redefined in
        /// derived classes
        /// </summary>
        /// <param name=ray> The intersecting <see cref="Ray"/></param>
        public abstract HitRecord? rayIntersection(Ray ray);

        /// <summary>
        /// Compute the list of all intersections between a ray and this shape. This method has to be redefined in
        /// derived classes
        /// </summary>
        /// <param name=ray> The intersecting <see cref="Ray"/></param>
        public abstract List<HitRecord?> rayIntersectionList(Ray ray);

        /// <summary>
        ///  Calculate whether there is any intersection between the shape and a ray.
        /// </summary>
        /// <param name="ray">The intersecting <see cref="Ray"/></param>
        /// <returns>True if there is at least one intersection</returns>
        public abstract bool quickRayIntersection(Ray ray);

        /// <summary>
        /// Computes whether a point is inside the shape. It must be redefinde in derived classes.
        /// </summary>
        /// <param name="a"></param>
        public abstract bool isPointInside(Point a);

        /// <summary>
        /// Operator to compute CSG Difference between two Shapes
        /// </summary>
        /// <param name="left">first shape</param>
        /// <param name="right">second shape</param>
        /// <returns>A new CSGDifference</returns>
        public static CSGDifference operator -(Shape left, Shape right)
            => new CSGDifference(left, right);

        /// <summary>
        /// Operator to compute CSG Union between two Shapes
        /// </summary>
        /// <param name="left">first shape</param>
        /// <param name="right">second shape</param>
        /// <returns>A new CSGUnion</returns>           
        public static CSGUnion operator +(Shape left, Shape right)
            => new CSGUnion(left, right);

        /// <summary>
        /// Operator to compute CSG Intersetion between two shapes
        /// </summary>
        /// <param name="left">first shape</param>
        /// <param name="right">second shape</param>
        /// <returns>A new CSGIntersection</returns>
        public static CSGIntersection operator *(Shape left, Shape right)
            => new CSGIntersection(left, right);
    }


    /// <summary>
    /// A 3D unit sphere centered on the origin of the axes. 
    /// It is possible to apply transformations to the sphere
    /// </summary>
    public class Sphere : Shape
    {

        /// <summary>
        /// Create a unit sphere, potentially associating a transformation to it
        /// </summary>
        /// <param name="transformation"><see cref="Transformation"> associated to the sphere</param>
        public Sphere(Transformation? transformation = null, Material? material = null) : base(transformation, material) { }

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
                ray,
                this
            );

        }

        /// <summary>
        /// Checks if a ray intersects the sphere and return a list of all the hit records
        /// </summary>
        /// <param name="ray"><see cref="Ray"> that you want to check if it intersects the sphere</param>
        /// <returns>List of <see cref="HitRecord"> or <see cref="null"> if no intersection was found.</returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> intersections = new List<HitRecord?>();
            Ray invRay = ray.Transform(this.transformation.getInverse());
            Vec originVec = invRay.origin.toVec();
            float a = invRay.dir.getSquaredNorm();
            float b = 2.0f * originVec * invRay.dir;
            float c = originVec.getSquaredNorm() - 1.0f;

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
            {
                return intersections;       // Here is empty
            }

            float sqrtDelta = (float)Math.Sqrt((float)delta);
            float tmin = (-b - sqrtDelta) / (2.0f * a);
            float tmax = (-b + sqrtDelta) / (2.0f * a);

            if (tmin > invRay.tmin && tmin < invRay.tmax)
            {
                Point hitPoint = invRay.at(tmin);

                intersections.Add(new HitRecord(
                                                this.transformation * hitPoint,
                                                this.transformation * _sphereNormal(hitPoint, ray.dir),
                                                _spherePointToUV(hitPoint),
                                                tmin,
                                                ray,
                                                this
                                                )
                                                );
            }

            if (tmax > invRay.tmin && tmax < invRay.tmax)
            {
                Point hitPoint = invRay.at(tmax);

                intersections.Add(new HitRecord(
                                                this.transformation * hitPoint,
                                                this.transformation * _sphereNormal(hitPoint, ray.dir),
                                                _spherePointToUV(hitPoint),
                                                tmax,
                                                ray,
                                                this
                                                )
                                                );
            }

            return intersections;       // If there are no intersections return an empty list
        }

        /// <summary>
        /// Compute the normal of a unit sphere<br/>
        /// The normal is computed for <see cref="Point"> (a point on the surface of the
        /// sphere), and it is chosen so that it is always in the opposite
        /// direction with respect to `ray_dir`.
        /// </summary>
        /// <param name="point"><see cref="Point"> on the surface of the sphere</param>
        /// <param name="rayDir"><see cref="Vec"> direction of the incoming ray</param>
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
        /// Method that checks if a Point is inside of the unit sphere.
        /// It is needed to implement CSG functionalities.
        /// </summary>
        /// <param name="a">Point inside/outside the sphere</param>
        /// <returns> True if INSIDE</returns>
        public override bool isPointInside(Point a)
        {
            a = this.transformation.getInverse() * a;
            return a.x * a.x + a.y * a.y + a.z * a.z <= 1;
        }

        /// <summary>
        /// Method that checks if a Ray intersects the sphere
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            Vec originVec = invRay.origin.toVec();
            float a = invRay.dir.getSquaredNorm();
            float b = 2.0f * originVec * invRay.dir;
            float c = originVec.getSquaredNorm() - 1.0f;

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
                return false;

            float sqrtDelta = (float)Math.Sqrt((float)delta);
            float tmin = (-b - sqrtDelta) / (2.0f * a);
            float tmax = (-b + sqrtDelta) / (2.0f * a);

            return (tmin > invRay.tmin && tmin < invRay.tmax) || (tmax > invRay.tmin && tmax < invRay.tmax);
        }
    }

    // ################################

    /// <summary>
    /// A 2D plane that you can put in the scene. Unless transformations are applied, the z=0 plane is constructed
    /// </summary>
    public class Plane : Shape
    {
        /// <summary>
        /// Construct a plane and potentially associate a transformation to it
        /// </summary>
        /// <param name="transformation"></param>
        public Plane(Transformation? transformation = null, Material? material = null) : base(transformation, material) { }

        /// <summary>
        /// Intersect a <see cref='Ray'/> object with the plane. It return a <see cref="HitRecord"/> object
        /// with the details of the intersection, or null if there is no intersection
        /// </summary>
        /// <param name="ray"></param>
        /// <returns> Hit Record with the intersection</returns>
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
                        r: ray,
                        shape: this
                    );

                }
                else return null;

            }
        }

        /// <summary>
        /// Intersect a <see cref='Ray'/> object with the plane. It returns a List of <see cref="HitRecord"/> 
        /// and it is needed for CSG features.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> hitList = new List<HitRecord?>();
            hitList.Add(this.rayIntersection(ray));
            return hitList;
        }


        /// <summary>
        /// Compute the oriented normal to the z=0 plane. It is a private method, so it cannot
        /// be used outside of this class.
        /// </summary>
        /// <param name="r"> Ray intersecting the plane </param>
        /// <param name="hitPoint"> Point of intersection (optional parameter)</param>
        /// <returns> The oriented <see cref="Normal"/> object, so that the scalar product with the ray is negative</returns>
        private static Normal _stdPlaneNormal(Ray r, Point? hitPoint = null)
        {

            // hitPoint is a nullable parameter because it is not needed. Yet, someone could be used to 
            // call it because it is needed on a Sphere, so I unclude it to avoid unnecessary errors

            Normal res = new Normal(0.0f, 0.0f, 1.0f);

            if (r.dir * res < 0.0f) return res;
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
            float u = point.x - MathF.Floor(point.x);
            float v = point.y - MathF.Floor(point.y);
            return new Vec2D(u, v);
        }

        /// <summary>
        /// Checks whether a <see cref="Point"/> lies on the plane
        /// </summary>
        /// <param name="p"> The <see cref="Point"/> object</param>
        /// <returns> True if the poit lies on the plane </returns>
        public override bool isPointInside(Point p)
        {
            Point invtransfpoint = this.transformation.getInverse() * p;
            if (Utility.areClose(invtransfpoint.z, 0.0f)) return true;
            else return false;
        }

        /// <summary>
        /// Method that checks if a Ray intersects the plane
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>

        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            if (invRay.dir.z == 0) return false;

            float tHit = -invRay.origin.z / invRay.dir.z;
            return (tHit > invRay.tmin && tHit < invRay.tmax);
        }

    }

    /// <summary>
    /// A 3D axis-aligned box (AAB)
    /// </summary>
    public class Box : Shape
    {
        /// <summary>
        /// A <see cref="Point"> object to record minimum x,y,z of the box
        /// </summary>
        public Point min;
        /// <summary>
        /// A <see cref="Point"> object to record maximum x,y,z of the box
        /// </summary>
        public Point max;

        /// <summary>
        /// Constructor for box. If min and max are not passed, they are respectively set to
        /// min=(-1,-1,-1) and max(1,1,1). You can optionally pass a transformation to the box
        /// </summary>
        /// <param name="min"> Minimum edge</param>
        /// <param name="max">Maximum edge</param>
        public Box(Point? min = null, Point? max = null, Transformation? transformation = null, Material? material = null) : base(transformation, material)
        {
            // Controllo min max

            this.min = min ?? new Point(-1f, -1f, -1f);
            this.max = max ?? new Point(1f, 1f, 1f);
        }

        /// <summary>
        /// Checks if a <see cref="Point"/> lies inside the box
        /// </summary>
        /// <param name="a"> The point </param>
        /// <returns>True if the point is inside</returns>
        public override bool isPointInside(Point a)
        {
            a = this.transformation.getInverse() * a;
            return a.x >= min.x && a.x <= max.x && a.y >= min.y && a.y <= max.y && a.z >= min.z && a.z <= max.z;
        }


        /// <summary>
        /// Intersect a <see cref="Ray"/> object with the box and returns a 
        /// <see cref="HitRecord"/> if there is an intersection,
        /// otherwise it returns null.
        /// </summary>
        /// <param name="ray"> The intersecting ray</param>
        /// <returns> A  nullable <see cref="HitRecord"> with the details of the intersection</returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            List<HitRecord?> intersections = rayIntersectionList(ray);
            if (intersections.Count == 0) return null;
            return intersections[0];
        }

        /// <summary>
        /// Private method to calculate the normal to a box, with a <see cref="Ray"> 
        /// object intersecting the box at a <see cref="Point">.
        /// </summary>
        /// <param name="point"> The point of intersection</param>
        /// <param name="rayDir"> Intersecting ray </param>
        /// <returns> The oriented normal</returns>
        private Normal _boxNormal(Point point, Vec rayDir)
        {
            Normal result = new Normal(0f, 0f, 1f);
            if (Utility.areClose(point.x, this.min.x)) result = -Constant.VEC_X_N;
            else if (Utility.areClose(point.x, this.max.x)) result = Constant.VEC_X_N;
            else if (Utility.areClose(point.y, this.min.y)) result = -Constant.VEC_Y_N;
            else if (Utility.areClose(point.y, this.max.y)) result = Constant.VEC_Y_N;
            else if (Utility.areClose(point.z, this.min.z)) result = -Constant.VEC_Z_N;
            else if (Utility.areClose(point.z, this.max.z)) result = Constant.VEC_Z_N;
            if (point.toVec() * rayDir > 0.0f)
                result = -result;
            return result;
        }


        /// <summary>
        /// Private method to perform UV mapping of a Point which lies on a box. 
        /// Reference <br/>
        /// http://ilkinulas.github.io/development/unity/2016/05/06/uv-mapping.html <br/>
        /// Note that the article use a left-handed orthonormal base, so this has been changed a little
        /// </summary>
        /// <param name="point"> <see cref="Point"> on the box</param>
        /// <returns> A 2D Vec with (u,v) coordiantes</returns>
        private Vec2D _boxPointToUV(Point point)
        {
            float var1 = 0f, var2 = 0f;

            int face = 0;
            if (Utility.areClose(point.x, this.min.x)) { face = 1; var1 = 1 - (point.z - this.min.z) / (this.max.z - this.min.z); var2 = (point.y - this.min.y) / (this.max.y - this.min.y); }
            else if (Utility.areClose(point.y, this.max.y)) { face = 2; var1 = (point.x - this.min.x) / (this.max.x - this.min.x); var2 = (point.z - this.min.z) / (this.max.z - this.min.z); }
            else if (Utility.areClose(point.z, this.min.z)) { face = 3; var1 = (point.x - this.min.x) / (this.max.x - this.min.x); var2 = (point.y - this.min.y) / (this.max.y - this.min.y); }
            else if (Utility.areClose(point.y, this.min.y)) { face = 4; var1 = (point.x - this.min.x) / (this.max.x - this.min.x); var2 = 1f - (point.z - this.min.z) / (this.max.z - this.min.z); }
            else if (Utility.areClose(point.x, this.max.x)) { face = 5; var1 = (point.z - this.min.z) / (this.max.z - this.min.z); var2 = (point.y - this.min.y) / (this.max.y - this.min.y); }
            else if (Utility.areClose(point.z, this.max.z)) { face = 6; var1 = 1f - (point.x - this.min.x) / (this.max.x - this.min.x); var2 = (point.y - this.min.y) / (this.max.y - this.min.y); }

            float u = 0f, v = 0f;
            if (face == 1) { u = 0.00f + var1 / 4f; v = 1f / 3f + var2 / 3f; }
            else if (face == 2) { u = 0.25f + var1 / 4f; v = 2f / 3f + var2 / 3f; }
            else if (face == 3) { u = 0.25f + var1 / 4f; v = 1f / 3f + var2 / 3f; }
            else if (face == 4) { u = 0.25f + var1 / 4f; v = 0f / 3f + var2 / 3f; }
            else if (face == 5) { u = 0.50f + var1 / 4f; v = 1f / 3f + var2 / 3f; }
            else if (face == 6) { u = 0.75f + var1 / 4f; v = 1f / 3f + var2 / 3f; }
            return new Vec2D(u, v);
        }

        /// <summary>
        /// Method that checks if a Ray intersects a Box. Overridden from abstract class <see cref="Shape"/>
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            float t0 = 0, t1 = ray.tmax;
            for (int i = 0; i < 3; i++)
            {
                float invRayDir = 1 / invRay.dir.ToList()[i];
                float tNear = (min.ToList()[i] - invRay.origin.ToList()[i]) * invRayDir;
                float tFar = (max.ToList()[i] - invRay.origin.ToList()[i]) * invRayDir;

                if (tNear > tFar)
                {
                    float tmp = tNear;
                    tNear = tFar;
                    tFar = tmp;
                }

                t0 = tNear > t0 ? tNear : t0;
                t1 = tFar < t1 ? tFar : t1;
                if (t0 > t1) return false;
            }

            if (t0 > 0f || t1 < 0f)
                return true;

            return false;
        }

        /// <summary>
        /// Intersect a <see cref="Ray"/> object with the box and returns a 
        ///List of <see cref="HitRecord"/> if there is an intersection,
        /// otherwise it returns a List with a null object.
        /// </summary>
        /// <param name="ray"> The intersecting ray</param>
        /// <returns> A  List of nullable <see cref="HitRecord"> with the details of the intersections</returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            float t0 = 0, t1 = ray.tmax;
            for (int i = 0; i < 3; i++)
            {
                float invRayDir = 1 / invRay.dir.ToList()[i];
                float tNear = (min.ToList()[i] - invRay.origin.ToList()[i]) * invRayDir;
                float tFar = (max.ToList()[i] - invRay.origin.ToList()[i]) * invRayDir;

                if (tNear > tFar)
                {
                    float tmp = tNear;
                    tNear = tFar;
                    tFar = tmp;
                }

                t0 = tNear > t0 ? tNear : t0;
                t1 = tFar < t1 ? tFar : t1;
                if (t0 > t1) return new List<HitRecord?>();
            }

            List<HitRecord?> hits = new List<HitRecord?>();

            if (t0 > 0f)
            {
                Point hitPoint0 = invRay.at(t0);
                hits.Add(new HitRecord(
                wp: this.transformation * hitPoint0,
                nm: (this.transformation * this._boxNormal(hitPoint0, ray.dir)),
                sp: this._boxPointToUV(hitPoint0),
                tt: t0,
                r: ray,
                shape: this
            ));
            }

            if (t1 > 0f)
            {
                Point hitPoint1 = invRay.at(t1);
                hits.Add(new HitRecord(
                    wp: this.transformation * hitPoint1,
                    nm: (this.transformation * this._boxNormal(hitPoint1, ray.dir)),
                    sp: this._boxPointToUV(hitPoint1),
                    tt: t1,
                    r: ray,
                    shape: this
                ));
            }

            return hits;
        }

    } // end of box

    /// <summary>
    /// Concrete class to represent a cylinder. If no transformation is passed, it is a cylinder with axis aligned along z, -0.5 < z < 0.5 and radius 1
    /// </summary>
    public class Cylinder : Shape
    {
        public Cylinder(Transformation? transformation = null, Material? material = null) : base(transformation, material) { }

        public Cylinder(Point center, float radius, float height, Vec direction, Material? material = null) : base(null, material)
        {
            Transformation scaling = Transformation.Scaling(new Vec(radius, radius, height));
            Transformation translation = Transformation.Translation(center.toVec());

            // Euler's angles
            direction = direction.Normalize();
            float cos = direction * Constant.VEC_Z;
            float thetaX = MathF.Atan2(-direction.y, direction.z);
            float thetaY = MathF.Atan2(direction.x, MathF.Sqrt(direction.y * direction.y + direction.z * direction.z));
            float thetaZ = MathF.Atan2(-(direction.x * direction.y) / (1 + direction.z), 1f - (direction.x * direction.x) / (1 + direction.z));
            Transformation rotation = Transformation.RotationZ(thetaZ) * Transformation.RotationY(thetaY) * Transformation.RotationX(thetaX);
            this.transformation = translation * rotation * scaling;
        }

        /// <summary>
        /// Method that checks if a ray intersects a cylinder
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            // Check if intersect lateral face
            float a = invRay.dir.x * invRay.dir.x + invRay.dir.y * invRay.dir.y;
            float b = invRay.dir.x * invRay.origin.x + invRay.dir.y * invRay.origin.y;
            float c = invRay.origin.x * invRay.origin.x + invRay.origin.y * invRay.origin.y - 1;

            float delta = b * b - a * c;
            float? t1 = null, t2 = null;

            if (delta > 0f)        // two solutions
            {
                t1 = (-b - MathF.Sqrt(delta)) / a;
                t2 = (-b + MathF.Sqrt(delta)) / a;

                if (t1 > 0f)
                {
                    Point hitPoint = invRay.at(t1.Value);
                    if (hitPoint.z > -0.5 && hitPoint.z < 0.5f)
                        return true;
                }

                if (t2 > 0)
                {
                    Point hitPoint = invRay.at(t2.Value);
                    if (hitPoint.z > -0.5 && hitPoint.z < 0.5f)
                        return true;
                }


            }

            // Check if intersect bottom face
            Plane planeBottom = new Plane(transformation: Transformation.Translation(0f, 0f, -0.5f));
            HitRecord? hitBottom = planeBottom.rayIntersection(invRay);
            if (hitBottom.HasValue)
            {
                Point pointInt = hitBottom.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < 1f)
                    return true;
            }

            // Check if intersect top face
            Plane planeTop = new Plane(transformation: Transformation.Translation(0f, 0f, 0.5f));
            HitRecord? hitTop = planeTop.rayIntersection(invRay);
            if (hitTop.HasValue)
            {
                Point pointInt = hitTop.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < 1f)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Overridden method from class <see cref="Shape"/>. It calculates the closest
        /// intersection between a <see cref="Ray"/> and the Cylinder
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            List<HitRecord?> intersections = rayIntersectionList(ray);
            if (intersections.Count == 0) return null;
            return intersections[0];
        }


        /// <summary>
        /// Overridden method that calculates every intersection between a Cylinder and a Ray.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());
            List<HitRecord?> hits = new List<HitRecord?>();

            // Check if intersect lateral face
            float a = invRay.dir.x * invRay.dir.x + invRay.dir.y * invRay.dir.y;
            float b = invRay.dir.x * invRay.origin.x + invRay.dir.y * invRay.origin.y;
            float c = invRay.origin.x * invRay.origin.x + invRay.origin.y * invRay.origin.y - 1;

            float delta = b * b - a * c;
            float? t1 = null, t2 = null;

            if (delta > 0f)        // two solutions
            {
                t1 = (-b - MathF.Sqrt(delta)) / a;
                t2 = (-b + MathF.Sqrt(delta)) / a;

                if (t1 > 0f)
                {
                    Point hitPoint = invRay.at(t1.Value);
                    if (hitPoint.z > -0.5 && hitPoint.z < 0.5f)
                        hits.Add(new HitRecord(
                                wp: this.transformation * hitPoint,
                                nm: (this.transformation * this._cylinderNormal(hitPoint, ray.dir)),
                                sp: this._cylinderPointToUV(hitPoint),
                                tt: t1.Value,
                                r: ray,
                                shape: this
                        ));
                }

                if (t2 > 0)
                {
                    Point hitPoint = invRay.at(t2.Value);
                    if (hitPoint.z > -0.5 && hitPoint.z < 0.5f)
                        hits.Add(new HitRecord(
                                wp: this.transformation * hitPoint,
                                nm: (this.transformation * this._cylinderNormal(hitPoint, ray.dir)),
                                sp: this._cylinderPointToUV(hitPoint),
                                tt: t2.Value,
                                r: ray,
                                shape: this
                        ));
                }


            }

            // Check if intersect bottom face
            Plane planeBottom = new Plane(transformation: Transformation.Translation(0f, 0f, -0.5f));
            HitRecord? hitBottom = planeBottom.rayIntersection(invRay);
            if (hitBottom.HasValue)
            {
                Point pointInt = hitBottom.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < 1f)
                    hits.Add(new HitRecord(
                                wp: this.transformation * pointInt,
                                nm: (this.transformation * this._cylinderNormal(pointInt, ray.dir)),
                                sp: this._cylinderPointToUV(pointInt),
                                tt: hitBottom.Value.t,
                                r: ray,
                                shape: this
                    ));
            }

            // Check if intersect top face
            Plane planeTop = new Plane(transformation: Transformation.Translation(0f, 0f, 0.5f));
            HitRecord? hitTop = planeTop.rayIntersection(invRay);
            if (hitTop.HasValue)
            {
                Point pointInt = hitTop.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < 1f)
                    hits.Add(new HitRecord(
                                wp: this.transformation * pointInt,
                                nm: (this.transformation * this._cylinderNormal(pointInt, ray.dir)),
                                sp: this._cylinderPointToUV(pointInt),
                                tt: hitTop.Value.t,
                                r: ray,
                                shape: this
                    ));
            }

            return hits.OrderBy(hit => hit?.t).ToList();
        }


        /// <summary>
        /// Calculate the <see cref="Normal"/> to the Cylinder at a certain point of intersection with a <see cref="Ray"/>.
        /// The normal is calculated so that its scalar product with the direction of the ray is negative
        /// </summary>
        /// <param name="point"></param>
        /// <param name="rayDir"></param>
        /// <returns></returns>
        private Normal _cylinderNormal(Point point, Vec rayDir)
        {
            Point invPoint = this.transformation.getInverse() * point;
            Vec invRayDir = this.transformation.getInverse() * rayDir;

            Normal result = new Normal(invPoint.x, invPoint.y, 0f);

            if (Utility.areClose(invPoint.z, 0.5f)) result = new Normal(0f, 0f, 1f);

            if (Utility.areClose(invPoint.z, -0.5f)) result = new Normal(0f, 0f, -1f);

            if (invPoint.x * invRayDir.x + invPoint.y * invRayDir.y > 0.0f)
                result = -result;
            return result;
        }

        /// <summary>
        /// U, V mapping for cylinder
        /// (0,0)     (0.5, 0)         (1,0)
        /// +---------------------------+
        /// |             |             |
        /// |    bottom   |     top     |
        /// |             |             |
        /// |             |             |
        /// +---------------------------+ (1, 0.5)
        /// |                           |
        /// |        lateral face       |
        /// |                           |
        /// |                           |
        /// +---------------------------+
        /// (0,1)                        (1,1)
        /// </summary>
        private Vec2D _cylinderPointToUV(Point point)
        {
            float u, v;
            if (Utility.areClose(point.z, 0.5f))
            {
                u = 0.5f + (point.x + 1f) / 4f;        // x in [-1,1] -> x+1 in [0,2] -> (x+1)/4 in [0,0.5]
                v = (point.y + 1f) / 4f;        // y in [-1,1] -> y+1 in [0,2] -> (y+1)/4 in [0,0.5]
            }
            else if (Utility.areClose(point.z, -0.5f))
            {
                u = (point.x + 1f) / 4f;        // x in [-1,1] -> x+1 in [0,2] -> (x+1)/4 in [0,0.5]
                v = (point.y + 1f) / 4f;        // y in [-1,1] -> y+1 in [0,2] -> (y+1)/4 in [0,0.5]
            }
            else
            {
                u = (((float)Math.Atan2(point.y, point.x) + (2f * Constant.PI)) % (2f * Constant.PI)) / (2.0f * Constant.PI);
                v = 1f - (point.z + 0.5f) / 2f;
            }

            return new Vec2D(u, v);
        }
        public override bool isPointInside(Point a)
        {
            Point inva = this.transformation.getInverse() * a;
            float distance = inva.x * inva.x + inva.y * inva.y;
            return inva.z <= 0.5f && inva.z >= -0.5f && distance <= 1f;
        }
    } // end of cylinder

    /// <summary>
    /// Class that implements a Cone shape.
    /// Its datamembars are (float) radius and (float) height
    /// </summary>
    public class Cone : Shape
    {

        public float radius;
        public float height;

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="h"></param>
        /// <param name="transformation"></param>
        /// <param name="material"></param>
        public Cone(float r = 1f, float h = 1f, Transformation? transformation = null, Material? material = null) : base(transformation, material)
        {
            this.radius = r;
            this.height = h;
        }


        /// <summary>
        /// Convert a 3D point on the surface of the cone into a (u, v) 2D point
        /// +-----------------------------+
        /// |              |              |
        /// |     bottom   |    lateral   |
        /// |     face     |              |
        /// |--------------|     face     |
        /// |              |              |
        /// |   nothing    |              |
        /// |              |              |
        /// +-----------------------------+
        /// </summary>
        /// <param name="point">3D <see cref="Point"/></param>
        /// <returns><see cref="Vec2D"/></returns>

        private Vec2D _conePointToUV(Point point)
        {
            float u = 0f, v = 0f;
            if (Utility.areClose(point.z, 0f))
            {
                // point is on the base
                u = (point.x + this.radius) / (4f * this.radius);       // u in [0,0.5]
                v = (point.y + this.radius) / (4f * this.radius);       // v in [0,0.5]
            }
            else
            {
                // lateral face
                u = ((MathF.Atan2(point.y, point.x) + (2f * Constant.PI)) % (2f * Constant.PI)) / (2.0f * Constant.PI);
                u = 0.5f + (u / 2f);
                v = (point.z) / this.height;
            }

            return new Vec2D(u, v);
        }

        /// <summary>
        /// Compute the normal of cone<br/>
        /// The normal is computed for <see cref="Point"> (a point on the surface of the
        /// cone), and it is chosen so that it is always in the opposite
        /// direction with respect to `ray_dir`.
        /// </summary>
        /// <param name="point"><see cref="Point"> on the surface of the cone</param>
        /// <param name="rayDir"><see cref="Vec"> direction of the incoming ray</param>
        /// <returns><see cref="Normal"> object</returns>
        private Normal _coneNormal(Point point, Vec rayDir)
        {
            Normal result;
            // point on the bottom plane
            if (Utility.areClose(point.z, 0f))
            {
                result = new Normal(0f, 0f, -1.0f);
            }
            else
            {
                float rad = MathF.Sqrt(point.x * point.x + point.y * point.y);
                result = new Normal(point.x / rad,
                                    point.y / rad,
                                    this.radius / this.height).Normalize();
            }
            if (point.toVec() * rayDir > 0.0f)
                result = -result;
            return result;
        }

        /// <summary>
        /// Checks whether a Ray intersects a Cone
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override bool quickRayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            Plane planeBottom = new Plane();
            HitRecord? hitBottom = planeBottom.rayIntersection(invRay);

            if (hitBottom.HasValue)
            {
                Point pointInt = hitBottom.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < this.radius * this.radius)
                    return true;
            }

            float dx = invRay.dir.x;
            float dy = invRay.dir.y;
            float dz = invRay.dir.z;

            float ox = invRay.origin.x;
            float oy = invRay.origin.y;
            float oz = invRay.origin.z;

            float hr = MathF.Pow((this.height / this.radius), 2f);

            float a = hr * (MathF.Pow(dx, 2f) + MathF.Pow(dy, 2f)) - MathF.Pow(dz, 2f);
            float b = 2f * (hr * (ox * dx + oy * dy) - dz * oz + this.height * dz);
            float c = hr * (MathF.Pow(ox, 2f) + MathF.Pow(oy, 2f)) + 2f * oz * this.height - MathF.Pow(oz, 2f) - MathF.Pow(this.height, 2f);

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
            {
                return false;
            }

            float sqrtDelta = MathF.Sqrt(delta);
            float tmin;
            float tmax;
            // needed if a ray has the same slope of the cone! (---> a would be 0)
            if (a == 0)
            {
                tmin = -c / b;
                tmax = Single.PositiveInfinity;
            }
            else
            {
                tmin = (-b - sqrtDelta) / (2.0f * a);
                tmax = (-b + sqrtDelta) / (2.0f * a);
            }

            if (tmin > invRay.tmin && tmin < invRay.tmax)
                return true;

            return false;

        }

        /// <summary>
        /// Calculates the closest intersection between a Ray and the cone.
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override HitRecord? rayIntersection(Ray ray)
        {
            Ray invRay = ray.Transform(this.transformation.getInverse());

            Plane planeBottom = new Plane();
            HitRecord? hitBottom = planeBottom.rayIntersection(invRay);
            HitRecord? planeHit = null;

            if (hitBottom.HasValue)
            {
                Point pointInt = hitBottom.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < this.radius * this.radius)
                {
                    planeHit = new HitRecord(
                                wp: this.transformation * pointInt,
                                nm: (this.transformation * this._coneNormal(pointInt, ray.dir)),
                                sp: this._conePointToUV(pointInt),
                                tt: hitBottom.Value.t,
                                r: ray,
                                shape: this);
                }
            }
            HitRecord? coneHit = null;

            float dx = invRay.dir.x;
            float dy = invRay.dir.y;
            float dz = invRay.dir.z;

            float ox = invRay.origin.x;
            float oy = invRay.origin.y;
            float oz = invRay.origin.z;

            float hr = MathF.Pow((this.height / this.radius), 2f);

            float a = hr * (MathF.Pow(dx, 2f) + MathF.Pow(dy, 2f)) - MathF.Pow(dz, 2f);
            float b = 2f * (hr * (ox * dx + oy * dy) - dz * oz + this.height * dz);
            float c = hr * (MathF.Pow(ox, 2f) + MathF.Pow(oy, 2f)) + 2f * oz * this.height - MathF.Pow(oz, 2f) - MathF.Pow(this.height, 2f);

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
            {
                goto NoConeHit;
            }

            float sqrtDelta = MathF.Sqrt(delta);
            float tmin;
            float tmax;
            // needed if a ray has the same slope of the cone! (---> a would be 0)
            if (a == 0)
            {
                tmin = -c / b;
                tmax = Single.PositiveInfinity;
            }
            else
            {
                tmin = (-b - sqrtDelta) / (2.0f * a);
                tmax = (-b + sqrtDelta) / (2.0f * a);
            }

            float firstHitT;



            if (tmin > invRay.tmin && tmin < invRay.tmax)
            {
                firstHitT = tmin;
                Point hitPoint = invRay.at(firstHitT);
                if (hitPoint.z >= 0f && hitPoint.z <= this.height)
                {
                    coneHit = new HitRecord(
                                            this.transformation * hitPoint,
                                            this.transformation * _coneNormal(hitPoint, ray.dir),
                                            _conePointToUV(hitPoint),
                                            firstHitT,
                                            ray,
                                            this);
                }
            }
            else if (tmax > invRay.tmin && tmax < invRay.tmax && coneHit == null)
            {
                firstHitT = tmax;
                Point hitPoint = invRay.at(firstHitT);
                if (hitPoint.z >= 0f && hitPoint.z <= this.height)
                {
                    coneHit = new HitRecord(
                                            this.transformation * hitPoint,
                                            this.transformation * _coneNormal(hitPoint, ray.dir),
                                            _conePointToUV(hitPoint),
                                            firstHitT,
                                            ray,
                                            this);
                }
            }

        NoConeHit:
            if (coneHit == null)
                return planeHit;
            else if (planeHit == null)
                return coneHit;
            else
                return HitRecord.lesserComparison((HitRecord)planeHit, (HitRecord)coneHit);
        }


        /// <summary>
        /// Calculates every intersection between a Ray and the cone
        /// </summary>
        /// <param name="ray"></param>
        /// <returns></returns>
        public override List<HitRecord?> rayIntersectionList(Ray ray)
        {
            List<HitRecord?> hits = new List<HitRecord?>();
            Ray invRay = ray.Transform(this.transformation.getInverse());

            Plane planeBottom = new Plane();
            HitRecord? hitBottom = planeBottom.rayIntersection(invRay);

            if (hitBottom.HasValue)
            {
                Point pointInt = hitBottom.Value.worldPoint;
                if (pointInt.x * pointInt.x + pointInt.y * pointInt.y < this.radius * this.radius)
                    hits.Add(new HitRecord(
                                wp: this.transformation * pointInt,
                                nm: (this.transformation * this._coneNormal(pointInt, ray.dir)),
                                sp: this._conePointToUV(pointInt),
                                tt: hitBottom.Value.t,
                                r: ray,
                                shape: this)
                            );
            }


            float dx = invRay.dir.x;
            float dy = invRay.dir.y;
            float dz = invRay.dir.z;

            float ox = invRay.origin.x;
            float oy = invRay.origin.y;
            float oz = invRay.origin.z;

            float hr = MathF.Pow((this.height / this.radius), 2f);

            float a = hr * (MathF.Pow(dx, 2f) + MathF.Pow(dy, 2f)) - MathF.Pow(dz, 2f);
            float b = 2f * (hr * (ox * dx + oy * dy) - dz * oz + this.height * dz);
            float c = hr * (MathF.Pow(ox, 2f) + MathF.Pow(oy, 2f)) + 2f * oz * this.height - MathF.Pow(oz, 2f) - MathF.Pow(this.height, 2f);

            float delta = b * b - 4.0f * a * c;
            if (delta <= 0.0f)
            {
                return hits;
            }


            float sqrtDelta = MathF.Sqrt(delta);
            float tmin;
            float tmax;
            if (a == 0)
            {
                tmin = -c / b;
                tmax = Single.PositiveInfinity;
            }
            else
            {
                tmin = (-b - sqrtDelta) / (2.0f * a);
                tmax = (-b + sqrtDelta) / (2.0f * a);
            }

            if (tmin > invRay.tmin && tmin < invRay.tmax)
            {
                Point hitPoint = invRay.at(tmin);
                if (hitPoint.z >= 0f && hitPoint.z <= this.height)
                {
                    hits.Add(new HitRecord(
                                            this.transformation * hitPoint,
                                            this.transformation * _coneNormal(hitPoint, ray.dir),
                                            _conePointToUV(hitPoint),
                                            tmin,
                                            ray,
                                            this));
                }
            }
            if (tmax > invRay.tmin && tmax < invRay.tmax)
            {
                Point hitPoint = invRay.at(tmax);
                if (hitPoint.z >= 0f && hitPoint.z <= this.height)
                {
                    hits.Add(new HitRecord(
                                            this.transformation * hitPoint,
                                            this.transformation * _coneNormal(hitPoint, ray.dir),
                                            _conePointToUV(hitPoint),
                                            tmax,
                                            ray,
                                            this));
                }
            }

            if (hits.Count != 0)
            {
                hits.Sort();
            }
            return hits;
        }

        /// <summary>
        /// Checks whether a point is inside the cone
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public override bool isPointInside(Point a)
        {
            Point invPoint = this.transformation.getInverse() * a;
            float x = invPoint.x * this.height / this.radius;
            float y = invPoint.y * this.height / this.radius;
            float z = invPoint.z - this.height;
            if (z * z >= x * x + y * y && this._conePointToUV(invPoint).v < this.height)
                return true;
            else
                return false;
        }
    } // end of Cone

} // end of Shapes
