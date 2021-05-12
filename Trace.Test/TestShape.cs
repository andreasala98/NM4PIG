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
using System.Collections.Generic;


namespace Trace.Test
{

    /// <summary>
    /// Class to collect tests for the <see cref="Sphere"/> class
    /// </summary>
    public class TestSphere
    {

        [Fact]
        public void TestHitSphere()
        {
            Sphere sphere = new Sphere();

            Ray ray1 = new Ray(origin: new Point(0f, 0f, 2f), dir: -Constant.VEC_Z);
            HitRecord? intersection1 = sphere.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHitSphere failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(0.0f, 0.0f, 1.0f),
                new Normal(0.0f, 0.0f, 1.0f),
                new Vec2D(0.0f, 0.0f),
                1.0f,
                ray1
            );
            Assert.True(hit1.isClose(intersection1), "TestHitSphere failed! - Assert 2/5");

            Ray ray2 = new Ray(new Point(3f, 0f, 0f), -Constant.VEC_X);
            HitRecord? intersection2 = sphere.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestHitSphere failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(1.0f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                2.0f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHitSphere failed! - Assert 4/5");

            Assert.True(sphere.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHitSphere failed! - Assert 5/5 ");
        }

        [Fact]
        public void TestInnerHitSphere()
        {
            Sphere sphere = new Sphere();
            Ray ray = new Ray(origin: new Point(0f, 0f, 0f), dir: Constant.VEC_X);
            HitRecord? intersection = sphere.rayIntersection(ray);
            Assert.True(intersection != null, "TestInnerHit failed! - Assert 1/2");

            HitRecord hit = new HitRecord(
                                            new Point(1.0f, 0.0f, 0.0f),
                                            new Normal(-1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.0f, 0.5f),
                                            1.0f,
                                            ray
                                        );
            Assert.True(hit.isClose(intersection), "TestInnerHit failed! - Assert 2/2");
        }

        [Fact]
        public void TestTransformation()
        {
            Sphere sphere = new Sphere(Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));

            Ray ray1 = new Ray(new Point(10f, 0f, 2f), -Constant.VEC_Z);
            HitRecord? intersection1 = sphere.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestTransformation failed - assert 1/6");
            HitRecord hit1 = new HitRecord(
                                            new Point(10.0f, 0.0f, 1.0f),
                                            new Normal(0.0f, 0.0f, 1.0f),
                                            new Vec2D(0.0f, 0.0f),
                                            1.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), "TestTransformation failed - assert 2/6");

            Ray ray2 = new Ray(new Point(13f, 0f, 0f), -Constant.VEC_X);
            HitRecord? intersection2 = sphere.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestTransformation failed - assert 3/6");
            HitRecord hit2 = new HitRecord(
                                            new Point(11.0f, 0.0f, 0.0f),
                                            new Normal(1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.0f, 0.5f),
                                            2.0f,
                                            ray2
                                        );
            Assert.True(hit2.isClose(intersection2), "TestTransformation failed - assert 4/6");

            // Check if the sphere failed to move by trying to hit the untransformed shape
            Assert.True(sphere.rayIntersection(new Ray(new Point(0f, 0f, 2f), -Constant.VEC_Z)) == null, "TestTransformation failed - assert 5/6");

            // Check if the *inverse* transformation was wrongly applied
            Assert.True(sphere.rayIntersection(new Ray(new Point(-10f, 0f, 0f), -Constant.VEC_Z)) == null, "TestTransformation failed - assert 6/6");
        }

        [Fact]
        public void TestisPointInside()
        {
            Point a = new Point(0.5f, 0.5f, 0.5f);
            Sphere s = new Sphere();
            Assert.True(s.isPointInside(a), "TestisPointInside failed - assert 1/3");

            Sphere s1 = new Sphere(Transformation.Translation(new Vec(10.0f, 0.0f, 0.0f)));
            Assert.False(s1.isPointInside(a), "TestisPointInside failed - assert 2/3");

            Point a1 = new Point(10.5f, 0.5f, 0.5f);
            Assert.True(s1.isPointInside(a1), "TestisPointInside failed - assert 3/3");
        }

        [Fact]
        public void TestrayIntersectionList()
        {
            Sphere s = new Sphere();
            Ray r = new Ray(origin: new Point(0f, 0f, 2f), dir: -Constant.VEC_Z);

            List<HitRecord?> intersection = s.rayIntersectionList(r);
            List<HitRecord> hits = new List<HitRecord>();
            hits.Add(new HitRecord(
                                    new Point(0.0f, 0.0f, 1.0f),
                                    new Normal(0.0f, 0.0f, 1.0f),
                                    new Vec2D(0.0f, 0.0f),
                                    1.0f,
                                    r)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.0f, 0.0f, -1.0f),
                                    new Normal(0.0f, 0.0f, 1.0f),
                                    new Vec2D(0.0f, 1.0f),
                                    3.0f,
                                    r)
                                   );


            for (int i = 0; i < 2; i++)
            {
                Assert.True(hits[i].isClose((HitRecord)intersection[i]), "TestRayIntersectionList failed");
            }

        }

    }

    /// <summary>
    /// Class to collect tests for the <see cref="Plane"/> class
    /// </summary>

    public class TestPlane
    {

        [Fact]
        public void TestHitPlane()
        {
            Plane plane = new Plane(Transformation.Translation(new Vec(0.0f, 0.0f, -0.2f)));
            Ray intRay = new Ray(origin: new Point(0.5f, 0.5f, 2.5f), dir: -Constant.VEC_Z);
            Ray notIntRay = new Ray(origin: new Point(0.0f, 0.0f, 2.5f), dir: Constant.VEC_Z);

            HitRecord? YesHit = plane.rayIntersection(intRay);
            HitRecord? NoHit = plane.rayIntersection(notIntRay);

            Assert.True(YesHit != null, "TestHitPlane failed! - Assert 1");
            Assert.True(NoHit == null, "TestHitPlane failed! - Assert 2");

            HitRecord trueHit = new HitRecord(new Point(0.5f, 0.5f, -0.2f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.5f, 0.5f), 2.7f, intRay);

            Assert.True(trueHit.isClose(YesHit), "TestHitPlane failed! - Assert 3");

            //  new HitRecord(new Point(), new Normal(0f,0f,1.0f), new Vec2D(0f,0f), 2.5f, intRay));

        }
    }

    public class TestBoxes
    {
        [Fact]
        void TestBoxesIntersection()
        {
            Box box = new Box();
            Ray ray1 = new Ray(new Point(-5f, 0f, 0f), Constant.VEC_X);
            HitRecord? intersection1 = box.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestBoxes failed - assert 1/6");
            Console.WriteLine();
            HitRecord hit1 = new HitRecord(
                                            new Point(-1.0f, 0.0f, 0.0f),
                                            new Normal(-1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.5f, 0.5f),
                                            4.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), "TestBoxes failed - assert 2/6");

            Ray ray2 = new Ray(new Point(0.8f, 0f, 4f), -Constant.VEC_Z);
            HitRecord? intersection2 = box.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestTransformation failed - assert 3/6");
            HitRecord hit2 = new HitRecord(
                                            new Point(11.0f, 0.0f, 0.0f),
                                            new Normal(1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.5f, 0.5f),
                                            2.0f,
                                            ray2
                                        );
            Assert.True(hit2.isClose(intersection2), "TestTransformation failed - assert 4/6");

        }
    }

} // end of namespace