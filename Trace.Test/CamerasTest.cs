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

using Xunit;
using System;

namespace Trace.Test
{

    public class RayTest
    {
        Ray ray1 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray2 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray3 = new Ray(new Point(5.0f, 1.0f, 4.0f), new Vec(3.0f, 9.0f, 4.0f));
        Ray ray4 = new Ray(new Point(1.0f, 2.0f, 4.0f), new Vec(4.0f, 2.0f, 1.0f));

        [Fact]
        public void TestIsRayClose()
        {
            Assert.True(ray1.isClose(ray2), "TestIsRayClose failed - Assert 1/2");
            Assert.False(ray1.isClose(ray3), "TestIsRayClose failed - Assert 2/2");
        }

        [Fact]
        public void TestAt()
        {
            Assert.True(ray4.at(0.0f).isClose(ray4.origin), "TestAt failed - Assert 1/3");
            Assert.True(ray4.at(1.0f).isClose(new Point(5.0f, 4.0f, 5.0f)), "TestAt failed - Assert 2/3");
            Assert.True(ray4.at(2.0f).isClose(new Point(9.0f, 6.0f, 6.0f)), "TestAt failed - Assert 3/3");
        }

        [Fact]
        public void TestTransform()
        {
            Ray ray5 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(6.0f, 5.0f, 4.0f));
            Transformation T = Transformation.Translation(new Vec(10.0f, 11.0f, 12.0f)) * Transformation.RotationX((float)System.Math.PI / 2.0f);

