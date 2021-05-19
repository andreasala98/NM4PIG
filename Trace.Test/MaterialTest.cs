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


namespace Trace.Test
{
    public class TestPigment
    {
        [Fact]
        public void TestUniform()
        {
            Color c = new Color(1.0f, 2.0f, 3.0f);
            UniformPigment p = new UniformPigment(c);

            Assert.True(p.getColor(new Vec2D(0f, 1f).isClose(c)), "Test failed, 1/4");
            Assert.True(p.getColor(new Vec2D(0f, 0f).isClose(c)), "Test failed, 2/4");
            Assert.True(p.getColor(new Vec2D(1f, 0f).isClose(c)), "Test failed, 3/4");
            Assert.True(p.getColor(new Vec2D(1f, 1f).isClose(c)), "Test failed, 4/4");

        }

        [Fact]
        public void TestImagePigment()
        {
            HdrImage image = new HdrImage(2, 2);
            image.setPixel(0, 0, new Color(1.0f, 2.0f, 3.0f));
            image.setPixel(1, 0, new Color(2.0f, 3.0f, 1.0f));
            image.setPixel(0, 1, new Color(2.0f, 1.0f, 3.0f));
            image.setPixel(1, 1, new Color(3.0f, 2.0f, 1.0f));

            ImagePigment pig = new ImagePigment(image);

            Assert.True(pig.getColor(new Vec2D(0f, 0f).isClose(new Color(1.0f, 2.0f, 3.0f))), "Test failed, 1/4");
            Assert.True(pig.getColor(new Vec2D(0f, 1f).isClose(new Color(2.0f, 1.0f, 3.0f))), "Test failed, 2/4");
            Assert.True(pig.getColor(new Vec2D(1f, 0f).isClose(new Color(2.0f, 3.0f, 1.0f))), "Test failed, 3/4");
            Assert.True(pig.getColor(new Vec2D(1f, 1f).isClose(new Color(3.0f, 2.0f, 1.0f))), "Test failed, 4/4");
        }
    }
}