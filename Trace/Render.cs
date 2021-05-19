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
namespace Trace
{


    public abstract class Render {

        public World wrld;
        public Color bkgColor;
        public abstract Color computeRadiance(Ray r);

    }


    public class OnOffRender : Render
    {

        public OnOffRender(World w, Color? bkg=null)
        {
            this.wrld = w;
            this.bkgColor = bkg ?? Constant.Black;
        }

        public override Color computeRadiance(Ray r)
        {
            return this.wrld.rayIntersection(r) != null ? Constant.White : this.bkgColor;
        }


    }

    public class FlatRender : Render
    {
        public FlatRender(World wo, Color? bkg = null) 
        {
            this.wrld = wo;
            this.bkgColor = bkg ?? Constant.Black;
        }

        public override Color computeRadiance(Ray r)
        {
            HitRecord? hr = this.wrld.rayIntersection(r);
            if (hr==null) return bkgColor;
            Material mat = ((Shape)hr?.shape).material;

            return mat.brdf.pigment.getColor((Vec2D)hr?.surfacePoint) + mat.emittedRadiance.getColor((Vec2D)hr?.surfacePoint);
        }

    }







} // trace