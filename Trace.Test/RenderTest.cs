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
using System.Collections.Generic;

namespace Trace.Test
{
    public class Render
    {
        [Fact]
        public void TestOnOffRender()
        {

            Sphere sphere = new Sphere(
                                        transformation: Transformation.Translation(new Vec(2f, 0f, 0f)) * Transformation.Scaling(new Vec(0.2f, 0.2f, 0.2f)),
                                        material: new Material(Brdf: new DiffuseBRDF(pig: new UniformPigment(Constant.White))));
            HdrImage image = new HdrImage(3, 3);
            OrthogonalCamera camera = new OrthogonalCamera();
            ImageTracer tracer = new ImageTracer(i: image, c: camera, sps: null);
            World world = new World();
            world.addShape(sphere);
            OnOffRender renderer = new OnOffRender(world: world);
            tracer.fireAllRays(renderer);

            Assert.True(image.getPixel(0, 0).isClose(Constant.Black), "TestOnOffRender failed - Assert 1/9");
            Assert.True(image.getPixel(1, 0).isClose(Constant.Black), "TestOnOffRender failed - Assert 2/9");
            Assert.True(image.getPixel(2, 0).isClose(Constant.Black), "TestOnOffRender failed - Assert 3/9");
            Assert.True(image.getPixel(0, 1).isClose(Constant.Black), "TestOnOffRender failed - Assert 4/9");
            Assert.True(image.getPixel(1, 1).isClose(Constant.White), "TestOnOffRender failed - Assert 5/9");
            Assert.True(image.getPixel(2, 1).isClose(Constant.Black), "TestOnOffRender failed - Assert 6/9");
            Assert.True(image.getPixel(0, 2).isClose(Constant.Black), "TestOnOffRender failed - Assert 7/9");
            Assert.True(image.getPixel(1, 2).isClose(Constant.Black), "TestOnOffRender failed - Assert 8/9");
            Assert.True(image.getPixel(2, 2).isClose(Constant.Black), "TestOnOffRender failed - Assert 9/9");
        }

        [Fact]
        public void TestFlatRender()
        {
            Color sphereColor = new Color(1f, 2f, 3f);
            Sphere sphere = new Sphere(
                transformation: Transformation.Translation(new Vec(2f, 0f, 0f)) * Transformation.Scaling(new Vec(0.2f, 0.2f, 0.2f)),
                material: new Material(Brdf: new DiffuseBRDF(pig: new UniformPigment(sphereColor)))
                );

            HdrImage image = new HdrImage(3, 3);
            OrthogonalCamera camera = new OrthogonalCamera();
            ImageTracer tracer = new ImageTracer(image, camera, null);
            World world = new World();
            world.addShape(sphere);
            FlatRender renderer = new FlatRender(world);
            tracer.fireAllRays(renderer);

            Assert.True(image.getPixel(0, 0).isClose(Constant.Black), "TestFlatRender failed - Assert 1/9");
            Assert.True(image.getPixel(1, 0).isClose(Constant.Black), "TestFlatRender failed - Assert 2/9");
            Assert.True(image.getPixel(2, 0).isClose(Constant.Black), "TestFlatRender failed - Assert 3/9");
            Assert.True(image.getPixel(0, 1).isClose(Constant.Black), "TestFlatRender failed - Assert 4/9");
            Assert.True(image.getPixel(1, 1).isClose(sphereColor), "TestFlatRender failed - Assert 5/9");
            Assert.True(image.getPixel(2, 1).isClose(Constant.Black), "TestFlatRender failed - Assert 6/9");
            Assert.True(image.getPixel(0, 2).isClose(Constant.Black), "TestFlatRender failed - Assert 7/9");
            Assert.True(image.getPixel(1, 2).isClose(Constant.Black), "TestFlatRender failed - Assert 8/9");
            Assert.True(image.getPixel(2, 2).isClose(Constant.Black), "TestFlatRender failed - Assert 9/9");
        }

        [Fact]
        public void TestPathTracer()
        {
            PCG pcg = new PCG();

            for (int i = 0; i < 5; i++)
            {
                World world = new World();

                float emittedRadiance = pcg.randomFloat();
                float reflectance = pcg.randomFloat();
                Material enclosureMaterial = new Material(
                    Brdf: new DiffuseBRDF(pig: new UniformPigment(new Color(1f, 1f, 1f) * reflectance)),
                    EmittedRadiance: new UniformPigment(new Color(1f, 1f, 1f) * emittedRadiance)
                );

                world.addShape(new Sphere(material: enclosureMaterial));

                PathTracer pathTracer = new PathTracer(
                    world: world,
                    pcg: pcg,
                    numOfRays: 1,
                    maxDepth: 100,
                    russianRouletteLimit: 101
                    );

                Ray ray = new Ray(origin: new Point(0f, 0f, 0f), dir: new Vec(1f, 0f, 0f));
                Color color = pathTracer.computeRadiance(ray);
                float expected = emittedRadiance / (1.0f - reflectance);
                Assert.True(Utility.areClose(expected, color.r, epsilon: 1e-3f), $"TestPathTracer failed - Assert i={i}, 1/3");
                Assert.True(Utility.areClose(expected, color.g, epsilon: 1e-3f), $"TestPathTracer failed - Assert i={i}, 2/3");
                Assert.True(Utility.areClose(expected, color.b, epsilon: 1e-3f), $"TestPathTracer failed - Assert i={i}, 3/3");
            }
        }
    }


}