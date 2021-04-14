using Xunit;
using System;

namespace Trace.Test
{

    // Color testing

    public class GeometryTest
    {

        [Fact]
        public void TestIsClose()
        {
            Vec a = new Vec(1.0f, 2.0f, 3.0f);
            Vec b = new Vec(4.0f, 6.0f, 8.0f);
            Assert.True(a.isClose(a), "TestIsClose failed! Assert 1/2");
            Assert.False(a.isClose(b), "TestIsClose failed! Assert 2/2");
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

    }

}