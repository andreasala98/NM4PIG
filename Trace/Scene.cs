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
        public HashSet<string> overriddenVariables = new HashSet<string>();


        public static Vec parseVector(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("[");
            float x = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float y = inputFile.expectNumber(scene);
            inputFile.expectSymbol(",");
            float z = inputFile.expectNumber(scene);
            inputFile.expectSymbol("]");

            return new Vec(x, y, z);
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

            return new Color(r, g, b);
        }


        public static IPigment parsePigment(InputStream inputFile, Scene scene)
        {
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
                using (Stream imageStream = File.OpenRead(fileName))
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
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Diffuse, KeywordEnum.Specular });
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


        public static Transformation parseTransformation(InputStream inputFile, Scene scene)
        {

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
                    result = result * Transformation.RotationX(Utility.DegToRad((int)inputFile.expectNumber(scene)));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationY)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationY(Utility.DegToRad((int)inputFile.expectNumber(scene)));
                    inputFile.expectSymbol(")");
                }
                else if (key == KeywordEnum.RotationZ)
                {
                    inputFile.expectSymbol("(");
                    result = result * Transformation.RotationZ(Utility.DegToRad((int)inputFile.expectNumber(scene)));
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

        public static Sphere parseSphere(InputStream inputFile, Scene scene)
        {

            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();

            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Sphere(tr, scene.materials[matName]);
        }


        public static Plane parsePlane(InputStream inputFile, Scene scene)
        {

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

            return new Cylinder(tr, scene.materials[matName]);
        }

        public static Box parseBox(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();
            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Box(new Point(-1f, -1f, -1f), new Point(1f, 1f, 1f), tr, scene.materials[matName]);
        }

        public static Cone parseCone(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("(");
            string matName = inputFile.expectIdentifier();
            if (!scene.materials.ContainsKey(matName)) throw new GrammarError(inputFile.location, $"{matName} is unknown material");

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new Cone(1f, 1f, tr, scene.materials[matName]);
        }

        public static CSGUnion parseCSGUnion(InputStream inputFile, Scene scene)
        {
            List<KeywordEnum> allowedKeys = new List<KeywordEnum>(){ KeywordEnum.Plane, KeywordEnum.Sphere,
                                                                     KeywordEnum.Box, KeywordEnum.Cylinder,
                                                                     KeywordEnum.Cone, KeywordEnum.CSGUnion,
                                                                     KeywordEnum.CSGIntersection, KeywordEnum.CSGDifference};

            Shape lShape, rShape;
            inputFile.expectSymbol("(");
            KeywordEnum lShapeKey = inputFile.expectKeywords(allowedKeys);
            switch (lShapeKey)
            {
                case KeywordEnum.Plane:
                    lShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    lShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    lShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    lShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    lShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    lShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    lShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    lShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            KeywordEnum rShapeKey = inputFile.expectKeywords(allowedKeys);

            switch (rShapeKey)
            {
                case KeywordEnum.Plane:
                    rShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    rShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    rShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    rShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    rShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    rShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    rShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    rShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new CSGUnion(lShape, rShape, tr);
        }

        public static CSGDifference parseCSGDifference(InputStream inputFile, Scene scene)
        {
            List<KeywordEnum> allowedKeys = new List<KeywordEnum>(){ KeywordEnum.Plane, KeywordEnum.Sphere,
                                                                                KeywordEnum.Box, KeywordEnum.Cylinder,
                                                                                KeywordEnum.Cone, KeywordEnum.CSGUnion,
                                                                                KeywordEnum.CSGIntersection, KeywordEnum.CSGDifference};

            Shape lShape, rShape;
            inputFile.expectSymbol("(");
            KeywordEnum lShapeKey = inputFile.expectKeywords(allowedKeys);
            switch (lShapeKey)
            {
                case KeywordEnum.Plane:
                    lShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    lShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    lShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    lShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    lShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    lShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    lShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    lShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            KeywordEnum rShapeKey = inputFile.expectKeywords(allowedKeys);

            switch (rShapeKey)
            {
                case KeywordEnum.Plane:
                    rShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    rShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    rShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    rShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    rShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    rShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    rShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    rShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new CSGDifference(lShape, rShape, tr);
        }

        public static CSGIntersection parseCSGIntersection(InputStream inputFile, Scene scene)
        {
            List<KeywordEnum> allowedKeys = new List<KeywordEnum>(){ KeywordEnum.Plane, KeywordEnum.Sphere,
                                                                    KeywordEnum.Box, KeywordEnum.Cylinder,
                                                                      KeywordEnum.Cone, KeywordEnum.CSGUnion,
                                                                    KeywordEnum.CSGIntersection, KeywordEnum.CSGDifference};

            Shape lShape, rShape;
            inputFile.expectSymbol("(");
            KeywordEnum lShapeKey = inputFile.expectKeywords(allowedKeys);
            switch (lShapeKey)
            {
                case KeywordEnum.Plane:
                    lShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    lShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    lShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    lShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    lShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    lShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    lShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    lShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            KeywordEnum rShapeKey = inputFile.expectKeywords(allowedKeys);

            switch (rShapeKey)
            {
                case KeywordEnum.Plane:
                    rShape = parsePlane(inputFile, scene);
                    break;
                case KeywordEnum.Sphere:
                    rShape = parseSphere(inputFile, scene);
                    break;
                case KeywordEnum.Box:
                    rShape = parseBox(inputFile, scene);
                    break;
                case KeywordEnum.Cylinder:
                    rShape = parseCylinder(inputFile, scene);
                    break;
                case KeywordEnum.Cone:
                    rShape = parseCone(inputFile, scene);
                    break;
                case KeywordEnum.CSGUnion:
                    rShape = parseCSGUnion(inputFile, scene);
                    break;
                case KeywordEnum.CSGDifference:
                    rShape = parseCSGDifference(inputFile, scene);
                    break;
                case KeywordEnum.CSGIntersection:
                    rShape = parseCSGIntersection(inputFile, scene);
                    break;
                default:
                    throw new GrammarError(inputFile.location, $"Shape not found at {inputFile.location.ToString()}");

            }

            inputFile.expectSymbol(",");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            return new CSGIntersection(lShape, rShape, tr);
        }

        public static Shape parseWikiShape(InputStream inputFile, Scene scene)
        {
            inputFile.expectSymbol("(");
            Transformation tr = parseTransformation(inputFile, scene);
            inputFile.expectSymbol(")");

            Shape result = Constant.wikiShape(tr);
            return result;

        }

        public static PointLight parsePointlight(InputStream inputFIle, Scene scene)
        {
            // pointlight()
            inputFIle.expectSymbol("(");
            Point p = parseVector(inputFIle, scene).ToPoint();
            inputFIle.expectSymbol(",");
            Color col = parseColor(inputFIle, scene);
            inputFIle.expectSymbol(")");

            return new PointLight(p, col);

        }

        public static Camera parseCamera(InputStream inputFile, Scene scene)
        {

            inputFile.expectSymbol("(");
            KeywordEnum key = inputFile.expectKeywords(new List<KeywordEnum>() { KeywordEnum.Perspective, KeywordEnum.Orthogonal });
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


        public static Scene parseScene(InputStream inputFile, Dictionary<string, float> variables)
        {
            Scene scene = new Scene();
            scene.floatVariables = variables;
            scene.overriddenVariables = scene.floatVariables.Keys.ToHashSet();


            while (true)
            {
                Token tok = inputFile.readToken();
                string varName = "";

                if (tok is StopToken) {Console.WriteLine("StopToken encountered");  break; }

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
                else if (((KeywordToken)tok).keyword == KeywordEnum.Box)
                {
                    scene.world.addShape(parseBox(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Cylinder)
                {
                    scene.world.addShape(parseCylinder(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Cone)
                {
                    scene.world.addShape(parseCone(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.CSGUnion)
                {
                    scene.world.addShape(parseCSGUnion(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.CSGIntersection)
                {
                    scene.world.addShape(parseCSGIntersection(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.CSGDifference)
                {
                    scene.world.addShape(parseCSGDifference(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Wikishape)
                {
                    scene.world.addShape(parseWikiShape(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Pointlight)
                {
                    scene.world.addPointLight(parsePointlight(inputFile, scene));
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Camera)
                {
                    if (scene.camera != null)
                    {
                        throw new GrammarError(inputFile.location, "You cannot define more than one camera! :'(");
                    }
                    scene.camera = parseCamera(inputFile, scene);
                }
                else if (((KeywordToken)tok).keyword == KeywordEnum.Material)
                {
                    Tuple<string, Material> t = parseMaterial(inputFile, scene);
                    scene.materials[t.Item1] = t.Item2;
                }
                if (tok is KeywordToken)
                Console.WriteLine("Correctly parsed "  + ((KeywordToken)tok).keyword);
            }
            return scene;
        }
    }
}
