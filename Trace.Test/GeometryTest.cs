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
using System.Numerics;

namespace Trace.Test
{

    // Vec and Point testing

    public class GeometryTest
    {
        // Vec tests
        [Fact]
        public void TestIsCloseVec()
        {
            Vec a = new Vec(1.0f, 2.0f, 3.0f);
            Vec b = new Vec(4.0f, 6.0f, 8.0f);
            Assert.True(a.isClose(a), "TestIsCloseVec failed! Assert 1/2");
            Assert.False(a.isClose(b), "TestIsCloseVec failed! Assert 2/2");
        }

        [Fact]
        public void TestVectorOperations()
        {
            Vec a = new Vec(1.0f, 2.0f, 3.0f);
            Vec b = new Vec(4.0f, 6.0f, 8.0f);
            Vec c = new Vec(15.0f, 0.0f, 20.0f);
            Assert.True((a + b).isClose(new Vec(5.0f, 8.0f, 11.0f)), "TestVectorOperations failed for addition of two Vec");
            Assert.True((b - a).isClose(new Vec(3.0f, 4.0f, 5.0f)), "TestVectorOperations failed for subtraction of two Vec");

            Assert.True( (a * 3.0f).isClose(new Vec (3.0f, 6.0f, 9.0f)), "TestVectorOperation failed in product with a scalar");
            Assert.True( (3.0f * a).isClose(new Vec (3.0f, 6.0f, 9.0f)), "TestVectorOperation failed in product with a scalar");
            Assert.True( (b / 2.0f).isClose(new Vec (2.0f, 3.0f, 4.0f)), "TestVectorOperation failed in division for a scalar");

            Assert.True( (a*b) == (40.0f), "TestVectorOperation failed in scalar product");
            Assert.True( (a.crossProd(b)).isClose( new Vec (-2.0f, 4.0f, -2.0f)), "TestVectorOperation failed in cross product (1/2)");
            Assert.True( (b.crossProd(a)).isClose(new Vec (2.0f, -4.0f, 2.0f)), "TestVectorOperation failed in cross product (2/2)");

            Assert.True(c.getSquaredNorm()== 625f, "TestVectorOperation failed in getSquaredNorm");
            Assert.True(c.getNorm()== 25.0f, "TestVectorOperation failed in getNorm");
            Assert.True(c.Normalize().isClose(new Vec (0.6f, 0.0f, 0.8f)));

        }

        [Fact]
        public void isNormalizedTest()
        {
            Vec a = new Vec(0.0f, 0.8f, 0.6f);
            Vec b = new Vec(4.0f, -2.5f, 3.9f);

            Assert.True(a.isNormalized(), "Test isNormalized Failed! Assert 1/2");
            Assert.False(b.isNormalized(), "Test isNormalized Failed! Assert 1/2");
        }

        // Point tests
        [Fact]
        public void TestIsClosePoint()
        {
            Point a = new Point(1.0f, 2.0f, 3.0f);
            Point b = new Point(4.0f, 6.0f, 8.0f);

            Assert.True(a.isClose(a), "TestIsClosePoint failed! Assert 1/2");
            Assert.False(b.isClose(a), "TestIsClosePoint failed! Assert 2/2");
        }

        [Fact]
        public void TestPointOperations()
        {
             Point a = new Point(1.0f, 2.0f, 3.0f);
             Point b = new Point(4.0f, 6.0f, 8.0f);
             Vec c = new Vec(4.0f, 6.0f, 8.0f);

             Assert.True((a + c).isClose(new Point(5.0f, 8.0f, 11.0f)), "TestPointOperations failed for Point + Vec operation");
             Assert.True((a - c).isClose(new Point(-3.0f, -4.0f, -5.0f)), "TestPointOperations failed for Point - Vec operation");
             Assert.True((b - a).isClose(new Vec(3.0f, 4.0f, 5.0f)),"TestPointOperations failed for Point - Point operation");
        }
        [Fact]
        public void TestAreClose()
        {
            Transformation m1 = new Transformation (new Matrix4x4(  1.0f, 2.0f, 3.0f, 4.0f,
                                                                    5.0f, 6.0f, 7.0f, 8.0f,
                                                                    9.0f, 9.0f, 8.0f, 7.0f,
                                                                    6.0f, 5.0f, 4.0f, 1.0f),
                                                    new Matrix4x4(  -3.75f, -2.75f, -1.0f, 0.0f,
                                                                    4.375f, -3.875f, 2.0f, -0.5f,
                                                                    0.5f, 0.5f, -1.0f, 1.0f,
                                                                    -1.375f, 0.875f, 0.0f, -0.5f));

            Assert.True(m1.isConsistent());

            Transformation m2  = new Transformation(m1.M, m1.Minv);
            Assert.True(m1.areClose(m2.M));

<<<<<<< HEAD
        [Fact]
        public void TestMatrixProducts()
        {
            Transformation A = new Transformation();
        }

=======
            Transformation m3  = new Transformation(m1.M, m1.Minv);
            m3.M.M22 = m3.M.M22 + 1.0f;
            Assert.False(m1.areClose(m3.M));

            Transformation m4  = new Transformation(m1.M, m1.Minv);
            m4.Minv.M22 = m3.Minv.M22 + 1.0f;
            Assert.False(m1.areClose(m3.Minv));

        }
>>>>>>> 8d224dfc4ff948ff436887ffe00aae0da027b942
    }

}