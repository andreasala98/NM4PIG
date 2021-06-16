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
using System.Collections.Generic;
using System;

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


            Assert.True(intersection.Count == hits.Count);
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

            Assert.True(YesHit != null, "TestHitPlane failed! - Assert 1/3");
            Assert.True(NoHit == null, "TestHitPlane failed! - Assert 2/3");

            HitRecord trueHit = new HitRecord(new Point(0.5f, 0.5f, -0.2f), new Normal(0.0f, 0.0f, 1.0f), new Vec2D(0.5f, 0.5f), 2.7f, intRay);

            Assert.True(trueHit.isClose(YesHit), "TestHitPlane failed! - Assert 3/3");

        }
    }

    public class TestBoxes
    {
        [Fact]
        void TestBoxesIntersectionBasic()
        {
            Box box = new Box();
            Ray ray1 = new Ray(new Point(-5f, 0f, 0f), Constant.VEC_X);
            HitRecord? intersection1 = box.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestBoxesIntersectionBasic failed - assert 1/6");
            // new Vec2D(0.375f, 0.8333334f) for other method UV mapping
            HitRecord hit1 = new HitRecord(
                                            new Point(-1.0f, 0.0f, 0.0f),
                                            new Normal(-1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.125f, 0.5f),
                                            4.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), "TestBoxesIntersectionBasic failed - assert 2/6");

            Ray ray2 = new Ray(new Point(0f, 0f, 10f), -Constant.VEC_Z);
            HitRecord? intersection2 = box.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestBoxesIntersectionBasic failed - assert 3/6");
            // new Vec2D(0.625f, 0.5f) for other method UV mapping
            HitRecord hit2 = new HitRecord(
                                            new Point(0.0f, 0.0f, 1.0f),
                                            new Normal(0.0f, 0.0f, 1.0f),
                                            new Vec2D(0.875f, 0.5f),
                                            9.0f,
                                            ray2
                                        );
            Assert.True(hit2.isClose(intersection2), "TestBoxesIntersectionBasic failed - assert 4/6");

            Ray ray3 = new Ray(new Point(-5f, 0f, 10f), Constant.VEC_X);
            HitRecord? intersection3 = box.rayIntersection(ray3);
            Assert.True(intersection3 == null, "TestBoxesIntersectionBasic failed - assert 5/6");

            // Intersect for t<0
            Ray ray4 = new Ray(new Point(0f, 3f, 0f), Constant.VEC_Y);
            HitRecord? intersection4 = box.rayIntersection(ray4);
            Assert.True(intersection4 == null, "TestBoxesIntersectionBasic failed - assert 6/6");
        }

        [Fact]
        void TestBoxesIntersectionTransformation()
        {
            Box box1 = new Box(transformation: Transformation.Scaling(5.0f));
            Ray ray1 = new Ray(new Point(-10f, 3f, 0f), Constant.VEC_X);
            HitRecord? intersection1 = box1.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestBoxesIntersectionTransformation failed - assert 1");
            HitRecord hit1 = new HitRecord(
                                            new Point(-5f, 3f, 0.0f),
                                            new Normal(-0.2f, 0.0f, 0.0f),
                                            new Vec2D(0.125f, 0.6f),
                                            5.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), intersection1.ToString());

            Ray ray2 = new Ray(new Point(0f, 10f, 10f), -Constant.VEC_Z);
            HitRecord? intersection2 = box1.rayIntersection(ray2);
            Assert.True(intersection2 == null, "TestBoxesIntersectionTransformation failed - assert 3/3");
        }
    }


    public class TestCylinder
    {
        [Fact]
        void TestCylinderIntersectionBasic()
        {
            Cylinder cylinder = new Cylinder();
            Ray ray1 = new Ray(new Point(-5f, 0f, 0f), Constant.VEC_X);
            HitRecord? intersection1 = cylinder.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestCylinderIntersectionBasic failed - assert 1/6");

            HitRecord hit1 = new HitRecord(
                                            new Point(-1.0f, 0.0f, 0.0f),
                                            new Normal(-1f, 0.0f, 0.0f),
                                            new Vec2D(0.5f, 0.25f),
                                            4.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), "TestCylinderIntersectionBasic failed - assert 2/6");

            // Intersect top
            Ray ray2 = new Ray(new Point(0f, 0f, 10f), -Constant.VEC_Z);
            HitRecord? intersection2 = cylinder.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestCylinderIntersectionBasic failed - assert 3/6");
            HitRecord hit2 = new HitRecord(
                                            new Point(0.0f, 0.0f, 0.5f),
                                            new Normal(0.0f, 0.0f, 1.0f),
                                            new Vec2D(0.75f, 0.75f),
                                            9.5f,
                                            ray2
                                        );
            Assert.True(hit2.isClose(intersection2), "TestCylinderIntersectionBasic failed - assert 4/6");

            // fire a ray just up
            Ray ray3 = new Ray(new Point(-5f, 0f, 0.6f), Constant.VEC_X);
            HitRecord? intersection3 = cylinder.rayIntersection(ray3);
            Assert.True(intersection3 == null, "TestCylinderIntersectionBasic failed - assert 5/6");

            // Intersect for t<0
            Ray ray4 = new Ray(new Point(0f, 3f, 0f), Constant.VEC_Y);
            HitRecord? intersection4 = cylinder.rayIntersection(ray4);
            Assert.True(intersection4 == null, "TestBoxesIntersectionBasic failed - assert 6/6");
        }

        [Fact]
        void TestCylindersIntersectionTransformation()
        {

            // Cylinder -2.5 < z < 2.5, radius 5
            Cylinder cylinder1 = new Cylinder(transformation: Transformation.Scaling(5.0f));
            Ray ray1 = new Ray(new Point(-10f, 4.5f, 2.4f), Constant.VEC_X);
            HitRecord? intersection1 = cylinder1.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestCylindersIntersectionTransformation failed - assert 1");
            HitRecord hit1 = new HitRecord(
                                            new Point(-2.179451f, 4.5f, 2.4f),
                                            new Normal(-0.017435608f, 0.036000002f, 0.0f),
                                            new Vec2D(0.32178319f, 0.49f),
                                            7.820549f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), intersection1.ToString());

            Ray ray2 = new Ray(new Point(0f, 5f, 5f), -Constant.VEC_Z);
            HitRecord? intersection2 = cylinder1.rayIntersection(ray2);
            Assert.True(intersection2 == null, "TestCylindersIntersectionTransformation failed - assert 3/3");

            // Cylinder -2.5 < z < 2.5, radius 5
            Ray ray3 = new Ray(new Point(-10f, 0f, 0f), new Vec(1f, 2f, 0f));
            HitRecord? intersection3 = cylinder1.rayIntersection(ray3);
            Assert.True(intersection3 == null, "TestCylindersIntersectionTransformation failed - assert 1");
        }
    } // end of cylinder tests

    public class TestCone
    {
        [Fact]
        public void TestHitCone()
        {
            Cone cone = new Cone(r: 1f, h: 1f);

            Ray ray1 = new Ray(origin: new Point(1.5f, 0f, 0.5f), dir: -Constant.VEC_X);
            HitRecord? intersection1 = cone.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHitCone failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(0.5f, 0f, 0.5f),
                new Normal(MathF.Sqrt(2f) / 2f, 0f, MathF.Sqrt(2f) / 2f),
                new Vec2D(0.5f, 0.5f),
                1f,
                ray1
            );

            Assert.True(hit1.isClose(intersection1), "TestHitCone failed! - Assert 2/5");

            Ray ray2 = new Ray(new Point(0f, 0f, -1f), Constant.VEC_Z);
            HitRecord? intersection2 = cone.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestHitCone failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(0.0f, 0.0f, 0.0f),
                new Normal(0.0f, 0.0f, -1.0f),
                new Vec2D(0.25f, 0.5f),
                1.0f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHitCone failed! - Assert 4/5");

            Assert.True(cone.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHitCone failed! - Assert 5/5 ");

            Cone cone2 = new Cone(transformation: Transformation.Translation(new Vec(0f, 5f, 0f)));
            Ray ray3 = new Ray(origin: new Point(0f, 6.5f, 0.5f), dir: -Constant.VEC_Y);
            HitRecord? intersection3 = cone2.rayIntersection(ray3);
            Assert.True(intersection3 != null, "TestHitCone failed! - Assert 6/5");
            HitRecord hit3 = new HitRecord(
                new Point(0f, 5.5f, 0.5f),
                new Normal(0f, MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f),
                new Vec2D(0.625f, 0.5f),
                1f,
                ray3
            );

            Assert.True(hit3.isClose(intersection3), "TestHitCone failed! - Assert 7/5");

            Ray ray4 = new Ray(origin: new Point(0f, 6.5f, 0.0f), dir: -Constant.VEC_Y);
            HitRecord? intersection4 = cone2.rayIntersection(ray4);
            Assert.True(intersection4 != null, "TestHitCone failed! - Assert 8/5");
            HitRecord hit4 = new HitRecord(
                new Point(0f, 6f, 0f),
                new Normal(0f, 0f, -1f),
                new Vec2D(0.25f, 1f),
                0.5f,
                ray4
            );
            Assert.True(hit4.isClose(intersection4), "TestHitCone failed! - Assert 9/5");

            Vec direzione = (-Constant.VEC_X - Constant.VEC_Z).Normalize();
            Ray ray5 = new Ray(origin: new Point(1.5f, 0f, 1.5f), dir: direzione);
            HitRecord? intersection5 = cone.rayIntersection(ray5);
            Assert.True(intersection5 != null, "TestHitCone failed! - Assert 10/5");
            HitRecord hit5 = new HitRecord(
                new Point(0.5f, 0f, 0.5f),
                new Normal(MathF.Sqrt(2f) / 2f, 0f, MathF.Sqrt(2f) / 2f),
                new Vec2D(0.5f, 0.5f),
                MathF.Sqrt(2f),
                ray5
            );
            Assert.True(hit5.isClose(intersection5), "TestHitCone failed! - Assert 11/5");

            Vec dir = (-0.5f * Constant.VEC_X - MathF.Sqrt(3) / 2f * Constant.VEC_Z).Normalize();
            Ray ray6 = new Ray(origin: new Point(1.0f, 0f, (0.5f + MathF.Sqrt(3) / 2f)), dir: dir);
            HitRecord? intersection6 = cone.rayIntersection(ray6);
            Assert.True(intersection6 != null, "TestHitCone failed! - Assert 12/5");
            HitRecord hit6 = new HitRecord(
                new Point(0.5f, 0f, 0.5f),
                new Normal(MathF.Sqrt(2f) / 2f, 0f, MathF.Sqrt(2f) / 2f),
                new Vec2D(0.5f, 0.5f),
                1f,
                ray6
            );
            Assert.True(hit6.isClose(intersection6), "TestHitCone failed! - Assert 13/5");
        }

        [Fact]
        public void TestInnerHitCone()
        {
            Cone cone = new Cone(r: 1f, h: 1f);

            Ray ray1 = new Ray(origin: new Point(0f, 0f, 0.5f), dir: (Constant.VEC_X));
            HitRecord? intersection1 = cone.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHitCone failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(0.5f, 0.0f, 0.5f),
                new Normal(-MathF.Sqrt(2) / 2, 0f, -MathF.Sqrt(2) / 2),
                new Vec2D(0.5f, 0.5f),
                0.5f,
                ray1
            );

            Assert.True(hit1.isClose(intersection1), "TestHitCone failed! - Assert 2/5");
        }

        [Fact]
        public void TestisPointInside()
        {
            Cone cone = new Cone(r: 0.5f, h: 2);
            Assert.True(cone.isPointInside(new Point(0f, 0f, 1.5f)));
            Assert.True(cone.isPointInside(new Point(0.25f, 0.25f, 0f)));
            Assert.False(cone.isPointInside(new Point(0.5f, 0.5f, 1f)));
        }

        [Fact]
        public void TestConeIntersectionList()
        {
            Cone cone = new Cone(r: 1f, h: 1f);

            Ray ray = new Ray(origin: new Point(1.5f, 0f, 0.5f), dir: -Constant.VEC_X);

            List<HitRecord?> hits = cone.rayIntersectionList(ray);
            List<HitRecord?> expected = new List<HitRecord?>();

            expected.Add(new HitRecord(
                                        new Point(0.5f, 0f, 0.5f),
                                        new Normal(MathF.Sqrt(2f) / 2f, 0f, MathF.Sqrt(2f) / 2f),
                                        new Vec2D(0.5f, 0.5f),
                                        1f,
                                        ray)
                        );
            expected.Add(new HitRecord(
                                        new Point(-0.5f, 0f, 0.5f),
                                        new Normal(MathF.Sqrt(2f) / 2f, 0f, -MathF.Sqrt(2f) / 2f),
                                        new Vec2D(0.75f, 0.5f),
                                        2f,
                                        ray)
                        );

            expected.Sort();

            Assert.True(expected.Count == hits.Count);
            for (int i = 0; i < 2; i++)
            {
                Assert.True(hits[i]?.isClose((HitRecord)expected[i]), $"TestRayIntersectionList {i} failed");
            }

        }
    } // end of cone tests
} // end of namespace