            Ray transf = ray5.Transform(T);
            Assert.True(transf.origin.isClose(new Point(11.0f, 8.0f, 14.0f)), "TestTransform failed - Assert 1/2");
            Assert.True(transf.dir.isClose(new Vec(6.0f, -4.0f, 5.0f)), "TestTransform failed - Assert 2/2");
        }

    }

    public class CameraTest
    {

        [Fact]
        public void testOrthogonalCamera()
        {
            OrthogonalCamera cam = new OrthogonalCamera(aspectRatio: 2.0f);

            Ray ray1 = cam.fireRay(0.0f, 0.0f);
            Ray ray2 = cam.fireRay(1.0f, 0.0f);
            Ray ray3 = cam.fireRay(0.0f, 1.0f);
            Ray ray4 = cam.fireRay(1.0f, 1.0f);

            Assert.True(Utility.areClose(0.0f, ray1.dir.crossProd(ray2.dir).getSquaredNorm()), "testOrthogonalCamera failed - assert 1/7");
            Assert.True(Utility.areClose(0.0f, ray1.dir.crossProd(ray3.dir).getSquaredNorm()), "testOrthogonalCamera failed - assert 2/7");
            Assert.True(Utility.areClose(0.0f, ray1.dir.crossProd(ray4.dir).getSquaredNorm()), "testOrthogonalCamera failed - assert 3/7");

            Assert.True(ray1.at(1.0f).isClose(new Point(0.0f, 2.0f, -1.0f)), "testOrthogonalCamera failed - assert 4/7");
            Assert.True(ray2.at(1.0f).isClose(new Point(0.0f, -2.0f, -1.0f)), "testOrthogonalCamera failed - assert 5/7");
            Assert.True(ray3.at(1.0f).isClose(new Point(0.0f, 2.0f, 1.0f)), "testOrthogonalCamera failed - assert 6/7");
            Assert.True(ray4.at(1.0f).isClose(new Point(0.0f, -2.0f, 1.0f)), "testOrthogonalCamera failed - assert 7/7");
        }

        [Fact]
        void TestOrthogonalCameraTransform()
        {
            OrthogonalCamera cam = new OrthogonalCamera(transformation: Transformation.Translation(-2.0f * Constant.VEC_Y) * Transformation.RotationZ((float)Math.PI / 2.0f));
            Ray ray = cam.fireRay(0.5f, 0.5f);
            Assert.True(ray.at(1.0f).isClose(new Point(0.0f, -2.0f, 0.0f)), "testOrthogonalCameraTransform failed - Assert 1/1");
        }

        [Fact]
        public void TestPerspectiveCamera()
        {
            PerspectiveCamera cam = new PerspectiveCamera(screenDistance: 1.0f, aspectRatio: 2.0f);

            Ray ray1 = cam.fireRay(0.0f, 0.0f);
            Ray ray2 = cam.fireRay(1.0f, 0.0f);
            Ray ray3 = cam.fireRay(0.0f, 1.0f);
            Ray ray4 = cam.fireRay(1.0f, 1.0f);

            Assert.True(ray1.origin.isClose(ray2.origin), "testPerspectiveCamera failed - assert 1/7");
            Assert.True(ray1.origin.isClose(ray3.origin), "testPerspectiveCamera failed - assert 2/7");
            Assert.True(ray1.origin.isClose(ray4.origin), "testPerspectiveCamera failed - assert 3/7");

            Assert.True(ray1.at(1.0f).isClose(new Point(0.0f, 2.0f, -1.0f)), "testPerspectiveCamera failed - assert 4/7");
            Assert.True(ray2.at(1.0f).isClose(new Point(0.0f, -2.0f, -1.0f)), "testPerspectiveCamera failed - assert 5/7");
            Assert.True(ray3.at(1.0f).isClose(new Point(0.0f, 2.0f, 1.0f)), "testPerspectiveCamera failed - assert 6/7");
            Assert.True(ray4.at(1.0f).isClose(new Point(0.0f, -2.0f, 1.0f)), "testPerspectiveCamera failed - assert 7/7");
        }

        [Fact]
        void TestPerspectiveCameraTransform()
        {
            PerspectiveCamera cam = new PerspectiveCamera(transformation: Transformation.Translation(-2.0f * Constant.VEC_Y) * Transformation.RotationZ((float)Math.PI / 2.0f));
            Ray ray = cam.fireRay(0.5f, 0.5f);
            Assert.True(ray.at(1.0f).isClose(new Point(0.0f, -2.0f, 0.0f)), "testPerspectiveCameraTransform failed - Assert 1/1");
        }
    }


    public class ImageTracerTest
    {
        [Fact]
        public void TestOrientation()
        {
            HdrImage image = new HdrImage(4, 2);
            PerspectiveCamera camera = new PerspectiveCamera(aspectRatio: 2.0f);
            ImageTracer tracer = new ImageTracer(image, camera);
            Ray topLeftRay = tracer.fireRay(0, 0, 0.0f, 0.0f);
            Point p = new Point(0.0f, 2.0f, 1.0f);
            Assert.True(p.isClose(topLeftRay.at(1.0f)));
        }

        [Fact]
        public void TestUVSubMapping()
        {
            HdrImage image = new HdrImage(4, 2);
            PerspectiveCamera camera = new PerspectiveCamera(aspectRatio: 2.0f);
            ImageTracer tracer = new ImageTracer(image, camera);

            Ray ray1 = tracer.fireRay(0, 0, 2.5f, 1.5f);
            Ray ray2 = tracer.fireRay(2, 1, 0.5f, 0.5f);
            Assert.True(ray1.isClose(ray2), "TestUVSubMapping failed - Assert 1/1");
        }

        public Color lambda(Ray r)
            => new Color(1.0f, 2.0f, 3.0f);

        [Fact]
        public void TestImageCoverage()
        {
            HdrImage image = new HdrImage(4, 2);
            PerspectiveCamera camera = new PerspectiveCamera();
            ImageTracer tracer = new ImageTracer(image, camera);

            tracer.fireAllRays(lambda);
            for (int row = 0; row < image.height; row++)
            {
                for (int col = 0; col < image.width; col++)
                {
                    Assert.True(image.getPixel(col, row).isClose(new Color(1.0f, 2.0f, 3.0f)), $"TestImageCoverage failed - Assert row={row}, col={col}");
                }
            }
        }

    }

}
