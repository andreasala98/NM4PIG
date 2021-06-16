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
using System.Collections.Generic;

namespace Trace
{
    /// <summary>
    /// A scene read from a scene file
    /// </summary>
    public class Scene
    {
        public World world;

        public Camera camera;

        


        public Vec parseVector(InputStream inputFile)
        {
            inputFile.expectSymbol("[");
            float x = inputFile.expectNumber();
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber();
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber();
            inputFile.expectSymbol("]");

            return new Vec(x,y,z);
        }


        public Color parseColor(InputStream inputFile)
        {
            inputFile.expectSymbol("<");
            float x = inputFile.expectNumber();
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber();
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber();
            inputFile.expectSymbol(">");

            return new Color(x, y, z);
        }


        public IPigment parsePigment(InputStream inputFile){
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image });
            inputFile.expectSymbol("(");
            if (key == KeywordEnum.Uniform)
            {
                Color color = parseColor(inputFile);
                IPigment result = new UniformPigment(color: color);
            }
            else if (key == KeywordEnum.Checkered)
            {
                Color col1 = parseColor(inputFile);
                inputFile.expectSymbol(",");
                Color col2 = parseColor(inputFile);
                inputFile.expectSymbol(",");
                // optiona parameter?
                int steps = (int)inputFile.expectNumber();
                IPigment result = new CheckeredPigment(col1, col2, steps);

            }
            else if (key == KeywordEnum.Image)
            {
                string fileName = inputFile.expectString();
                using (FileStream imageStream = FileStream.OpenRead())
                {}
            }

        }

    }
}