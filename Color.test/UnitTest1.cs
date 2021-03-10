using System;
using Xunit;

namespace Color.test
{
    public class ColorTest
    {
        [Fact]
        // Test scalar product
        public void TestScalarProduct()
        {
            Colore a = new Colore (1.0, 2.5, 3.7);
            double alfa = 2.5;

            Assert.True(Colore.are_close(new Colore (2.5, 6.25, 9.25), a * alfa));
            Assert.True(Colore.are_close(new Colore (2.5, 6.25, 9.25), alfa * a));

        }
    }
}
