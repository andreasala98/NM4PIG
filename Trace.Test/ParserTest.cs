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
using System.IO;
using System.Linq;
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
                camera(perspective, rotationZ(30) * translation([-4, 0, 1]), 1.0, 2.0)

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
    
                plane (sky_material, translation([0, 0, 100]) * rotationY(clock))
                
    
                sphere(sphere_material, translation([0, 0, 1]))
    
                ";

            byte[] byteArray = Encoding.ASCII.GetBytes(test);
            MemoryStream stream = new MemoryStream(byteArray);
            InputStream inputStream = new InputStream(stream);
            Scene scene = Scene.parseScene(inputStream);

            Assert.True( scene.floatVariables.Count == 1, "TestParser failed! Assert 1");
            Assert.True( scene.floatVariables.ContainsKey("clock"), "TestParser failed! Assert 2");
            Assert.True( scene.floatVariables["clock"]==150.0f, "TestParser failed! Assert 3");

        }
    }
}