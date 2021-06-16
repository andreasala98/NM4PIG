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

using System;
using System.Collections;

namespace Trace
{
#nullable enable

    /// <summary>
    ///  A class holding information about a ray-shape intersection
    /// </summary>
    public struct HitRecord : IComparable<HitRecord>
    {
        /// <summary>
        /// a <see cref="Point"/> object holding the world coordinates of the hit point
        /// </summary>
        public Point worldPoint;
        /// <summary>
        /// a <see cref="Normal"/> object holding the orientation of the normal to the surface where the hit happened
        /// </summary>
        public Normal normal;

        /// <summary>
        /// a <see cref="Vec2D"> object holding the position of the hit point on the surface of the object
        /// </summary>
        public Vec2D surfacePoint;
        /// <summary>
        /// a floating-point value specifying the distance from the origin of the <see cref="Ray"> where the hit happened
        /// </summary>
        public float t;
        /// <summary>
        /// The <see cref="Ray"/> that hit the surface
        /// </summary>
        public Ray ray;

        /// <summary>
        /// The <see cref="Shape"> intersected by the Ray
        /// </summary>
        public Shape? shape;


        public HitRecord(Point wp, Normal nm, Vec2D sp, float tt, Ray r, Shape? shape = null)
        {
            this.worldPoint = wp;
            this.normal = nm;
            this.surfacePoint = sp;
            this.t = tt;
            this.ray = r;
            this.shape = shape;
        }

        public bool isClose(HitRecord? other)
        {
            if (other == null) return false;
            return (this.worldPoint.isClose(other.Value.worldPoint)
                            && this.normal.isClose(other.Value.normal)
                            && this.surfacePoint.isClose(other.Value.surfacePoint)
                            && this.ray.isClose(other.Value.ray)
                        );
        }



        // Nested class to do descending sort on year property.
      private class sortHelper: IComparer
      {
         int IComparer.Compare(object? a1, object? b1)
         {

                HitRecord? a = (HitRecord?)a1;
                HitRecord? b = (HitRecord?)b1;

                if (a?.t < b?.t)
               return 1;

            else if (a?.t > b?.t)
               return -1;

            else
               return 0;
         }
      }

      public static IComparer sortDescending()
        {
            return (IComparer) new sortHelper();
        }



        /// <summary>
        /// Default method to compare two HitRecord objects. 
        /// One HitRecord is "bigger" than another one if its t parameter is bigger.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public int CompareTo(HitRecord a)
        {
            return this.t.CompareTo(a.t);
        }


        public override string ToString()
        {

            string Cons = "wPoint: " + this.worldPoint.ToString() + "\n"
                         + "Normal: " + this.normal.ToString() + "\n"
                         + "sPoint: " + this.surfacePoint.ToString() + "\n"
                         + "t: " + this.t + "\n";
            return Cons;
        }

        /// <summary>
        /// Method that compares two HitRecord object by the t parameter.
        /// It returns the lesser one.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static HitRecord lesserComparison (HitRecord a, HitRecord b)
        {
            if (a.t < b.t)
                return a;
            else
                return b;
        }


    } //HitRecord

} //Trace