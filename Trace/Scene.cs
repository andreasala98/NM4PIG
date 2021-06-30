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
using System.Linq;

#nullable enable
namespace Trace
{
    /// <summary>
    /// A scene read from a scene file
    /// </summary>
    public class Scene
    {
        public World world = new World();
        public Camera? camera = null;
        public Dictionary<string, Material> materials = new Dictionary<string, Material>();
        public Dictionary<string, float> floatVariables = new Dictionary<string, float>();
        public string[] overriddenVariables = new string[]{};




        public static Vec parseVector(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("[");
            float x = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber(scene);
            inputFile.expectSymbol("]");

            return new Vec(x,y,z);
        }


        public static Color parseColor(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("<");
            float r = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float g = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float b = inputFile.expectNumber(scene);
            inputFile.expectSymbol(">");

            return new Color(r,g,b);
        }


        public static IPigment parsePigment(InputStream inputFile, Scene scene){
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Uniform, KeywordEnum.Checkered, KeywordEnum.Image });
            inputFile.expectSymbol("(");
            IPigment result;


            if (key == KeywordEnum.Uniform)
            {
                Color color = parseColor(inputFile, scene);
                result = new UniformPigment(color: color);
            }
            else if (key == KeywordEnum.Checkered)
            {
                Color col1 = parseColor(inputFile, scene);
                inputFile.expectSymbol(",");
                Color col2 = parseColor(inputFile, scene);
                inputFile.expectSymbol(",");
                // optional parameter?
                int steps = (int)inputFile.expectNumber(scene);
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
                throw new GrammarError(inputFile.location, "This line should be unreachable");
            }

            inputFile.expectSymbol(")");

            return result;

        }


        public static BRDF parseBRDF(InputStream inputFile, Scene scene)
        {
            BRDF result;
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() {KeywordEnum.Diffuse, KeywordEnum.Specular});
            inputFile.expectSymbol("(");
            IPigment pigment = parsePigment(inputFile, scene);
            inputFile.expectSymbol(")");


            if (key == KeywordEnum.Diffuse)
                result = new DiffuseBRDF(pigment);
            else if (key == KeywordEnum.Specular)
                result = new SpecularBRDF(pigment);
            else throw new GrammarError(inputFile.location, "This line should be unreachable");


            return result;

        }


        public static Tuple<string, Material> parseMaterial(InputStream inputFile, Scene scene)
        {
            string name = inputFile.expectIdentifier();

            inputFile.expectSymbol("(");
            BRDF brdf = parseBRDF(inputFile, scene);
            inputFile.expectSymbol(",");
            IPigment emRad = parsePigment(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Tuple<string, Material>(name, new Material(brdf, emRad));
        }


        public static Transformation parseTransformation(InputStream inputFile, Scene scene){

            Transformation result = new Transformation(1);
            List<KeywordEnum> keyList = new List<KeywordEnum>() { KeywordEnum.Identity, KeywordEnum.Translation, KeywordEnum.Scaling,
                                                                  KeywordEnum.RotationX, KeywordEnum.RotationY, KeywordEnum.RotationZ };

            // now we look for transformations until there is no more *
            while (true)
            {      
                KeywordEnum key = inputFile.expectKeywords(keyList);

                if (key == KeywordEnum.Identity) break;
                else if (key == KeywordEnum.Translation)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.Translation(parseVector(inputFile, scene));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationX)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationX(inputFile.expectNumber(scene));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationY)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationY(inputFile.expectNumber(scene));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationZ)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationZ(inputFile.expectNumber(scene));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.Scaling)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.Scaling(parseVector(inputFile, scene));
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

        public static Sphere parseSphere(InputStream inputFile, Scene scene) {

            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();

            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Sphere(tr, scene.materials[matName]);
        }


        public static Plane parsePlane(InputStream inputFile, Scene scene) {

            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();

            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Plane(tr, scene.materials[matName]);
        }

        public static Cylinder parseCylinder(InputStream inputFile, Scene scene)
        {
            // cylinder(material, transformation)
            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();
            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Cylinder();
        }

        public static Camera parseCamera(InputStream inputFile, Scene scene) {

            inputFile.expectSymbol("(");
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Perspective, KeywordEnum.Orthogonal});
            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(",");
            float aspectRatio = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float distance = inputFile.expectNumber(scene);
            inputFile.expectSymbol(")");

            Camera result;

            if (key == KeywordEnum.Perspective) result = new PerspectiveCamera(distance, aspectRatio, tr);
            else if (key == KeywordEnum.Orthogonal) result = new OrthogonalCamera(aspectRatio, tr);

            else throw new GrammarError(inputFile.location, "Unknown Camera type!");

            return result;
        }


        public static Scene parseScene(InputStream inputFile)
        {
            Scene scene = new Scene();
            scene.floatVariables = new Dictionary<string, float>();
            scene.floatVariables.Keys.CopyTo(scene.overriddenVariables,0);

            while (true)
            {
                Token tok = inputFile.readToken();
                string varName = "";

                if (tok is StopToken) break;
                if (tok is not KeywordToken) throw new GrammarError(inputFile.location, $"Expected keyword, got {tok} instead");

                if (((KeywordToken)tok).keyword == KeywordEnum.Float)
                {
                    varName = inputFile.expectIdentifier();
                    SourceLocation varLoc = inputFile.location;
                    inputFile.expectSymbol("(");
                    float varValue = inputFile.expectNumber(scene);
                    inputFile.expectSymbol(")");

                    if (scene.floatVariables.ContainsKey(varName) && !scene.overriddenVariables.Contains(varName))
                    {
                        throw new GrammarError(inputFile.location, $"You cannot redefine variable {varName}");
                    }
                    if (!(scene.overriddenVariables.Contains(varName)))
                    {
                        scene.floatVariables.Add(varName, varValue);
                    }
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Sphere)
                {
                    scene.world.addShape(parseSphere(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Plane)
                {
                    scene.world.addShape(parsePlane(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Camera)
                {
                    if (scene.camera != null)
                    {
                        throw new GrammarError(inputFile.location, "Youn cannot define more than one camera! :'(");
                    }

                    scene.camera = parseCamera(inputFile, scene);
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Material)
                {
                    Tuple<string, Material> t = parseMaterial(inputFile, scene);
                    scene.materials[t.Item1] = t.Item2;
                }
            }

            return scene; // ok
        }

    }
}
