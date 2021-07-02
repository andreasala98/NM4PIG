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

            Console.WriteLine("Width: " + width + " pixels");
            Console.WriteLine("Height: " + height + " pixels");
            Console.WriteLine("Angle: " + angle + " degrees");
            Console.WriteLine(orthogonal ? "Orthogonal Camera" : "Perspective Camera");
            Console.WriteLine("pfmFile: " + pfmFile);
            Console.WriteLine("ldrFile: " + ldrFile);
            Console.WriteLine("Samples per pixel: " + sqSpp);
            Console.WriteLine("Render type: " + dictRend[rendType]);

            Console.WriteLine("\n");

            HdrImage image = new HdrImage(width, height);

            // Camera initialization
            Console.WriteLine("Creating the camera...");
            var cameraTransf = Transformation.RotationZ(Utility.DegToRad(angle)) * Transformation.Translation(-2.0f, 0.0f, 0.5f) * Tsf.RotationY(Utility.DegToRad(15));
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
                    HdrImage img = new HdrImage();
                    string inputpfm = "Texture/minecraft.pfm";
                    using (FileStream inputStream = File.OpenRead(inputpfm))
                    {
                        img.readPfm(inputStream);
                        Console.WriteLine($"Texture {inputpfm} has been correctly read from disk.");
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

                    break;

                case 3:
                    PCG pcg = new PCG();
                    Material sph1Mat = new Material(new DiffuseBRDF(new UniformPigment(CC.BlueChill)));
                    Material sph2Mat = new Material(new DiffuseBRDF(new UniformPigment(Color.random())));
                    Material boxMat = new Material(new DiffuseBRDF(new UniformPigment(CC.BrightGreen)));


                    world.addShape(new Sphere(Tsf.Scaling(500f), CC.skyMat));
                    world.addShape(new Plane(Tsf.Translation(0f, 0f, -1f), CC.groundMat));
                    world.addShape(new CSGUnion(new Sphere(Transformation.Translation(0.5f, -2.6f, 1f) * Transformation.Scaling(0.6f), sph2Mat),
                                                     new Box(new Point(0f, -2.25f, 0.9f), new Point(1f, -3.25f, 1.8f), null, boxMat)));
                    world.addShape(new Sphere(Tsf.Translation(3f, 5f, 1.6f) * Tsf.Scaling(2.0f, 4.0f, 2.0f), CC.refMat));
                    world.addShape(new Sphere(Tsf.Translation(4f, -1f, 1.3f) * Tsf.Scaling(1.0f), sph1Mat));
                    world.addShape(new Sphere(Tsf.Translation(-4f, -0.5f, 1f) * Tsf.Scaling(2f), sph2Mat));

                    break;
                case 4:
                    Material mat = new Material(null, new UniformPigment(new Color(10f, 10f, 10f)));
                    world.addShape(CC.SKY);
                    world.addShape(new Plane(Tsf.Scaling(-3f, 0f, 0f) * Tsf.RotationY(Utility.DegToRad(270)), mat));

                    world.addShape(CC.wikiShape(Tsf.Scaling(0.75f)));

                    break;
                case 5:
                    Material skyM = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(CC.SkyBlue));
                    Material checkered = new Material(new DiffuseBRDF(new CheckeredPigment(CC.Blue, CC.Yellow)), new UniformPigment(CC.Black));
                    Material ground = new Material(new DiffuseBRDF(new CheckeredPigment(CC.LightRed, CC.Orange)), new UniformPigment(CC.Black));


                    world.addShape(new Sphere(Tsf.Scaling(500f), skyM));
                    world.addShape(new Cylinder(Tsf.Translation(0f, 2f, -0.5f) * Tsf.Scaling(0.5f), checkered));
                    world.addShape(new Cone(r: 0.5f, material: checkered));
                    world.addShape(new Plane(Tsf.Translation(0f, 0f, -1f), ground));

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