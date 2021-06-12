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
    public class TestCSGUnion
    {
        [Fact]
        public void TestrayIntersection()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.0f, 1.7f, 0.0f)));

            CSGUnion u1 = new CSGUnion(s1, s2);
            Ray ray1 = new Ray(origin: new Point(3f, 0f, 0f), dir: -Constant.VEC_X);
            HitRecord? intersection1 = u1.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(1f, 0.0f, 0.0f),
                Constant.VEC_X_N,
                new Vec2D(0.0f, 0.5f),
                3.0f,
                ray1
            );
            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/5");

            Ray ray2 = new Ray(new Point(6f, 1.7f, 0f), -Constant.VEC_X);
            HitRecord? intersection2 = u1.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestHit failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(1.0f, 1.7f, 0.0f),
                Constant.VEC_X_N,
                new Vec2D(0.0f, 0.5f),
                6.0f,
                ray2
            );
            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");

            Assert.True(u1.rayIntersection(new Ray(new Point(0f, 10f, 2f), -Constant.VEC_Z)) == null, "TestHit failed! - Assert 5/5 ");
        }

        [Fact]
        public void TestrayIntersctionInner()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));

            CSGUnion u1 = new CSGUnion(s1, s2);
            Ray ray1 = new Ray(origin: new Point(0.5f, 0f, 0f), dir: Constant.VEC_X);

            HitRecord? intersection1 = u1.rayIntersection(ray1);
            Assert.True(intersection1 != null, "TestInnerHit failed! - Assert 1/4");
            HitRecord hit1 = new HitRecord(
                                            new Point(1.5f, 0.0f, 0.0f),
                                            new Normal(-1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.0f, 0.5f),
                                            1.0f,
                                            ray1
                                        );
            Assert.True(hit1.isClose(intersection1), "TestInnerHit failed! - Assert 2/4");

            // now from the other side
            Ray ray2 = new Ray(origin: new Point(0.5f, 0f, 0f), dir: -Constant.VEC_X);

            HitRecord? intersection2 = u1.rayIntersection(ray2);
            Assert.True(intersection2 != null, "TestInnerHit failed! - Assert 3/4");
            HitRecord hit2 = new HitRecord(
                                            new Point(-1.0f, 0.0f, 0.0f),
                                            new Normal(1.0f, 0.0f, 0.0f),
                                            new Vec2D(0.5f, 0.5f),
                                            1.5f,
                                            ray2
                                        );
            Assert.True(hit2.isClose(intersection2), "TestInnerHit failed! - Assert 4/4");

        }

        [Fact]
        public void TestrayIntersectionList()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.0f, 0.5f, 0.0f)));
            CSGUnion u1 = new CSGUnion(s1, s2);

            Ray r = new Ray(origin: new Point(0.0f, 2.0f, 0f), dir: -Constant.VEC_Y);
            Assert.True(u1.rayIntersection(r) != null, "TestHit failed! - Assert 1/2");

            List<HitRecord?> intersection = u1.rayIntersectionList(r);
            List<HitRecord> hits = new List<HitRecord>();
            hits.Add(new HitRecord(
                                    new Point(0.0f, -1.0f, 0f),
                                    new Normal(0.0f, 1.0f, 0f),
                                    new Vec2D(0.75f, 0.5f),
                                    3.0f,
                                    r)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.0f, 1.5f, 0f),
                                    new Normal(0.0f, 1.0f, 0f),
                                    new Vec2D(0.25f, 0.5f),
                                    0.5f,
                                    r)
                                   );
            hits.Sort();

            for (int i = 0; i < 2; i++)
            {
                Assert.True(hits[i].isClose((HitRecord)intersection[i]), "TestRayIntersectionList failed - assert 2/2");
            }

        }

        [Fact]
        public void TestisPointInside()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            CSGUnion u1 = new CSGUnion(s1, s2);


            Point p1 = new Point(0.25f, 0f, 0f);
            Point p2 = new Point(-0.75f, 0f, 0f);
            Point p3 = new Point(1.25f, 0f, 0f);
            Point p4 = new Point(0.25f, 0.999f, 0f);
            Point p5 = new Point(-56789f, 0f, 0f);

            Assert.True(u1.isPointInside(p1), "TestisPointInside failed - assert 1/4");
            Assert.True(u1.isPointInside(p2), "TestisPointInside failed - assert 2/4");
            Assert.True(u1.isPointInside(p3), "TestisPointInside failed - assert 3/4");
            Assert.False(u1.isPointInside(p4), "TestisPointInside failed - assert 4/4");
            Assert.False(u1.isPointInside(p5), "TestisPointInside failed - assert 5/4");

        }
    } // CSG Union Tests

    public class TestCSGDifference
    {
        [Fact]
        public void TestrayIntersction()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere();
            CSGDifference u1 = new CSGDifference(s1, s2);

            Ray r1 = new Ray(origin: new Point(0.0f, 0.0f, 0.0f), dir: Constant.VEC_X);
            HitRecord? intersection1 = u1.rayIntersection(r1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/");
            HitRecord hit1 = new HitRecord(
                new Point(1.0f, 0.0f, 0.0f),
                new Normal(-1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                1.0f,
                r1
            );

            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/");

            Ray r2 = new Ray(origin: new Point(12.0f, 12.0f, 10.0f), dir: Constant.VEC_Z);
            HitRecord? intersection2 = u1.rayIntersection(r2);
            Assert.True(intersection2 == null, "Far away ray test failed - Asser 3/");

            Ray r3 = new Ray(origin: new Point(0.0f, 0.0f, 1.0f), dir: -Constant.VEC_Z);
            HitRecord? intersection3 = u1.rayIntersection(r3);
            Assert.True(intersection3 == null, "Ray through secondShape only test failed - Asser 4/");
        }

        [Fact]
        public void TestrayIntersctionInner()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere();
            CSGDifference u1 = new CSGDifference(s1, s2);

            Ray r1 = new Ray(origin: new Point(1.25f, 0.0f, 0.0f), dir: -Constant.VEC_X);
            HitRecord? intersection1 = u1.rayIntersection(r1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(1.0f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                0.25f,
                r1
            );

            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/5");

            Ray r2 = new Ray(origin: new Point(1.25f, 0.0f, 0.0f), dir: Constant.VEC_X);
            HitRecord? intersection2 = u1.rayIntersection(r2);
            Assert.True(intersection2 != null, "TestHit failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(1.5f, 0.0f, 0.0f),
                new Normal(-1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                0.25f,
                r2
            );

            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");
        }

        [Fact]

        public void TestrayIntersectionList()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere();
            CSGDifference u1 = new CSGDifference(s1, s2);
            Ray r1 = new Ray(origin: new Point(0.5f, 0.0f, 0.0f), dir: Constant.VEC_X);

            List<HitRecord?> intersection = u1.rayIntersectionList(r1);
            List<HitRecord> hits = new List<HitRecord>();


            hits.Add(new HitRecord(
                                    new Point(1.5f, 0f, 0f),
                                    new Normal(-1.0f, 0f, 0f),
                                    new Vec2D(0.0f, 0.5f),
                                    1.0f,
                                    r1)
                                   );
            hits.Add(new HitRecord(
                                    new Point(1.0f, 0.0f, 0.0f),
                                    new Normal(-1.0f, 0.0f, 0f),
                                    new Vec2D(0.0f, 0.5f),
                                    0.5f,
                                    r1)
                                   );
            hits.Sort();

            Assert.True(intersection.Count == hits.Count);
            for (int i = 0; i < intersection.Count; i++)
            {
                Assert.True(hits[i].isClose((HitRecord)intersection[i]), "TestRayIntersectionList failed - assert 2/2");
            }
        }

        [Fact]

        public void TestisPointInside()
        {
            Sphere s1 = new Sphere();
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            CSGDifference u1 = new CSGDifference(s2, s1);


            Point p1 = new Point(1.25f, 0f, 0f);
            Point p2 = new Point(1.25f, 0.2f, 0.2f);
            Point p3 = new Point(0.95f, 0f, 0f);
            Assert.True(u1.isPointInside(p1), "Test isPointInside failed - assert 1/4");
            Assert.True(u1.isPointInside(p2), "Test isPointInside failed - assert 2/4");
            Assert.False(u1.isPointInside(p3), "Test isPointInside failed - assert 4/4");
        }
    } // CSG Difference Tests

    public class TestCSGIntersection
    {
        [Fact]
        public void TestrayIntersction()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(-0.5f, 0.0f, 0.0f)));
            CSGIntersection u1 = new CSGIntersection(s1, s2);

            Ray r1 = new Ray(origin: new Point(1.0f, 0.0f, 0.0f), dir: -Constant.VEC_X);
            HitRecord? intersection1 = u1.rayIntersection(r1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/");
            HitRecord hit1 = new HitRecord(
                new Point(0.5f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                0.5f,
                r1
            );

            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/");

            Ray r2 = new Ray(origin: new Point(12.0f, 12.0f, 10.0f), dir: Constant.VEC_Z);
            HitRecord? intersection2 = u1.rayIntersection(r2);
            Assert.True(intersection2 == null, "Far away ray test failed - Asser 3/");

            Ray r3 = new Ray(origin: new Point(1.0f, 0.0f, 1.0f), dir: -Constant.VEC_Z);
            HitRecord? intersection3 = u1.rayIntersection(r3);
            Assert.True(intersection3 == null, "Ray through firstShape only test failed - Asser 4/");
        }

        [Fact]
        public void TestrayIntersctionInner()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(-0.5f, 0.0f, 0.0f)));
            CSGIntersection u1 = new CSGIntersection(s1, s2);

            Ray r1 = new Ray(origin: new Point(0.0f, 0.0f, 0.0f), dir: -Constant.VEC_X);
            HitRecord? intersection1 = u1.rayIntersection(r1);
            Assert.True(intersection1 != null, "TestHit failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(-0.5f, 0.0f, 0.0f),
                new Normal(1.0f, 0.0f, 0.0f),
                new Vec2D(0.5f, 0.5f),
                0.5f,
                r1
            );

            Assert.True(hit1.isClose(intersection1), "TestHit failed! - Assert 2/5");

            Ray r2 = new Ray(origin: new Point(0.0f, 0.0f, 0.0f), dir: Constant.VEC_X);
            HitRecord? intersection2 = u1.rayIntersection(r2);
            Assert.True(intersection2 != null, "TestHit failed! - Assert 3/5");
            HitRecord hit2 = new HitRecord(
                new Point(0.5f, 0.0f, 0.0f),
                new Normal(-1.0f, 0.0f, 0.0f),
                new Vec2D(0.0f, 0.5f),
                0.5f,
                r2
            );

            Assert.True(hit2.isClose(intersection2), "TestHit failed! - Assert 4/5");
        }

        [Fact]
        public void TestrayIntersectionList()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(-0.5f, 0.0f, 0.0f)));
            CSGIntersection u1 = new CSGIntersection(s1, s2);
            Ray r1 = new Ray(origin: new Point(1.0f, 0.0f, 0.0f), dir: -Constant.VEC_X);

            List<HitRecord?> intersection = u1.rayIntersectionList(r1);
            List<HitRecord> hits = new List<HitRecord>();
            hits.Add(new HitRecord(
                                    new Point(-0.5f, 0.0f, 0.0f),
                                    new Normal(1.0f, 0.0f, 0f),
                                    new Vec2D(0.5f, 0.5f),
                                    1.5f,
                                    r1)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.5f, 0f, 0f),
                                    new Normal(1.0f, 0f, 0f),
                                    new Vec2D(0.0f, 0.5f),
                                    0.5f,
                                    r1)
                                   );
            hits.Sort();

            Assert.True(intersection.Count == hits.Count);
            for (int i = 0; i < intersection.Count; i++)
            {
                Assert.True(hits[i].isClose((HitRecord)intersection[i]), $"TestRayIntersectionList failed - assert 2.{i}/2");
            }
        }

        [Fact]
        public void TestisPointInside()
        {
            Sphere s1 = new Sphere(Transformation.Translation(new Vec(0.5f, 0.0f, 0.0f)));
            Sphere s2 = new Sphere(Transformation.Translation(new Vec(-0.5f, 0.0f, 0.0f)));
            CSGIntersection u1 = new CSGIntersection(s2, s1);


            Point p1 = new Point(0.25f, 0f, 0f);
            Point p2 = new Point(0.25f, 0.1f, 0.1f);
            Point p3 = new Point(0.5f, 0.3f, 0f);
            Assert.True(u1.isPointInside(p1), "Test isPointInside failed - assert 1/4");
            Assert.True(u1.isPointInside(p2), "Test isPointInside failed - assert 2/4");
            Assert.False(u1.isPointInside(p3), "Test isPointInside failed - assert 4/4");
        }

        [Fact]
        public void TestCSGCubeSphere()
        {
            Shape S1 = new Sphere(transformation: Transformation.Scaling(1.2f));
            Shape B1 = new Box();

            CSGIntersection IntCubeSphere = S1 * B1;

            Ray r1 = new Ray(origin: new Point(-5.0f, 0.0f, 0.0f), dir: Constant.VEC_X);
            HitRecord? intersection1 = IntCubeSphere.rayIntersection(r1);
            Assert.True(intersection1 != null, "TestCSGCubeSphere failed! - Assert 1/5");
            HitRecord hit1 = new HitRecord(
                new Point(-1.0f, 0.0f, 0.0f),
                new Normal(-1.0f, 0.0f, 0.0f),
                new Vec2D(0.125f, 0.5f),
                4f,
                r1
            );

            Assert.True(hit1.isClose(intersection1), "TestCSGCubeSphere failed! - Assert 2/5");

            Ray r2 = new Ray(origin: new Point(2f, 2f, 0.0f), dir: (-Constant.VEC_X - Constant.VEC_Y).Normalize());
            HitRecord? intersection2 = IntCubeSphere.rayIntersection(r2);
            Assert.True(intersection2 != null, "TestCSGCubeSphere failed! - Assert 1/5");
            HitRecord hit2 = new HitRecord(
                (S1.transformation * new Point(MathF.Sqrt(2) / 2f, MathF.Sqrt(2) / 2f, 0.0f)),
                (S1.transformation * new Normal(MathF.Sqrt(2) / 2f, MathF.Sqrt(2) / 2f, 0.0f)),
                new Vec2D(0.125f, 0.5f),
                1.6284273f,
                r2
            );

            Console.WriteLine("hit : " + hit2.ToString());
            Console.WriteLine("intersection : " + intersection2.ToString());
            Console.WriteLine(MathF.Sqrt(2) / 2f / 0.5892556f);

            Assert.True(hit2.isClose(intersection2), "TestCSGCubeSphere failed! - Assert 2/5");



        }
    } // CSG Intersection Tests

}

/* Ray r1 = new Ray(origin: new Point(0.75f, 0.0f, 2.0f), dir: -Constant.VEC_Z);
            
            List<HitRecord?> intersection = u1.rayIntersectionList(r1);
            List<HitRecord> hits = new List<HitRecord>();
            hits.Add(new HitRecord(
                                    new Point(0.75f, 0.0f, 0.96824584f),
                                    new Normal(0.25f, 0.0f, 0.96824584f),
                                    new Vec2D(0.0f, 0.30820222f),
                                    3.0f,
                                    r1)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.0f, 1.5f, 0f),
                                    new Normal(0.0f, 1.0f, 0f),
                                    new Vec2D(0.25f, 0.5f),
                                    0.5f,
                                    r1)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.0f, 1.5f, 0f),
                                    new Normal(0.0f, 1.0f, 0f),
                                    new Vec2D(0.25f, 0.5f),
                                    0.5f,
                                    r1)
                                   );
            hits.Add(new HitRecord(
                                    new Point(0.75f, 0.0f, -0.96824584f),
                                    new Normal(0.0f, 1.0f, 0f),
                                    new Vec2D(0.25f, 0.5f),
                                    0.5f,
                                    r1)
                                   );    
*/

