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
using System.Numerics;
using System.Collections.Generic;
using System;

namespace Trace.Test
{
    public class GeometryTest
    {
        // Vec tests
        [Fact]
        public void TestIsCloseVec()
        {
            Vec a = new Vec(1.0f, 2.0f, 3.0f);
            Vec b = new Vec(4.0f, 6.0f, 8.0f);
            Assert.True(a.isClose(a), "TestIsCloseVec failed - Assert 1/2");
            Assert.False(a.isClose(b), "TestIsCloseVec failed - Assert 2/2");
        }

        [Fact]
        public void TestVectorOperations()
        {
            Vec a = new Vec(1.0f, 2.0f, 3.0f);
            Vec b = new Vec(4.0f, 6.0f, 8.0f);
            Vec c = new Vec(15.0f, 0.0f, 20.0f);
            Assert.True((a + b).isClose(new Vec(5.0f, 8.0f, 11.0f)), "TestVectorOperations failed for addition of two Vec");
            Assert.True((b - a).isClose(new Vec(3.0f, 4.0f, 5.0f)), "TestVectorOperations failed for subtraction of two Vec");

            Assert.True((a * 3.0f).isClose(new Vec(3.0f, 6.0f, 9.0f)), "TestVectorOperation failed in product with a scalar");
            Assert.True((3.0f * a).isClose(new Vec(3.0f, 6.0f, 9.0f)), "TestVectorOperation failed in product with a scalar");
            Assert.True((b / 2.0f).isClose(new Vec(2.0f, 3.0f, 4.0f)), "TestVectorOperation failed in division for a scalar");

            Assert.True((a * b) == (40.0f), "TestVectorOperation failed in scalar product");
            Assert.True((a.crossProd(b)).isClose(new Vec(-2.0f, 4.0f, -2.0f)), "TestVectorOperation failed in cross product (1/2)");
            Assert.True((b.crossProd(a)).isClose(new Vec(2.0f, -4.0f, 2.0f)), "TestVectorOperation failed in cross product (2/2)");

            Assert.True(c.getSquaredNorm() == 625f, "TestVectorOperation failed in getSquaredNorm");
            Assert.True(c.getNorm() == 25.0f, "TestVectorOperation failed in getNorm");
            Assert.True(c.Normalize().isClose(new Vec(0.6f, 0.0f, 0.8f)));
        }

        [Fact]
        public void isNormalizedTest()
        {
            Vec a = new Vec(0.0f, 0.8f, 0.6f);
            Vec b = new Vec(4.0f, -2.5f, 3.9f);

            Assert.True(a.isNormalized(), "Test isNormalized Failed - Assert 1/2");
            Assert.False(b.isNormalized(), "Test isNormalized Failed - Assert 1/2");
        }

        // Point tests
        [Fact]
        public void TestIsClosePoint()
        {
            Point a = new Point(1.0f, 2.0f, 3.0f);
            Point b = new Point(4.0f, 6.0f, 8.0f);

            Assert.True(a.isClose(a), "TestIsClosePoint failed - Assert 1/2");
            Assert.False(b.isClose(a), "TestIsClosePoint failed - Assert 2/2");
        }

        [Fact]
        public void TestPointOperations()
        {
            Point a = new Point(1.0f, 2.0f, 3.0f);
            Point b = new Point(4.0f, 6.0f, 8.0f);
            Vec c = new Vec(4.0f, 6.0f, 8.0f);

            Assert.True((a + c).isClose(new Point(5.0f, 8.0f, 11.0f)), "TestPointOperations failed for Point + Vec operation");
            Assert.True((a - c).isClose(new Point(-3.0f, -4.0f, -5.0f)), "TestPointOperations failed for Point - Vec operation");
            Assert.True((b - a).isClose(new Vec(3.0f, 4.0f, 5.0f)), "TestPointOperations failed for Point - Point operation");
        }

        [Fact]
        public void TestAreClose()
        {
            Transformation m1 = new Transformation(new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f,
                                                                    5.0f, 6.0f, 7.0f, 8.0f,
                                                                    9.0f, 9.0f, 8.0f, 7.0f,
                                                                    6.0f, 5.0f, 4.0f, 1.0f),
                                                    new Matrix4x4(-3.750f, 2.750f, -1.0f, 0.00f,
                                                                    4.3750f, -3.875f, 2.00f, -0.5f,
                                                                    0.5000f, 0.500f, -1.0f, 1.00f,
                                                                    -1.375f, 0.875f, 0.00f, -0.5f));

