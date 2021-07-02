
using System;
using System.IO;
using Trace;
using System.Collections.Generic;
using System.Diagnostics;

using Tsf = Trace.Transformation;
using CC = Trace.Constant;

namespace NM4PIG 
{
    class MainFuncs
    {
        public static void Demo(int width, int height, int angle, bool orthogonal, string pfmFile, 
                                string ldrFile, int scene, float? luminosity, int spp)
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

            Console.WriteLine("\n");

            HdrImage image = new HdrImage(width, height);

            // Camera initialization
            Console.WriteLine("Creating the camera...");
            var cameraTransf = Transformation.RotationZ(Utility.DegToRad(angle)) * Transformation.Translation(-1.0f, 0.0f, 0.0f);
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
                                world.addShape(new Sphere(transformation: Transformation.Translation(new Vec(x, y, z))
                                                    * Transformation.Scaling(new Vec(0.1f, 0.1f, 0.1f))));
                            } // z
                        } // y
                    }// x

                    //Adding two more spheres to break simmetry
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0f, -0.5f))
                                             * Transformation.Scaling(0.1f)));
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0.5f, 0f))
                                             * Transformation.Scaling(0.1f)));
                    break;

                case 2:

                    Material material1 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(CC.Yellow, Constant.Blue)));
                    Material material2 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(Constant.Red, Constant.White)));
                    Material material3 = new Material(Brdf: new DiffuseBRDF(new CheckeredPigment(Constant.Orange, Constant.Green)));

                    //One sphere for each vertex of the cube
                    foreach (var x in Vertices)
                    {
                        foreach (var y in Vertices)
                        {
                            foreach (var z in Vertices)
                            {
                                world.addShape(new Sphere(transformation: Transformation.Translation(new Vec(x, y, z))
                                                    * Transformation.Scaling(new Vec(0.1f, 0.1f, 0.1f)),
                                                    material1));
                            } // z
                        } // y
                    }// x

                    //Adding two more spheres to break simmetry
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0f, -0.5f))
                                             * Transformation.Scaling(0.1f), material2));
                    world.addShape(new Sphere(Transformation.Translation(new Vec(0f, 0.5f, 0f))
                                             * Transformation.Scaling(0.1f), material3));

                    renderer = new FlatRender(world);
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

                    renderer = new FlatRender(world, new Color(0f, 1f, 1f));

                    break;

                case 4:

                    Material skyMatr = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(CC.SkyBlue));
                    Material gndMat = new Material(new DiffuseBRDF(new CheckeredPigment(CC.White, CC.Black)), new CheckeredPigment(CC.White, CC.Black));
                    Material pigMat = new Material(new DiffuseBRDF(new UniformPigment(CC.PiggyPink)));

                    world.addShape(new Sphere(Tsf.Scaling(150f), skyMatr));
                    world.addShape(new Plane(Tsf.Translation(0f, 0f, -1f), gndMat));
                    world.addShape(CC.Body);
                    

                    renderer = new PathTracer(world: world,background: new Color(0.2f, 0.2f, 0.2f), pcg: new PCG());

                    break;
                case 5:
                    PCG pcg = new PCG();
                    Material skyMat = new Material(new DiffuseBRDF(new UniformPigment(CC.SkyBlue)), new UniformPigment(new Color(0.5294117647f, 0.80784313725f, 0.92156862745f)));
                    Material groundMat = new Material(new DiffuseBRDF(new CheckeredPigment(CC.Black, CC.BroomYellow)), new UniformPigment(CC.Black));
                    Material sph1Mat = new Material(new DiffuseBRDF(new UniformPigment(CC.BlueChill)));
                    Material sph2Mat = new Material(new DiffuseBRDF(new UniformPigment(Color.random())));
                    Material boxMat = new Material(new DiffuseBRDF(new UniformPigment(CC.BrightGreen)));
                    Material refMat = new Material(new SpecularBRDF(new UniformPigment(CC.LightRed)));


                    world.addShape(new Sphere(Tsf.Scaling(500f), skyMat));
                    world.addShape(new Plane(Tsf.Translation(0f, 0f, -1f), groundMat));
                    world.addShape(new CSGUnion(new Sphere(Transformation.Translation(0.5f,-2.6f,1f)*Transformation.Scaling(0.6f), sph2Mat),
                                                     new Box(new Point(0f,-2.25f,0.9f), new Point(1f,-3.25f,1.8f), null, boxMat)));
                    world.addShape(new Sphere(Tsf.Translation(3f, 5f, 1.6f) * Tsf.Scaling(2.0f,4.0f,2.0f), refMat));
                    world.addShape(new Sphere(Tsf.Translation(4f, -1f, 1.3f) * Tsf.Scaling(1.0f), sph1Mat));
                    world.addShape(new Sphere(Tsf.Translation(-4f, -0.5f, 1f) * Tsf.Scaling(2f), sph2Mat));
                    renderer = new PathTracer(world, Constant.Black, pcg);
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

            Convert(pfmFile, ldrFile, Default.factor, Default.gamma, luminosity);

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

        }//Demo


        public static void Convert(string inputpfm, string outputldr, float factor, float gamma, float? luminosity)
        {
            string fmt = outputldr.Substring(outputldr.Length - 3, 3);

            Console.WriteLine("\n\nStarting file conversion using these parameters:\n");

            Console.WriteLine("pfmFile: " + inputpfm);
            Console.WriteLine("ldrFile: " + outputldr);
            Console.WriteLine("Format: " + fmt);
            Console.WriteLine("Factor: " + factor);
            Console.WriteLine("Gamma: " + gamma);
            Console.WriteLine(luminosity.HasValue ? ("Manual luminosity: " + luminosity) : "Average luminosity");

            Console.WriteLine("\n");

            HdrImage myImg = new HdrImage();

            try
            {
                using (FileStream inputStream = File.OpenRead(inputpfm))
                {
                    myImg.readPfm(inputStream);
                    Console.WriteLine($"File {inputpfm} has been correctly read from disk.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            Console.WriteLine("Starting Tone Mapping...");
            try
            {
                Console.WriteLine("Normalizing image...");

                if (luminosity.HasValue) myImg.normalizeImage(factor, luminosity.Value);
                else myImg.normalizeImage(factor);

                Console.WriteLine("Clamping image...");
                myImg.clampImage();

                Console.WriteLine("Saving LDR image...");
                myImg.writeLdrImage(outputldr, fmt, gamma);

                Console.WriteLine($"File {outputldr} has been correctly written to disk.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

        } //Convert

    } //Main Funcs

} //NM4PIG