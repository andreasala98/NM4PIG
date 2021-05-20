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
    /// <summary>
    /// Class to collect test for the <see cref="HitRecord"> and <see cref="World"> classes.
    /// </summary>
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
            Ray myRay = new Ray(new Point(5.0f, 0.0f, 0.0f), -Constant.VEC_X);
            w.addShape(new Sphere()); //Sphere in the origin
            w.addShape(new Sphere(Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)))); //Centred in (2,0,0)

            HitRecord? TrueHitRecord = w.shapes[1].rayIntersection(myRay);
            Assert.True(TrueHitRecord?.isClose(w.rayIntersection(myRay)));
        }
    } //end of class

} //end of namespace