            Assert.True(m1.isConsistent(), "TestAreClose failed! - Assert 1/4");

            Transformation m2 = new Transformation(m1.M, m1.Minv);
            Assert.True(m1.isClose(m2), "TestAreClose failed! - Assert 2/4");

            Transformation m3 = new Transformation(m1.M, m1.Minv);
            m3.M.M22 = m3.M.M22 + 1.0f;
            Assert.False(m1.isClose(m3), "TestAreClose failed! - Assert 3/4");

            Transformation m4 = new Transformation(m1.M, m1.Minv);
            m4.Minv.M22 = m3.Minv.M22 + 1.0f;
            Assert.False(m1.isClose(m3), "TestAreClose failed! - Assert 4/4");

        }

        [Fact]
        public void TestRotations()
        {
            Assert.True(Transformation.RotationX(0.1f).isConsistent(), "TestRotation failed - Assert 1/6");
            Assert.True(Transformation.RotationY(0.1f).isConsistent(), "TestRotation failed - Assert 2/6");
            Assert.True(Transformation.RotationZ(0.1f).isConsistent(), "TestRotation failed - Assert 3/6");

            Assert.True((Transformation.RotationX(Constant.PI / 2.0f) * Constant.VEC_Y).isClose(Constant.VEC_Z), "TestRotation failed - Assert 4/6");
            Assert.True((Transformation.RotationY(Constant.PI / 2.0f) * Constant.VEC_Z).isClose(Constant.VEC_X), "TestRotation failed - Assert 5/6");
            Assert.True((Transformation.RotationZ(Constant.PI / 2.0f) * Constant.VEC_X).isClose(Constant.VEC_Y), "TestRotation failed - Assert 6/6");
        }

        [Fact]
        public void TestTransformationTranslation()
        {
            Transformation tr1 = Transformation.Translation(new Vec(1.0f, 2.0f, 3.0f));
            Assert.True(tr1.isConsistent(), "TestTransformationTranslation failed - Assert 1/4");

            Transformation tr2 = Transformation.Translation(new Vec(4.0f, 6.0f, 8.0f));
            Assert.True(tr2.isConsistent(), "TestTransformationTranslation failed - Assert 2/4");

            Transformation prod = tr1 * tr2;
            Assert.True(prod.isConsistent(), "TestTransformationTranslation failed - Assert 3/4");

            Transformation expected = Transformation.Translation(new Vec(5.0f, 8.0f, 11.0f));
            Assert.True(prod.isClose(expected), "TestTransformationTranslation failed - Assert 4/4");
        }

        [Fact]
        public void TestTransformationScaling()
        {
            Transformation tr1 = Transformation.Scaling(new Vec(2.0f, 5.0f, 10.0f));
            Assert.True(tr1.isConsistent(), "TestTransformationScaling failed - Assert 1/3");

            Transformation tr2 = Transformation.Scaling(new Vec(3.0f, 2.0f, 4.0f));
            Assert.True(tr2.isConsistent(), "TestTransformationScaling failed - Assert 2/3");

            Transformation expected = Transformation.Scaling(new Vec(6.0f, 10.0f, 40.0f));
            Assert.True(expected.isClose(tr1 * tr2), "TestTransformationScaling failed - Assert 3/3");
        }

        [Fact]
        public void TestMatrixProducts()
        {
            Transformation T1 = new Transformation(new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f,
                                                                    5.0f, 6.0f, 7.0f, 8.0f,
                                                                    9.0f, 9.0f, 8.0f, 7.0f,
                                                                    6.0f, 5.0f, 4.0f, 1.0f),
                                                    new Matrix4x4(-3.750f, 2.750f, -1.0f, 0.00f,
                                                                    4.3750f, -3.875f, 2.00f, -0.5f,
                                                                    0.5000f, 0.500f, -1.0f, 1.00f,
                                                                    -1.375f, 0.875f, 0.00f, -0.5f));

            Transformation T2 = new Transformation(new Matrix4x4(2.0f, 1.0f, 0.0f, -1.0f,
                                                                     1.0f, 1.0f, -1.0f, 1.0f,
                                                                     2.0f, 2.0f, 2.0f, 2.0f,
                                                                    -1.0f, -1.0f, 1.0f, 1.0f),
                                                    new Matrix4x4(1.0f, 0.5f, -0.25f, 1.00f,
                                                                    -1.0f, -0.5f, 0.5f, -1.5f,
                                                                     0.0f, -0.5f, 0.25f, 0.00f,
                                                                     0.0f, 0.5f, 0.00f, 0.5f));


