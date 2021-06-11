using System;
using System.IO;
using Trace;
using System.Collections.Generic;
using System.Diagnostics;

using Tsf = Trace.Transformation;
using CC = Trace.Constant;

namespace NM4PIG
{

    class Demo
    {
        public static void ExecuteDemo(int width, int height, int angle, bool orthogonal, string pfmFile,
                                string ldrFile, int scene, float? luminosity, int spp, char rendType)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();
            int sqSpp = (int)Math.Pow((int)Math.Sqrt(spp), 2);

            Console.WriteLine("Starting Demo with these parameters:\n");

            Console.WriteLine("Width: " + width);
            Console.WriteLine("Height: " + height);
            Console.WriteLine("Angle: " + angle);
            Console.WriteLine(orthogonal ? "Orthogonal Camera" : "Perspective Camera");
            Console.WriteLine("pfmFile: " + pfmFile);
            Console.WriteLine("ldrFile: " + ldrFile);
            Console.WriteLine("Samples per pixel: " + sqSpp);
            Console.WriteLine("Render type: " + dictRend[rendType]);

            Console.WriteLine("\n");

            HdrImage image = new HdrImage(width, height);

            // Camera initialization
            Console.WriteLine("Creating the camera...");
            var cameraTransf = Transformation.RotationZ(Utility.DegToRad(angle)) * Transformation.Translation(-2.0f, 0.0f, 0.0f);
            Camera camera;
            if (orthogonal) { camera = new OrthogonalCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }
            else { camera = new PerspectiveCamera(aspectRatio: (float)width / height, transformation: cameraTransf); }

            // Default value on/off renderer
            Render? renderer = null;

            Console.WriteLine("Creating the scene...");
            World world = new World();
            List<float> Vertices = new List<float>() { -0.5f, 0.5f };

