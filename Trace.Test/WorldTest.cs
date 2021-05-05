using Xunit;
using Trace;
using System.Numerics;
using System;

namespace Trace.Test
{
    public class HitRecordAndWorldTest
    {
        [Fact]
        public void TestIsCloseHitRecord() 
        {
            HitRecord myHR = new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(3.0f,5.0f,6.0f), new Vec2D(-3.0f,7.0f), 3.5f, new Ray(new Point(10.0f,4.0f,4.0f), Constant.VEC_X));
            HitRecord myHR2 = new HitRecord(new Point(0.0f, 0.0f, 1.0f), new Normal(3.0f,5.0f,6.0f), new Vec2D(-3.0f,7.0f), 3.5f, new Ray(new Point(10.0f,4.0f,4.0f), Constant.VEC_X));
            HitRecord myHR3 = new HitRecord(new Point(0.0f, 0.0f, 2.0f), new Normal(-1.0f,5.0f,6.0f), new Vec2D(-3.0f,7.0f), 3.5f, new Ray(new Point(0.0f,4.0f,4.0f), Constant.VEC_X));


            Assert.True(myHR.isClose(myHR2));
            Assert.False(myHR.isClose(myHR3));
            return;
        }

        [Fact]
        public void TestRayIntersection() 
        {
            World w = new World();
            Ray myRay = new Ray(new Point(5.0f, 0.0f, 0.0f),-Constant.VEC_X);
            w.addShape(new Sphere()); //Sphere in the origin
            w.addShape(new Sphere(Transformation.Translation(new Vec(2.0f, 0.0f, 0.0f)))); //Centred in (2,0,0)

            HitRecord? TrueHitRecord = w.shapes[1].rayIntersection(myRay);
            Assert.True( TrueHitRecord?.isClose( w.rayIntersection(myRay)));
        }




    } //end of class

} //end of namespace