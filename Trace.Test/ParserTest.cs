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

using System.IO;
using System.Text;
using System.Collections.Generic;
using Xunit;

namespace Trace.Test
{
    public class ParserTest
    {
        [Fact]
        public void TestParser()
        {
            string test = @" 
                float clock(150)
                camera(perspective, rotation_z(30) * translation([-4, 0, 1]), 1.0, 2.0)

                material sky_material(
                 diffuse(uniform(<0, 0, 0>)),
                 uniform(<0.7, 0.5, 1>)
                ) 
                
                # here is a comment
                
                        material ground_material(
                            diffuse(checkered(<0.3, 0.5, 0.1>,
                                                <0.1, 0.2, 0.5>, 4)),
                            uniform(<0, 0, 0>)
                        )
    
                material sphere_material(
                            specular(uniform(<0.5, 0.5, 0.5>)),
                            uniform(<0, 0, 0>)
                                        )
    
                plane (sky_material, translation([0, 0, 100]) * rotation_y(clock))
                plane (ground_material, identity)
                # hi
                sphere(sphere_material, translation([0, 0, 1]))
    
                ";

            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);
            InputStream inputStream = new InputStream(stream);
            Scene scene = Scene.parseScene(inputStream, new Dictionary<string, float>());

            // variables

            Assert.True(scene.floatVariables.Count == 1, "TestParser failed! Assert 1");
            Assert.True(scene.floatVariables.ContainsKey("clock"), "TestParser failed! Assert 2");
            Assert.True(scene.floatVariables["clock"] == 150.0f, "TestParser failed! Assert 3");

            // materials

            Assert.True(scene.materials.Count == 3, "TestParserFailed! Assert 4");
            Assert.True(scene.materials.ContainsKey("sphere_material"), "TestParser failed! Assert 5");
            Assert.True(scene.materials.ContainsKey("sky_material"), "TestParser failed! Assert 6");
            Assert.True(scene.materials.ContainsKey("ground_material"), "TestParser failed! Assert 7");

            Material sphMat = scene.materials["sphere_material"];
            Material skyMat = scene.materials["sky_material"];
            Material gndMat = scene.materials["ground_material"];

            Assert.True(sphMat.brdf is SpecularBRDF, $"TestParser failed! Assert 8 (SpecularBrdf is not {sphMat.brdf.GetType()})");
            Assert.True(skyMat.brdf is DiffuseBRDF, "TestParser failed! Assert 9");
            Assert.True(gndMat.brdf is DiffuseBRDF, "TestParser failed! Assert 10");


            Assert.True(sphMat.brdf.pigment is UniformPigment, "TestParser failed! Assert 11");
            Assert.True(skyMat.brdf.pigment is UniformPigment, "TestParser failed! Assert 12");
            Assert.True(gndMat.brdf.pigment is CheckeredPigment, "TestParser failed! Assert 13");

            Assert.True(((UniformPigment)sphMat.brdf.pigment).c.isClose(new Color(0.5f, 0.5f, 0.5f)), "TestParser failed! Assert 14");
            Assert.True(((CheckeredPigment)gndMat.brdf.pigment).color1.isClose(new Color(0.3f, 0.5f, 0.1f)), "TestParser failed! Assert 15");
            Assert.True(((CheckeredPigment)gndMat.brdf.pigment).color2.isClose(new Color(0.1f, 0.2f, 0.5f)), "TestParser failed! Assert 16");
            Assert.True(((CheckeredPigment)gndMat.brdf.pigment).nSteps == 4, "TestParser failed! Assert 17");
            Assert.True(((UniformPigment)skyMat.emittedRadiance).c.isClose(new Color(0.7f, 0.5f, 1f)), "TestParser failed! Assert 18");

            Assert.True(skyMat.emittedRadiance is UniformPigment, "TestParser failed! Assert 19");
            Assert.True(gndMat.emittedRadiance is UniformPigment, "TestParser failed! Assert 20");
            Assert.True(sphMat.emittedRadiance is UniformPigment, "TestParser failed! Assert 21");

            Assert.True(((UniformPigment)gndMat.emittedRadiance).c.isClose(new Color(0f, 0f, 0f)), "TestParser failed! Assert 22");
            Assert.True(((UniformPigment)sphMat.emittedRadiance).c.isClose(new Color(0f, 0f, 0f)), "TestParser failed! Assert 23");


            // Shapes

            Assert.True(scene.world.shapes.Count == 3, "TestParser failed! Assert 24");
            Assert.True(scene.world.shapes[0] is Plane, "TestParser failed! Assert 25");
            Assert.True(scene.world.shapes[1] is Plane, "TestParser failed! Assert 26");
            Assert.True(scene.world.shapes[2] is Sphere, "TestParser failed! Assert 27");

            Transformation tr1 = Transformation.Translation(new Vec(0f, 0f, 100f)) * Transformation.RotationY(Utility.DegToRad(150));
            Transformation tr2 = Transformation.Translation(new Vec(0f, 0f, 1f));

            Assert.True(scene.world.shapes[0].transformation.isClose(tr1),
             $"TestParser failed! Assert 28");
            Assert.True(scene.world.shapes[1].transformation.isClose(new Transformation(1)), "TestParser failed! Assert 29");
            Assert.True(scene.world.shapes[2].transformation.isClose(tr2), "TestParser failed! Assert 30");

            // Camera

            Assert.True(scene.camera is PerspectiveCamera, "TestParser failed! Assert 31");
            Assert.True(scene.camera.transformation.isClose(Transformation.RotationZ(Utility.DegToRad(30)) * Transformation.Translation(new Vec(-4f, 0f, 1f))), "TestParser failed! Assert 32");
            Assert.True(Utility.areClose(scene.camera.aspectRatio, 1f), "TestParser failed! Assert 33");
            Assert.True(Utility.areClose(((PerspectiveCamera)scene.camera).screenDistance, 2f), "TestParser failed! Assert 34");
        }

        [Fact]
        void TestUndefinedMaterial()
        {
            string test = @" 
                material sky_material(
                 diffuse(uniform(<0, 0, 0>)),
                 uniform(<0.7, 0.5, 1>)
                ) 

                sphere(non_existing_material, translation([0, 0, 1]))
                ";

            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);
            InputStream inputStream = new InputStream(stream);

            try
            {
                Scene scene = Scene.parseScene(inputStream, new Dictionary<string, float>());
                Assert.False(true, "The test did not throw an exception");
            }
            catch (GrammarError)
            {
                // if we land here it's ok
            }
            return;
        }

        [Fact]
        void TestMultipleCamera()
        {
            string test = @" 
                camera(perspective, rotationZ(30) * translation([-4, 0, 1]), 1.0, 2.0)
                camera(orthogonal, identity, 1.0, 2.0)
                ";

            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);
            InputStream inputStream = new InputStream(stream);
            try
            {
                Scene scene = Scene.parseScene(inputStream, new Dictionary<string, float>());
                Assert.False(true, "The test did not throw an exception");
            }
            catch (GrammarError)
            {
                // if we land here it's ok
            }
            return;

        }


    }
}
