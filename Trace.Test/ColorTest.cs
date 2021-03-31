using Xunit;

namespace Trace.Test
{

    // Color testing

    public class ColorTest
    {

        [Fact]
        // Test scalar product
        public void TestScalarProduct()
        {
            Color a = new Color(1.0f, 2.5f, 3.7f);
            float alfa = 2.5f;
            Assert.True(new Color(2.5f, 6.25f, 9.25f).isClose(a * alfa), "Scalar product Test failed");
            Assert.True(new Color(2.5f, 6.25f, 9.25f).isClose(alfa * a), "Scalar product Test failed");
        }

        [Fact]
        public void TestSum()
        {
            Color col1 = new Color(4.0f, 5.0f, 6.0f);
            Color col2 = new Color(1.0f, 2.0f, 3.0f);
            Assert.True(new Color(5.0f, 7.0f, 9.0f).isClose(col1 + col2), "Sum Test failed!");
        }

        [Fact]
        public void TestDifference()
        {
            Color col1 = new Color(4.0f, 5.0f, 6.0f);
            Color col2 = new Color(1.0f, 2.0f, 3.0f);
            Assert.True(new Color(3.0f, 3.0f, 3.0f).isClose(col1 - col2), "Difference Test failed!");
        }

        [Fact]
        public void TestProduct()
        {
            Color col1 = new Color(4.0f, 5.0f, 6.0f);
            Color col2 = new Color(1.0f, 2.0f, 3.0f);
            Assert.True(new Color(4.0f, 10.0f, 18.0f).isClose(col1 * col2), "Product between colours Test failed!");
        }

        [Fact]
        public void TestLuminosity()
        {
            Color col1 = new Color(1.0f, 2.0f, 3.0f);
            Color col2 = new Color(9.0f, 5.0f, 7.0f);
            Assert.True(col1.Luminosity() == 2.0f, "Pixel Luminosity Test failed!");
            Assert.True(col2.Luminosity() == 7.0f, "Pixel Luminosity Test failed!");
        }


    }

}