            Vec v = new Vec(3.0f, -1.0f, 4.0f);
            Normal n = new Normal(1.0f, -1.0f, 1.0f);
            Point p = new Point(0.0f, 1.0f, 1.0f);

            Assert.True(Utility.areMatricesClose((T1 * T2).M, new Matrix4x4(6f, 5f, 8f, 11f,
                                                                            22f, 17f, 16f, 23f,
                                                                            36f, 27f, 14f, 23f,
                                                                            24f, 18f, 4f, 8f)), "TestMatrixProducts failed - Assert 1/4");
            Assert.True((T1 * v).isClose(new Vec(13f, 37f, 50f)), "TestMatrixProducts failed - Assert 2/4");
            Assert.True((T2 * n).isClose(new Normal(2.0f, 0.5f, -0.5f)), "TestMatrixProducts failed - Assert 3/4");
            Assert.True((T2 * p).isClose(new Point(0.0f, 1.0f, 6.0f)), "TestMatrixProducts failed - Assert 4/4");
        }


        [Fact]
        public void TestGetInverse()
        {
            Transformation T1 = new Transformation(new Matrix4x4(1.0f, 2.0f, 3.0f, 4.0f,
                                                                    5.0f, 6.0f, 7.0f, 8.0f,
                                                                    9.0f, 9.0f, 8.0f, 7.0f,
                                                                    6.0f, 5.0f, 4.0f, 1.0f),
                                                    new Matrix4x4(-3.750f, 2.750f, -1.0f, 0.00f,
                                                                    4.3750f, -3.875f, 2.00f, -0.5f,
                                                                    0.5000f, 0.500f, -1.0f, 1.00f,
                                                                    -1.375f, 0.875f, 0.00f, -0.5f));
            Transformation T1_inv = T1.getInverse();
            Assert.True((T1 * T1_inv).isClose(new Transformation(1)), "TestGetInverse failed - Assert 1/1");
        }

        [Fact]
        public void ONBRandomTestingNormal()
        {
            PCG pcg = new PCG();

            for (int i = 0; i < 1E5; i++) {
                Normal n = new Normal(pcg.randomFloat(), pcg.randomFloat(), pcg.randomFloat()).Normalize();
                
                List<Vec> onb = n.createONBfromZ();

                Assert.True(onb[2].isClose(n.toVec()), "ONBRandomTesting Failed! Assert 1");
        
                Assert.True(Utility.areClose(onb[0] * onb[1], 0f), $"{onb[0] * onb[1]} is not close to {0f}!!");
                Assert.True(Utility.areClose(onb[1] * onb[2], 0f), $"{onb[1] * onb[2]} is not close to {0f}!!");
                Assert.True(Utility.areClose(onb[0] * onb[2], 0f), $"{onb[0] * onb[2]} is not close to {0f}!!");

                Assert.True(Utility.areClose(onb[0].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 5");
                Assert.True(Utility.areClose(onb[1].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 6");
                Assert.True(Utility.areClose(onb[2].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 7");
            }
        }

        [Fact]
        public void ONBRandomTestingVec()
        {
            PCG pcg = new PCG();

            for (int i = 0; i < 1E5; i++) {
                Vec n = new Vec(pcg.randomFloat(), pcg.randomFloat(), pcg.randomFloat()).Normalize();
                
                List<Vec> onb = n.createONBfromZ();

                Assert.True(onb[2].isClose(n), "ONBRandomTesting Failed! Assert 1");
        
                Assert.True(Utility.areClose(onb[0] * onb[1], 0f), $"{onb[0] * onb[1]} is not close to {0f}!!");
                Assert.True(Utility.areClose(onb[1] * onb[2], 0f), $"{onb[1] * onb[2]} is not close to {0f}!!");
                Assert.True(Utility.areClose(onb[0] * onb[2], 0f), $"{onb[0] * onb[2]} is not close to {0f}!!");

                Assert.True(Utility.areClose(onb[0].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 5");
                Assert.True(Utility.areClose(onb[1].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 6");
                Assert.True(Utility.areClose(onb[2].getSquaredNorm(),1f), "ONBRandomTesting Failed! Assert 7");
            }
        }

    } // end of Geometry test

} //end of namespace Trace.Test