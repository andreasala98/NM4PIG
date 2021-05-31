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
        /// An abstract class representing an observer <br/>
        /// Concrete subclasses are <see cref="OrthogonalCamera"/> and <see cref="OrthogonalCamera"/>.
        /// </summary>
    public abstract class Camera
    {
        /// <summary>
        /// This parameter defines how larger than the height is the image. For fullscreen
        /// images, you should probably set `aspect_ratio` to 16/9, as this is the most used aspect ratio
        /// used in modern monitors.
        /// </summary>
        public float aspectRatio;

        /// <summary>
        /// It is an instance of the struct <see cref="Transformation"/>.
        /// </summary>
        public Transformation transformation;

        /// <summary>
        /// Create a new camera. This is a constructor for an abstract class and so it cannot be used.
        /// Use instead constructors for <see cref="OrthogonalCamera"/> and <see cref="PerspectiveCamera"/>.
        /// </summary>
        public Camera(float? aspectRatio = null, Transformation? transformation = null)
        {
            this.aspectRatio = aspectRatio ?? 1.0f;
            this.transformation = transformation ?? new Transformation(1);
        }

        /// <summary>
        /// This is an abstract method. It has been redefined in derived classes.
        /// Fire a <see cref="Ray"/> that goes through the screen at the position (u, v). The exact meaning
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
        /// <param name="aspectRatio">It is the ratio between the width and the height of the screen. For fullscreen
        /// images, you should probably set `aspectRatio` to 16/9, as this is the most used aspect ratio
        /// used in modern monitors.</param>
        /// <param name="transformation">It is an instance of the struct <see cref="Transformation"/>.</param>
        public OrthogonalCamera(float? aspectRatio = null, Transformation? transformation = null) : base(aspectRatio, transformation) { }

        /// <summary>
        /// Shoot a ray through the camera's screen <br/>
        /// The coordinates(u, v) specify the point on the screen where the ray crosses it. Coordinates(0f, 0f) represent
        /// the bottom-left corner, (0f, 1f) the top-left corner, (1f, 0f) the bottom-right corner, and (1f, 1f) the top-right
        /// corner
        /// </summary>
        /// <param name="u">x coordinate of the screen. 0.0f represent left edge, 1.0f represent right edge.</param>
        /// <param name="v">y coordinate of the screen. 0.0f represent bottom edge, 1.0f represent upper edge.</param>
        /// <returns>A <see cref="Trace.Ray"/> object.</returns>
        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-1.0f, (1.0f - 2f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            Vec direction = new Vec(1.0f, 0.0f, 0.0f);
            return new Ray(origin, direction, 1.0f).Transform(this.transformation);
        }
    }

    /// <summary>
    /// A camera implementing a perspective 3D → 2D projection<br/>
    /// This class implements an observer seeing the world through a perspective projection.
    /// </summary>
    public class PerspectiveCamera : Camera
    {
        /// <summary>
        /// It tells how much far from the eye of the observer is the screen, 
        /// and it influences the so-called «aperture» 
        /// (the field-of-view angle along the horizontal direction).
        /// </summary>
        public float screenDistance;

        /// <summary>
        /// Create a new <see cref="PerspectiveCamera"/>.
        /// </summary>
        /// <param name="screenDistance">It tells how much far from the eye of the observer is the screen, and it influences the so-called «aperture» (the field-of-view angle along the horizontal direction).</param>
        /// <param name="aspectRatio">The parameter `aspect_ratio` defines how larger than the height is the image.For fullscreen
        /// images, you should probably set `aspect_ratio` to 16/9, as this is the most used aspect ratio
        /// used in modern monitors.</param>
        /// <param name="transformation">It is an instance of the struct <see cref="Transformation"/>.</param>
        public PerspectiveCamera(float? screenDistance = null, float? aspectRatio = null, Transformation? transformation = null) : base(aspectRatio, transformation) { this.screenDistance = screenDistance ?? 1.0f; }

        /// <summary>
        /// Shoot a <see cref="Ray"/> through the camera's screen <br/>
        /// The coordinates(u, v) specify the point on the screen where the ray crosses it.Coordinates(0, 0) represent
        /// the bottom-left corner, (0, 1) the top-left corner, (1, 0) the bottom-right corner, and (1, 1) the top-right
        /// corner, as in the following diagram::
        /// </summary>
        /// <param name="u">x coordinate of the screen. 0.0f represent left edge, 1.0f represent right edge.</param>
        /// <param name="v">y coordinate of the screen. 0.0f represent bottom edge, 1.0f represent upper edge.</param>
        /// <returns>A <see cref="Trace.Ray"/> object.</returns>
        public override Ray fireRay(float u, float v)
        {
            Point origin = new Point(-this.screenDistance, 0.0f, 0.0f);
            Vec direction = new Vec(this.screenDistance, (1.0f - 2.0f * u) * this.aspectRatio, 2.0f * v - 1.0f);
            return new Ray(origin, direction, 1.0f).Transform(this.transformation);
        }

        /// <summary>
        /// Compute the aperture of the camera in degrees
        /// The aperture is the angle of the field-of-view along the horizontal direction (Y axis)
        /// </summary>
        /// <returns> The aperture angle, measured in degrees</returns>
        public float apertureDeg()
            => 2.0f * (float)Math.Atan(this.screenDistance / this.aspectRatio) * 180.0f / Constant.PI;
    }

    /// <summary>
    /// A class that links a <see cref="Camera"/> object to a <see cref="HdrImage"/> object to create
    /// a matrix of pixel after solving the rendering equation. <br/>
    /// Public data members: <see cref="HdrImage"/>, <see cref="Camera"/>.
    /// </summary>
    public class ImageTracer
    {
        public HdrImage image;
        /// <summary>
        /// A <see cref="Camera"/> object (=observer) that can be either Orthogonal or Perspective.
        /// </summary>
        public Camera camera;

        /// <summary>
        /// Basic constructor for the class.
        /// </summary>
        /// <param name="i"><see cref="HdrImage"/> input parameter</param>
        /// <param name="c"><see cref="Camera"/> input parameter</param>
        public ImageTracer(HdrImage i, Camera c)
        {
            this.image = i;
            this.camera = c;
        }
        /// <summary>
        /// Method that generates a <see cref="Ray"/> object on the virtual screen
        /// at the coordinates (col + uPixel),(row+ vPixel), both normalized
        /// to the <see cref="HdrImage"/> scale.
        /// </summary>
        /// <param name="col"> Column number (start fom 0)</param>
        /// <param name="row"> Row number (staet from 0) </param>
        /// <param name="uPixel"> x coordinate inside the pixel (default: 0.5) </param>
        /// <param name="vPixel"> y coordinate inside the pixel (default: 0.5) </param>
        /// <returns> The fired <see cref="Ray"/>. </returns>
        public Ray fireRay(int col, int row, float uPixel = 0.5f, float vPixel = 0.5f)
        {
            float u = (col + uPixel) / (this.image.width);
            float v = 1.0f - (row + vPixel) / (this.image.height);
            return this.camera.fireRay(u, v);
        }

        public delegate Color PFunction(Ray r);


        /// <summary>
        /// Method that calculates all pixels of the <see cref="HdrImage"/>
        /// datamember according to the rendering equation,
        /// specified by a <see cref="myFunction"/> object.
        /// </summary>
        /// <param name="Func"> The function that transforms a <see cref="Ray"/> into a <see cref="Color"/>.</param>
        public void fireAllRays(Render rend)
        {
            for (int i = 0; i < image.height; i++)
            {
                for (int j = 0; j < image.width; j++)
                {
                    Ray raggio = this.fireRay(j, i);
                    Color colore = rend.computeRadiance(raggio);
                    this.image.setPixel(j, i, colore);
                    if (i%50==0 && i!=0)
                    Console.Write(((float)(100*i)/image.height).ToString("0.00") + "% of rendering completed\r");
                }
            }

        }

        public void fireAllRays(PFunction lf)
        {
            for (int i = 0; i < image.height; i++)
            {
                for (int j = 0; j < image.width; j++)
                {
                    Ray raggio = this.fireRay(j, i);
                    Color colore = lf(raggio);
                    this.image.setPixel(j, i, colore);
                }

                if (i%50==0 && i!=0)
                Console.Write(((float)i/image.height).ToString("0.00") + " of rendering completed\r");

            }

        }
    }
}