            switch (scene)
            {
                case 1:
                    //One sphere for each vertex of the cube
                    foreach (var x in Vertices)
                    {
                        foreach (var y in Vertices)
                        {
                            foreach (var z in Vertices)
                            {
                                world.addShape(new Sphere(Tsf.Translation(new Vec(x, y, z))
                                                    * Tsf.Scaling(new Vec(0.1f, 0.1f, 0.1f))));
                            } // z
                        } // y
                    }// x

                    //Adding two more spheres to break simmetry
                    world.addShape(new Sphere(Tsf.Translation(new Vec(0f, 0f, -0.5f))
                                             * Tsf.Scaling(0.1f)));
                    world.addShape(new Sphere(Tsf.Translation(new Vec(0f, 0.5f, 0f))
                                             * Tsf.Scaling(0.1f)));
                    break;

                case 2:

                    Material material1 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(Constant.Yellow, Constant.Blue)));
                    Material material2 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(Constant.Red, Constant.White)));
                    Material material3 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(Constant.Orange, Constant.Green)));

                    world.addShape(new Cylinder(
                                                //transformation: Tsf.RotationY(MathF.PI / 2f),
                                                material: material1
                                                ));
                    //renderer = new FlatRender(world, new Color(0f, 1f, 1f));
                    break;
                case 3:
                    HdrImage img = new HdrImage();
                    string inputpfm = "Texture/minecraft.pfm";
                    using (FileStream inputStream = File.OpenRead(inputpfm))
                    {
                        img.readPfm(inputStream);
                        Console.WriteLine($"File {inputpfm} has been correctly read from disk.");
                    }

                    world.addShape(
                                    new Box(
                                            transformation: Transformation.Scaling(0.5f),
                                            material: new Material(
                                                                Brdf: new DiffuseBRDF(new ImagePigment(img)),
                                                                EmittedRadiance: new UniformPigment(Constant.Black)
                                                                )
                                )
                                );

                    //renderer = new FlatRender(world, new Color(0f, 1f, 1f));

                    break;

                case 4:
                    HdrImage img2 = new HdrImage();
                    string inputpfm2 = "Texture/diceW.pfm";
                    using (FileStream inputStream = File.OpenRead(inputpfm2))
                    {
                        img2.readPfm(inputStream);
                        Console.WriteLine($"File {inputpfm2} has been correctly read from disk.");
                    }

                    world.addShape(
                                    new Box(
                                            transformation: Transformation.Scaling(0.5f),
                                            material: new Material(
                                                                Brdf: new DiffuseBRDF(new ImagePigment(img2)),
                                                                EmittedRadiance: new UniformPigment(Constant.Black)
                                                                )
                                )
                                );

                    //renderer = new FlatRender(world, new Color(0f, 1f, 1f));

                    break;
                case 5:
                    PCG pcg = new PCG();
                    Material skyMat = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(CC.SkyBlue));
                    Material groundMat = new Material(new DiffuseBRDF(new CheckeredPigment(CC.Black, CC.BroomYellow)), new UniformPigment(CC.Black));
                    Material sph1Mat = new Material(new DiffuseBRDF(new UniformPigment(CC.BlueChill)));
                    Material sph2Mat = new Material(new DiffuseBRDF(new UniformPigment(Color.random())));
                    Material boxMat = new Material(new DiffuseBRDF(new UniformPigment(CC.BrightGreen)));
                    Material refMat = new Material(new SpecularBRDF(new UniformPigment(CC.LightRed)));


                    world.addShape(new Sphere(Tsf.Scaling(500f), skyMat));
                    world.addShape(new Plane(Tsf.Translation(0f, 0f, -1f), groundMat));
                    world.addShape(new CSGUnion(new Sphere(Transformation.Translation(0.5f, -2.6f, 1f) * Transformation.Scaling(0.6f), sph2Mat),
                                                     new Box(new Point(0f, -2.25f, 0.9f), new Point(1f, -3.25f, 1.8f), null, boxMat)));
                    world.addShape(new Sphere(Tsf.Translation(3f, 5f, 1.6f) * Tsf.Scaling(2.0f, 4.0f, 2.0f), refMat));
                    world.addShape(new Sphere(Tsf.Translation(4f, -1f, 1.3f) * Tsf.Scaling(1.0f), sph1Mat));
                    world.addShape(new Sphere(Tsf.Translation(-4f, -0.5f, 1f) * Tsf.Scaling(2f), sph2Mat));
                    //renderer = new PathTracer(world, Constant.Black, pcg);
                    break;
                case 6:
                    world.addShape(new CSGUnion(
                                            new Sphere(
                                                    Transformation.Translation(new Vec(0f, 0f, 1.2f))
                                                    * Transformation.Scaling(0.635f)),
                                            new Box(transformation: Transformation.Scaling(0.5f, 0.5f, 1f))
                                            )
                                    );
                    break;
                case 8:
                    Material cylMat = new Material(new DiffuseBRDF(new UniformPigment(CC.BrightGreen)));
                    Material BrightRedMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(170f / 255, 1f / 255, 20f / 255))));
                    Material BrightBlueMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(0f, 78f / 255, 255f / 255))));
                    Material grndMat = new Material(new DiffuseBRDF(new CheckeredPigment(CC.LightRed, CC.Orange)), new UniformPigment(CC.Black));
                    Material skyMtrl = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(CC.SkyBlue));


                    Shape C1 = new Cylinder(Tsf.Scaling(0.5f, 0.5f, 1.5f), cylMat);
                    Shape C2 = new Cylinder(Tsf.RotationY(Utility.DegToRad(45)) * Tsf.RotationX(CC.PI / 2f) * Tsf.Scaling(0.5f, 0.5f, 1.5f), cylMat);
                    Shape C3 = new Cylinder(Tsf.RotationX(Utility.DegToRad(-45)) * Tsf.RotationY(CC.PI / 2f) * Tsf.Scaling(0.5f, 0.5f, 1.5f), cylMat);

                    Shape S1 = new Sphere(transformation: Tsf.Scaling(1.5f), material: BrightBlueMat);
                    Shape B1 = new Box(material: BrightRedMat);

                    Shape left = S1 - B1;
                    Shape right = (C1 + C2) + C3;

                    world.addShape(left);
                    // world.addShape(new Cylinder(Tsf.Scaling(0.5f, 0.5f, 2f), cylMat));
                    // world.addShape(new Cylinder(Tsf.RotationY(Utility.DegToRad(45))*Tsf.RotationX(CC.PI/2f)*Tsf.Scaling(0.5f, 0.5f, 2f), cylMat));
                    //renderer = new PathTracer(world, Constant.Black, new PCG(), 6);
                    break;
                case 9:
                    Material skyM = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(CC.SkyBlue));
                    Material BRedMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(170f / 255, 1f / 255, 20f / 255))));
                    Material ground = new Material(new DiffuseBRDF(new CheckeredPigment(CC.LightRed, CC.Orange)), new UniformPigment(CC.Black));


                    world.addShape(new Sphere(Tsf.Scaling(500f), skyM));
                    world.addShape(new Cone(r: 0.5f, material: BRedMat, transformation: Transformation.RotationY(Constant.PI / 2f) *
                                                                                Transformation.Scaling(0.5f)));
                    world.addShape(new Plane(Tsf.Scaling(0f, 0f, -1f), ground));


                    break;
                default:
                    break;
            }

            switch (rendType)
            {
                case 'o':
                    renderer = new OnOffRender(world);
                    break;
                case 'f':
                    renderer = new FlatRender(world);
                    break;
                //case 'p':
                // renderer = new PointLightTracer(world);
                //break;
                case 'r':
                    renderer = new PathTracer(world, CC.Black, new PCG());
                    break;
                default:
                    break;
            }

            // Ray tracing
            Console.WriteLine("Rendering the scene...");
            var rayTracer = new ImageTracer(image, camera, (int)Math.Sqrt(sqSpp));

            if (renderer == null) renderer = new OnOffRender(world);

            rayTracer.fireAllRays(renderer);

            // Write PFM image
            Console.WriteLine("Saving in pfm format...");
            using (FileStream outpfmstream = File.OpenWrite(pfmFile))
            {
                image.savePfm(outpfmstream);
                Console.WriteLine($"Image saved in {pfmFile}");
            }

            Convert.ExecuteConvert(pfmFile, ldrFile, Default.factor, Default.gamma, luminosity);

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

        }//Demo


        public static Dictionary<char, string> dictRend = new Dictionary<char, string>(){
            {'o', "On-Off Renderer"},
            {'f',"Flat Renderer"},
            {'p', "Point-Light Tracer"},
            {'r', "Path Tracer"}

        };
    }

}