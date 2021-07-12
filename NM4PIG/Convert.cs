
using System;
using System.IO;
using Trace;
using System.Collections.Generic;
using System.Diagnostics;

using Tsf = Trace.Transformation;
using CC = Trace.Constant;

namespace NM4PIG
{

    class Convert
    {

        public static void ExecuteConvert(string inputpfm, string outputldr, float factor, float gamma, float? luminosity)
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
                Console.WriteLine(">>>> Normalizing image...");

                if (luminosity.HasValue) myImg.normalizeImage(factor, luminosity.Value);
                else myImg.normalizeImage(factor);

                Console.WriteLine(">>>> Clamping image...");
                myImg.clampImage();

                Console.WriteLine(">>>> Saving LDR image...");
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