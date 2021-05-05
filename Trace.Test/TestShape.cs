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
                new Point(1.0f, 0.0f, 1.0f),
                new Normal(1.0f, 0.0f, 1.0f),
                new Vec2D(0.0f, 0.5f),
                2.0f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");

            Assert.True(sphere.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHit failed! - Assert 5/5 ");

        }
    }

}

/*class TestSphere(unittest.TestCase):
    def testHit(self):
        sphere = Sphere()

        ray1 = Ray(origin = Point(0, 0, 2), dir = -VEC_Z)
        intersection1 = sphere.ray_intersection(ray1)
        assert intersection1
        assert HitRecord(
            world_point=Point(0.0, 0.0, 1.0),
            normal = Normal(0.0, 0.0, 1.0),
            surface_point = Vec2d(0.0, 0.0),
            t = 1.0,
            ray = ray1,
        ).is_close(intersection1)

        ray2 = Ray(origin = Point(3, 0, 0), dir = -VEC_X)
        intersection2 = sphere.ray_intersection(ray2)
        assert intersection2
        assert HitRecord(
            world_point=Point(1.0, 0.0, 0.0),
            normal = Normal(1.0, 0.0, 0.0),
            surface_point = Vec2d(0.0, 0.5),
            t = 2.0,
            ray = ray2,
        ).is_close(intersection2)

        assert not sphere.ray_intersection(Ray(origin=Point(0, 10, 2), dir = -VEC_Z))

    def testInnerHit(self):
        sphere = Sphere()

        ray = Ray(origin = Point(0, 0, 0), dir = VEC_X)
        intersection = sphere.ray_intersection(ray)
        assert intersection
        assert HitRecord(
            world_point=Point(1.0, 0.0, 0.0),
            normal = Normal(-1.0, 0.0, 0.0),
            surface_point = Vec2d(0.0, 0.5),
            t = 1.0,
            ray = ray,
        ).is_close(intersection)

    def testTransformation(self):
        sphere = Sphere(transformation = translation(Vec(10.0, 0.0, 0.0)))

        ray1 = Ray(origin = Point(10, 0, 2), dir = -VEC_Z)
        intersection1 = sphere.ray_intersection(ray1)
        assert intersection1
        assert HitRecord(
            world_point=Point(10.0, 0.0, 1.0),
            normal = Normal(0.0, 0.0, 1.0),
            surface_point = Vec2d(0.0, 0.0),
            t = 1.0,
            ray = ray1,
        ).is_close(intersection1)

        ray2 = Ray(origin = Point(13, 0, 0), dir = -VEC_X)
        intersection2 = sphere.ray_intersection(ray2)
        assert intersection2
        assert HitRecord(
            world_point=Point(11.0, 0.0, 0.0),
            normal = Normal(1.0, 0.0, 0.0),
            surface_point = Vec2d(0.0, 0.5),
            t = 2.0,
            ray = ray2,
        ).is_close(intersection2)

        # Check if the sphere failed to move by trying to hit the untransformed shape
assert not sphere.ray_intersection(Ray(origin=Point(0, 0, 2), dir = -VEC_Z))

        # Check if the *inverse* transformation was wrongly applied
        assert not sphere.ray_intersection(Ray(origin=Point(-10, 0, 0), dir = -VEC_Z))
*/