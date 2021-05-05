using System;

namespace Trace
{
    public abstract class Shape
    {
        public Transformation transformation;

        public Shape(Transformation? transformation = null)
        {
            this.transformation = transformation ?? new Transformation(1);
        }

        public abstract HitRecord? RayIntersection(Ray ray)

    }

    public class Sphere : Shape
    {

        public Sphere(Transformation? transformation = null) : base(transformation) { }
    }
}