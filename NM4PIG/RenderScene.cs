
using System;
using System.IO;
using Trace;
using System.Collections.Generic;
using System.Diagnostics;

using CC = Trace.Constant;

namespace NM4PIG
{
    class RenderScene
    {
        public static void ExecuteRender(string file, int width, int height, string pfmFile,
                                string ldrFile, int spp, char rend, Dictionary<string, float> variables,
                                float factor, float gamma, int maxDepth, int nRays, int rrLimit)
        {

            Console.WriteLine($"File describing the scene: {file}");
            Console.WriteLine($"Image size: {width}x{height}");
            Console.WriteLine($"Output PFM-file: {pfmFile}");
            Console.WriteLine($"Output LDR-file: {ldrFile}");
            Console.WriteLine($"Samples-per-pixel (antialiasing): {spp}");
            // Console.WriteLine($"Maximum ray depth: {maxDepth}");
            // Console.WriteLine($"Number of sampled rays: {nRays}");
            // Console.WriteLine($"Russian Roulette Lower Limit: {rrLimit}");
            Console.WriteLine("User-defined overridden variables");
            if (variables.Count == 0) Console.WriteLine("    - No Variables");
            foreach (var item in variables)
            {
                Console.WriteLine($"    - {item.Key} = {item.Value}");
            }

            Scene scene = new Scene();

            using (FileStream inputSceneStream = File.OpenRead(file))
            {
                try
                {
                    scene = Scene.parseScene(inputFile: new InputStream(stream: inputSceneStream, fileName: file), variables: variables);
                }
                catch (GrammarError e)
                {
                    SourceLocation loc = e.sourceLocation;
                    Console.WriteLine($"{loc.fileName}:{loc.lineNum}:{loc.colNum}: {e.Message}");
                    return;
                }
            }

            HdrImage image = new HdrImage(width, height);

            // Run the ray-tracer
            ImageTracer tracer = new ImageTracer(i: image, c: scene.camera, sps: (int)MathF.Sqrt(spp));

            Render renderer;
            if (rend == 'o')
            {
                Console.WriteLine("\nUsing on/off renderer:");
                renderer = new OnOffRender(world: scene.world, background: CC.Black);
            }
            else if (rend == 'f')
            {
                Console.WriteLine("\nUsing flat renderer:");
                renderer = new FlatRender(world: scene.world, background: CC.Black);
            }
            else if (rend == 'r')
            {
                Console.WriteLine("\nUsing a path tracer:");
                renderer = new PathTracer(world: scene.world, numOfRays: nRays, maxDepth: maxDepth, russianRouletteLimit: rrLimit);
                Console.WriteLine($">>>> Max depth: {((PathTracer)renderer).maxDepth}");
                Console.WriteLine($">>>> Russian Roulette Limit: {((PathTracer)renderer).russianRouletteLimit}");
                Console.WriteLine($">>>> Number of rays: {((PathTracer)renderer).numOfRays}");
            }
            else if (rend == 'p')
            {
                Console.WriteLine("\nUsing a point-light tracer: ");
                renderer = new PointLightRender(world: scene.world, background: CC.Black);
                Console.WriteLine($">> Ambient color: {((PointLightRender)renderer).ambientColor}");
            }
            else
            {
                Console.WriteLine($"Unknown renderer: {rend}");
                return;
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            tracer.fireAllRays(renderer);

            Console.WriteLine("Saving in pfm format...");
            using (FileStream outpfmstream = File.OpenWrite(pfmFile))
            {
                image.savePfm(outpfmstream);
                Console.WriteLine($"Image saved in {pfmFile}");
            }

            Convert.ExecuteConvert(pfmFile, ldrFile, factor, gamma, null);

            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("\nRun Time: " + elapsedTime);

            Console.WriteLine("See you next time!\n");
        } //RenderScene

    } //Main Funcs

} //NM4PIG