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
using System.Collections.Generic;

#nullable enable
namespace Trace
{
    /// <summary>
    /// A scene read from a scene file
    /// </summary>
    public class Scene
    {
        public World world;
        public Camera? camera = null;
        public Dictionary<string, Material> materials;
        public Dictionary<string, float> floatVariables;

        public HashSet<string> overriddenVaribles;




        public Vec parseVector(InputStream inputFile)
        {
            inputFile.expectSymbol("[");
            float x = inputFile.expectNumber(this);
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber(this);
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber(this);
            inputFile.expectSymbol("]");

            return new Vec(x,y,z);
        }


        public Color parseColor(InputStream inputFile)
        {
            inputFile.expectSymbol("<");
            float x = inputFile.expectNumber(this);
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber(this);
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber(this);
            inputFile.expectSymbol(">");

            return new Color(x, y, z);
        }


        public IPigment parsePigment(InputStream inputFile){
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image });
            inputFile.expectSymbol("(");
            IPigment result;


            if (key == KeywordEnum.Uniform)
            {
                Color color = parseColor(inputFile);
                result = new UniformPigment(color: color);
            }
            else if (key == KeywordEnum.Checkered)
            {
                Color col1 = parseColor(inputFile);
                inputFile.expectSymbol(",");
                Color col2 = parseColor(inputFile);
                inputFile.expectSymbol(",");
                // optiona parameter?
                int steps = (int)inputFile.expectNumber(this);
                result = new CheckeredPigment(col1, col2, steps);

            }
            else if (key == KeywordEnum.Image)
            {
                string fileName = inputFile.expectString();
                using (Stream imageStream = File.OpenWrite(fileName))
                {
                    HdrImage img = new HdrImage(imageStream);
                    result = new ImagePigment(img);
                }
                
            }
            else
            {
                throw new ParsingError("This line should be unreachable");
            }

            inputFile.expectSymbol(")");

            return result;

        }


        public BRDF parseBRDF(InputStream inputFile)
        {
            BRDF result;
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() {KeywordEnum.Diffuse, KeywordEnum.Specular});
            inputFile.expectSymbol("(");
            IPigment pigment = parsePigment(inputFile);
            inputFile.expectSymbol(")");


            if (key == KeywordEnum.Diffuse)
                result = new DiffuseBRDF(pigment);
            else if (key == KeywordEnum.Specular)
                result = new DiffuseBRDF(pigment);
            else throw new ParsingError("This line should be unreachable");


            return result;

        }


        public Tuple<string, Material> parseMaterial(InputStream inputFile)
        {
            string name = inputFile.expectIdentifier();

            inputFile.expectSymbol("(");
            BRDF brdf = parseBRDF(inputFile);
            inputFile.expectSymbol(",");
            IPigment emRad = parsePigment(inputFile);
            inputFile.expectSymbol(")");

            return new Tuple<string, Material>(name, new Material(brdf, emRad));
        }


        public Transformation parseTransformation(InputStream inputFile){

            Transformation result = new Transformation(1);
            List<KeywordEnum> keyList = new List<KeywordEnum>() { KeywordEnum.Identity, KeywordEnum.Translation, KeywordEnum.Scaling,
                                                                  KeywordEnum.RotationX, KeywordEnum.RotationY, KeywordEnum.RotationZ };

            // now we look for transformation until there is no more *
            while (true)
            {      
                KeywordEnum key = inputFile.expectKeywords(keyList);

                if (key == KeywordEnum.Identity) continue;
                else if (key == KeywordEnum.Translation)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.Translation(parseVector(inputFile));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationX)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationX(inputFile.expectNumber(this));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationY)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationY(inputFile.expectNumber(this));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationZ)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationZ(inputFile.expectNumber(this));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.Scaling)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.Scaling(parseVector(inputFile));
                    inputFile.expectSymbol(")");
                }


                Token nextKey = inputFile.readToken();

                if (!(nextKey is SymbolToken) || ((SymbolToken)nextKey).symbol != "*")
                {
                    inputFile.unreadToken(nextKey);
                    break;
                }

            } //while

            return result;
        } //parseTranformation

        public Sphere parseSphere(InputStream inputFile) {

            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();

            if (!this.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile);
            inputFile.expectSymbol(")");

            return new Sphere(tr, this.materials[matName]);
        }


        public Plane parsePlane(InputStream inputFile) {

            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();

            if (!this.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile);
            inputFile.expectSymbol(")");

            return new Plane(tr, this.materials[matName]);
        }

        public Camera parseCamera(InputStream inputFile) {

            inputFile.expectSymbol("(");
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Perspective, KeywordEnum.Orthogonal});
            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile);
            inputFile.expectSymbol(",");
            float aspectRatio = inputFile.expectNumber(this);
            inputFile.expectSymbol(",");
            float distance = inputFile.expectNumber(this);
            inputFile.expectSymbol(")");

            Camera result;

            if (key == KeywordEnum.Perspective) result = new PerspectiveCamera(distance, aspectRatio, tr);
            else if (key == KeywordEnum.Orthogonal) result = new OrthogonalCamera(aspectRatio, tr);

            else throw new GrammarError(inputFile.location, "Unknown Camera type!");

            return result;
        }


        public Scene parseScene(InputStream inputFile, Dictionary<string, float> vars)
        {
            Scene scene = new Scene();
            scene.floatVariables = vars;
            scene.over
        }

    }
}
