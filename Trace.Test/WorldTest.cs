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
using System;

namespace Trace.Test
{
    public class HitRecordAndWorldTest
    {
        [Fact]
        public void TestIsCloseHitRecord()
        {
            HitRecord myHR = new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(3.0f, 5.0f, 6.0f), new Vec2D(-3.0f, 7.0f), 3.5f, new Ray(new Point(10.0f, 4.0f, 4.0f), Constant.VEC_X));
            HitRecord myHR2 = new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(3.0f, 5.0f, 6.0f), new Vec2D(-3.0f, 7.0f), 3.5f, new Ray(new Point(10.0f, 4.0f, 4.0f), Constant.VEC_X));
            HitRecord myHR3 = new HitRecord(new Point(0.0f, 0.0f, 2.0f), new Normal(-1.0f, 5.0f, 6.0f), new Vec2D(-3.0f, 7.0f), 3.5f, new Ray(new Point(0.0f, 4.0f, 4.0f), Constant.VEC_X));


            Assert.True(myHR.isClose(myHR2));
            Assert.False(myHR.isClose(myHR3));
            return;
        }

        [Fact]
        public void TestRayIntersection()
        {
            World w = new World();
            // mettere 2 sfere e verifiare che l'HitRecord è quello della sfera più avanti
            w.addShape(new Sphere());
        }
    } //end of class

} //end of namespace