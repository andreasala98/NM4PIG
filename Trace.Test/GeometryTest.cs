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
            Assert.True((a + b).isClose(new Vec(5.0f, 8.0f, 11.0f)), "TestVectorOperations failed for addition of two Vec");
            Assert.True((b - a).isClose(new Vec(3.0f, 4.0f, 5.0f)), "TestVectorOperations failed for subtraction of two Vec");

            //Insert here other tests for operations

        }

    }

}