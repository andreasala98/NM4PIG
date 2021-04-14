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