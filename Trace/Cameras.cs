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
    /// An efficient type representing a ray to be fired.
    /// </summary>
    public struct Ray
    {
        /// <summary>
        /// The origin point, where the observer lies.
        /// </summary>
        public Point origin;
        /// <summary>
        /// The vector orthogonally connecting the observer to the center of the screen 
        /// </summary>
        public Vec dir;
        /// <summary>
        /// Minimum travelling distance of the ray. <br/>
        /// Default setting: 1e-5
        /// </summary>
        public float tmin;
        /// <summary>
        /// Minimum travelling distance of the ray. <br/>
        /// Default setting: Infinity
        /// 
        /// </summary>
        public float tmax;
        /// <summary>
        /// Number of reflections allowed <br/>
        /// Default setting: 0
        /// </summary>

        public int depth;
        /// <summary>
        ///  Default constructor for Ray.
        /// </summary>
        /// <param name="or"> Origin point (observer) </param>
        /// <param name="d"> Vector direction </param>
        /// <param name="tm"> Minimum distance </param>
        /// <param name="tM"> Maximum distance </param>
        /// <param name="dep"> Number of reflections </param>
        public Ray(Point or, Vec d, float? tm = 1e-5f, float? tM = System.Single.PositiveInfinity, int? dep = 0)
        {
            this.origin = or;
            this.dir = d;
            this.tmin = (float)tm;
            this.tmax = (float)tM;
            this.depth = (int)dep;
        }

        /// <summary>
        /// Calculate ray position at origin + dir*t.
        /// </summary>
        /// <param name="t"> Running paramter between tmin and tmax.</param>
        /// <returns> A <see cref="Point"/> object </returns>
        public Point at(float t)
            => this.origin + (this.dir * t);

        /// <summary>
        ///  Boolean to check if two rays are equal.
        /// </summary>
        /// <param name="r"> The other ray.</param>
        /// <returns> True if rays are close enough.</returns>
        public bool isClose(Ray r)
        {
            return this.origin.isClose(r.origin) && this.dir.isClose(r.dir);
        }
        /// <summary>
        /// Apply affine transformation to the ray.
        /// </summary>
        /// <param name="T"> A <see cref="Transformation"/> object. </param>
        /// <returns> The transformed ray.</returns>
        public Ray transform(Transformation T)
            => new Ray(T * this.origin, T * this.dir, this.tmin, this.tmax, this.depth);


    }
    
    /// <summary>
    /// An abstract class representing an observer <br/>
    /// Concrete subclasses are `OrthogonalCamera` and `PerspectiveCamera`.
    /// </summary>
    public abstract class Camera
    {
        public float aspectRatio;
        public Transformation transformation;

        public Camera(float? aspectRatio = null, Transformation? transformation = null)
        {

            this.aspectRatio = aspectRatio ?? 1.0f;
            this.transformation = transformation ?? new Transformation(1);
        }

        /// <summary>
        /// Fire a ray through the camera.<br/>
        /// This is an abstract method. It has been redefined in derived classes.
        /// Fire a ray that goes through the screen at the position (u, v). The exact meaning
        /// of these coordinates depends on the projection used by the camera.
        /// </summary>
        public abstract Ray fireRay(float u, float v);
    }

    /// <summary>
    /// A camera implementing an orthogonal 3D → 2D projection
    /// This class implements an observer seeing the world through an orthogonal projection.
    /// </summary>
    public class OrthogonalCamera : Camera
    {
        /// <summary>
        /// Create a new orthogonal camera
        /// </summary>
        /// <param name="aspRat">It is the ratio between the width and the height of the screen. For fullscreen
        /// images, you should probably set `aspectRatio` to 16/9, as this is the most used aspect ratio
        /// used in modern monitors.</param>
        /// <param name="transf">It is an instance of the struct <see cref="Transformation"/>.</param>
        public OrthogonalCamera(float? aspectRatio = null, Transformation? transformation = null) : base(aspectRatio, transformation) { }

        /// <summary>
        /// Shoot a ray through the camera's screen <br/>
        /// The coordinates(u, v) specify the point on the screen where the ray crosses it. Coordinates(0f, 0f) represent
        /// the bottom-left corner, (0f, 1f) the top-left corner, (1f, 0f) the bottom-right corner, and (1f, 1f) the top-right
        /// corner
        /// </summary>
        /// <param name="u">questo è u</param>
        /// <param name="v"></param>
        /// <returns>A <see cref="Trace.Ray"/> object.</returns>
        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-1.0f, (1.0f - 2f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            Vec direction = new Vec(1.0f, 0.0f, 0.0f);
            return new Ray(origin, direction, 1.0f).transform(this.transformation);
        }
    }

    public class PerspectiveCamera : Camera
    {
        public float screenDistance;
        public PerspectiveCamera(float? screenDistance = null, float? aspectRatio = null, Transformation? transformation = null) : base(aspectRatio, transformation)
        {
            this.screenDistance = screenDistance ?? 1.0f;
        }

        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-this.screenDistance, 0.0f, 0.0f);
            Vec direction = new Vec(this.screenDistance, (1.0f - 2.0f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            return new Ray(origin, direction, 1.0f).transform(this.transformation);
        }

        public float apertureDeg()
            => 2.0f * (float)Math.Atan(this.screenDistance / this.aspectRatio) * 180.0f / (float)Math.PI;
    }

        class ImageTracer
    {
        public HdrImage image;
        public Camera camera;

        public ImageTracer(HdrImage i, Camera c)
        {
            image  = i;
            camera = c;
        }

        public Ray fireRay(int col, int row, float uPixel = 0.5, float vPixel = 0.5)
        {
            u = (col + uPixel) / (image.width  - 1);
            v = (row + vPixel) / (image.height - 1);
            return camera.fireRay(u, v);
        }

        public delegate Color myFunction(Ray r);
        public void fireAllRay(Func<Ray, Color> myFunction)
        {
            for(int r = 1; r <= image.height; r++)
            {
                for(int c = 1; r <= image.width; c++)
                {
                    Ray raggio = this.fireRay(c, r);
                    Color colore = Func(raggio);
                    this.image.setPixel(r, c, colore);
                }
            }
            
        }
    }
}