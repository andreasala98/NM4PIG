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

namespace Trace.Test
{
    public class TestSphere
    {

        [Fact]
        public void TestHit()
        {
            Sphere sphere = new Sphere();

            Ray ray1 = new Ray(origin: new Point(0f, 0f, 2f), dir: -Constant.VEC_Z);
            HitRecord? intersection1 = sphere.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(0.0f, 0.0f, 1.0f),
                new Normal(0.0f, 0.0f, 1.0f),
                new Vec2D(0.0f, 0.0f),
                1.0f,
                ray1
            );
            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/5");

            Ray ray2 = new Ray(new Point(3f, 0f, 0f), -Constant.VEC_X);
            HitRecord? intersection2 = sphere.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestHit failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(1.0f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                2.0f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");

            Assert.True(sphere.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHit failed! - Assert 5/5 ");
        }

        [Fact]
        public void TestInnerHit()
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

    }

    public class TestCSGUnion
    {
        [Fact]
        public void TestUnion()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));

            CSGUnion u1 = new CSGUnion(s1, s2);
            Ray ray1 = new Ray(origin: new Point(0.5f, 0f, 2.0f), dir: -Constant.VEC_Z);
            HitRecord? intersection1 = u1.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(0.5f, 0.0f, 1.0f),
                new Normal(0.0f, 0.0f, 1.0f),
                new Vec2D(0.0f, 0.0f),
                1.0f,
                ray1
            );
            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/5");

            Ray ray2 = new Ray(new Point(3f, 0f, 0f), -Constant.VEC_X);
            HitRecord? intersection2 = u1.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestHit failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(1.5f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                1.5f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");

            Assert.True(u1.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHit failed! - Assert 5/5 ");
        }
    }
} // end of namespace