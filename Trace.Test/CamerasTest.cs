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
using Trace;

namespace Trace.Test
{

    public class RayTest
    {
        Ray ray1 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray2 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        Ray ray3 = new Ray(new Point(5.0f, 1.0f, 4.0f), new Vec(3.0f, 9.0f, 4.0f));
        Ray ray4 = new Ray(new Point(1.0f, 2.0f, 4.0f), new Vec(4.0f,2.0f,1.0f));
    
        [Fact]
        public void TestIsRayClose()
        {
            Assert.True(ray1.isClose(ray2));
            Assert.False(ray1.isClose(ray3));
        }

        [Fact]
        public void TestAt()
        {
            Assert.True( ray4.at(0.0f).isClose(ray4.origin) );
            Assert.True( ray4.at(1.0f).isClose(new Point(5.0f, 4.0f, 5.0f)) );
            Assert.True( ray4.at(2.0f).isClose(new Point(9.0f, 6.0f, 6.0f)) );
        }

        [Fact]
        public void TestTransform()
         {
            Ray ray5 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(6.0f, 5.0f, 4.0f));
            Transformation T = Transformation.Translation(new Vec(10.0f, 11.0f, 12.0f)) * Transformation.rotationX( (float)System.Math.PI / 2.0f );

            Ray transf = ray5.Transform(T);
            Assert.True(transf.origin.isClose(new Point(11.0f, 8.0f, 14.0f)));
            Assert.True(transf.dir.isClose(new Vec(6.0f, -4.0f, 5.0f)));
        }

    }


    public class CameraTest
    {
    


    }


    public class ImageTracerTest
    {



    }

}
