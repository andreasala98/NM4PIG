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
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Trace
{
    /// <summary>
    /// Value type used to handle images in HDR format. 
    /// This means every pixel is associated with three floating-point vlaues.
    /// Use this struct to read a PFM file, perform tone mapping,
    /// gamma correction, and convert the image to png/jpg.
    /// </summary>
    public struct HdrImage
    {
        /// <summary>
        /// Widht of the image (number of pixels)
        /// </summary>
        public int width;

        /// <summary>
        /// Height of the image (number of pixels)
        /// </summary>
        public int height;

        /// <summary>
        /// List of (width * height) pixels
        /// </summary>
        public List<Color> pixel;

        /// <summary>
        /// Constructor with the width and heigth of the image. 
        /// It sets all the pixels to black, i.e. RGB=(0,0,0)
        /// </summary>
        /// <param name="x">Width of the image</param>
        /// <param name="y">Height of the image</param>
        public HdrImage(int x, int y)
        {
            this.width = x;
            this.height = y;

            this.pixel = new List<Color>(x * y);
            for (int nrow = 0; nrow < y; nrow++)
                for (int ncol = 0; ncol < x; ncol++)
                    this.pixel.Add(new Color(0f, 0f, 0f));
        }


        /// <summary>
        /// Load image from a Stream. Import a PFM file from either a 
        /// memory stream or file stream.
        /// </summary>
        /// <param name="inputStream">Stream pointing to a .pfm file</param>
        public HdrImage(Stream inputStream) : this()
        {
            readPfm(inputStream);
        }

        /// <summary>
        /// Load image from a file. It takes as input a string 
        /// with the name of the file you want to open.
        /// </summary>
        /// <param name="fileName">String of the name of the pfm file you want to open.</param>
        public HdrImage(string fileName) : this()
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                readPfm(fileStream);
            }
        }



        // Boolean method to check if (x,y) is in the allowed pixel range.
        public bool validCoords(int x, int y)
        {
            return (x >= 0) & (x < this.width) & (y >= 0) & (y < this.height);

        }

        // It flattens the image into 1D array.
        public int pixelOffset(int x, int y)
        {
            return (y * this.width + x);
        }

        /// <summary>
        /// It outputs a pixel with given coordinates (x,y).
        /// </summary>
        /// <param name="x"> x coordinate of the pixel</param>
        /// <param name="y"> y coordinate of the pixel</param>
        /// <returns> The selected pixel. </returns>
        public Color getPixel(int x, int y)
        {

            if(validCoords(x, y))
            {
                return pixel[pixelOffset(x, y)];
            }
            else throw new InvalidPfmFileFormat($"Pixel {x},{y} out of range!");
        }

        /// <summary>
        /// Manually set the color of a single pixel by passing a <see cref="Color"/> object.
        /// </summary>
        /// <param name="x"> x coordinate of the pixel</param>
        /// <param name="y"> y coordinate of the pixel</param>
        /// <param name="a"> <see cref="Color"/> object</param>
        public void setPixel(int x, int y, Color a)
        {
            if (validCoords(x, y)) pixel[pixelOffset(x, y)] = a;
            else throw new InvalidPfmFileFormat($"Pixel {x},{y} out of range!");
        }

        /// <summary>
        /// It saves the image in .pfm format into a stream
        /// </summary>
        /// <param name="outputStream"> Output Stream (either Memory- or FileStream).</param>
        public void savePfm(Stream outputStream)
        {
            var endiannessValue = BitConverter.IsLittleEndian ? "-1.0" : "1.0";
            var header = Encoding.ASCII.GetBytes($"PF\n{this.width} {this.height}\n{endiannessValue}\n");
            outputStream.Write(header);
            var img = new HdrImage(7, 4);

            for (int x = 0; x < this.height; x++)
            {
                for (int y = 0; y < this.width; y++)
                {
                    // Bottom left to top right
                    Color col = this.getPixel(y, this.height - 1 - x);
                    _writeFloat(outputStream, col.r);
                    _writeFloat(outputStream, col.g);
                    _writeFloat(outputStream, col.b);
                }
            }
            return;
        }

        /// <summary>
        ///  Write a floating point number onto a stream after converting to binary.
        /// </summary>
        /// <param name="outputStream"> File or Memory stream where you are writing the float</param>
        /// <param name="rgb"> Float to be written</param>
        private static void _writeFloat(Stream outputStream, float rgb)
        {
            var buffer = BitConverter.GetBytes(rgb);
            outputStream.Write(buffer, 0, buffer.Length);

            return;
        }

        // Reading functions

        /// <summary>
        /// Read a byte line from a stream and convert to string.
        /// </summary>
        /// <param name="s"> The stream from which you want to read the line.</param>
        /// <returns> The line in <see cref="String"/> format.</returns>
        public static string readLine(Stream s)
        {
            string result = "";

            while (true)
            {
                var curByte = s.ReadByte(); // ReadByte returns -1 at the end of stream

                if (curByte == -1 || curByte == '\n')
                {
                    return result;
                }
                else
                {
                    result += Convert.ToChar(curByte);
                }
            }
        }

        /// <summary>
        /// Check if the image is encoded in little endian.
        /// This function makes advantage of the InvariantCulture method.
        /// </summary>
        /// <param name="line"> The 'endianness line' of a .pfm file</param>
        /// <returns> True if the image is encoded with little-endianness.</returns>
        public static bool isLittleEndian(string line)
        {
            float value;
            try
            {
                value = float.Parse(line, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidPfmFileFormat("Missing endianness specification");
            }

            if (value == 1.0f)
                return false;
            else if (value == -1.0f)
                return true;
            else
                throw new InvalidPfmFileFormat($"Invalid endianness specification: {value}");

        }

        /// <summary>
        /// Read a 32bit sequence from a stream and convert it to floating-point number.
        /// </summary>
        /// <param name="inputStream"> The input stream</param>
        /// <param name="lEnd"> True if the image is little-endian.</param>
        /// <returns> Float value corresponding to 4-byte sequence</returns>
        public static float readFloat(Stream inputStream, bool lEnd)
        {

            byte[] bytes = new byte[4];

            try
            {
                foreach (int i in Enumerable.Range(1, 4)) bytes[i] = (byte)inputStream.ReadByte();
            }
            catch
            {
                throw new InvalidPfmFileFormat("Unable to read float!");
            }

            if (!lEnd) Array.Reverse(bytes);
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Read the size (width x height) of a .pfm image
        /// </summary>
        /// <param name="line"> The third line of a .pfm file</param>
        /// <returns>List containing image width and height.</returns>
        public static List<int> parseImageSize(string line)
        {
            List<string> linePieces = new List<string>(line.Split(' ').ToList());
            if (linePieces.Count != 2) throw new InvalidPfmFileFormat("Invalid image size specification");

            List<int> widthHeight = new List<int>();
            try
            {
                widthHeight.Add(Int32.Parse(linePieces[0]));
                widthHeight.Add(Int32.Parse(linePieces[1]));
            }
            catch
            {
                throw new InvalidPfmFileFormat("Invalid width/height (not integers)");
            }

            if (widthHeight[0] < 0 || widthHeight[1] < 0) throw new InvalidPfmFileFormat("Invalid width/height (negative values)");

            return widthHeight;
        }

        /// <summary>
        ///  Read an image from a stream pointing to a .pfm file
        /// </summary>
        /// <param name="inputStream"> A stream pointing to a .pfm file.</param>
        public void readPfm(Stream inputStream)
        {
            string magic = readLine(inputStream);
            if (magic != "PF") throw new InvalidPfmFileFormat("Invalid magic PFM line!");

            string whLine = readLine(inputStream);
            List<int> w_h = parseImageSize(whLine);

            this = new HdrImage(w_h[0], w_h[1]);

            string endianLine = readLine(inputStream);
            bool lEnd = isLittleEndian(endianLine);

            Color temp = new Color();
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    temp.r = readFloat(inputStream, lEnd);
                    temp.g = readFloat(inputStream, lEnd);
                    temp.b = readFloat(inputStream, lEnd);
                    pixel[pixelOffset(j, this.height - 1 - i)] = temp;
                }
            }
        }

        /// <summary>
        /// Returns average luminosity of the image (logarithmic average)
        /// </summary>
        /// <param name="Delta"> Optional offset </param>
        /// <returns>Average luminosity in floating-point format.</returns>
        public float averageLumi(double delta = 1e-10)
        {

            double av = 0.0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    av += (Math.Log(delta + this.getPixel(i, j).Luminosity(), 10));
                }
            }

            return (float)Math.Pow(10, av / (width * height));

        }

        /// <summary>
        /// Divide every pixel of the image by a factor.
        /// </summary>
        /// <param name="factor"> The scaling factor</param>
        /// <param name="luminosity"> Average luminosity (optional)</param>
        public void normalizeImage(float factor, float? luminosity = null)

        {
            var lum = luminosity ?? averageLumi();

            for (int i = 0; i < this.pixel.Count; i++)
            {
                this.pixel[i] = (factor / lum) * this.pixel[i];
            }
            return;
        }

        private float _clamp(float x) => x / (1f + x);

        /// <summary>
        /// Apply tone mapping to the image by clamping every pixel
        /// </summary>
        public void clampImage()
        {
            for (int i = 0; i < pixel.Count; i++)
            {
                pixel[i] = new Color(_clamp(pixel[i].r), _clamp(pixel[i].g), _clamp(pixel[i].b));
            }
            return;
        }

        /// <summary>
        /// Saves the image into a LDR file (png/jpg)
        /// </summary>
        /// <param name="outputFile"> Output file name</param>
        /// <param name="format"> .png or .jpg (auto-recognised)</param>
        /// <param name="gamma"> gamma factor of your screen</param>
        public void writeLdrImage(string outputFile, string format, float gamma)

        {
            var bitmap = new Image<Rgb24>(Configuration.Default, this.width, this.height);
            for (int x = 0; x < this.width; x++)
            {
                for (int y = 0; y < this.height; y++)
                {
                    var curColor = this.getPixel(x, y);
                    var red = (int)(255 * Math.Pow(curColor.r, 1.0f / gamma));
                    var green = (int)(255 * Math.Pow(curColor.g, 1.0f / gamma));
                    var blue = (int)(255 * Math.Pow(curColor.b, 1.0f / gamma));

                    bitmap[x, y] = new Rgb24(Convert.ToByte(red), Convert.ToByte(green), Convert.ToByte(blue));
                }
            }

            using (Stream fileStream = File.OpenWrite(outputFile))
            {
                switch (format)
                {
                    case "png":
                        bitmap.SaveAsPng(fileStream);
                        break;
                    case "jpeg":
                    case "jpg":
                        bitmap.SaveAsJpeg(fileStream);
                        break;
                    default:
                        throw new CommandLineException($"{format} is not a valid format");
                }
            }
            return;
        }
    }

}
