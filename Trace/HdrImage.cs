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

    public struct HdrImage
    {
        public int width;
        public int height;
        public List<Color> pixel;

        // Constructor with pixel number
        public HdrImage(int x, int y)
        {
            // x = COLS, y = ROWS (larghezza x altezza)
            this.width = x;
            this.height = y;

            // Initializing all pixels to black.
            this.pixel = new List<Color>(x * y);
            for (int nrow = 0; nrow < y; nrow++)
            {
                for (int ncol = 0; ncol < x; ncol++)
                {
                    this.pixel.Add(new Color(0f, 0f, 0f));
                }
            }
        }


        // Constructor passing a stream
        public HdrImage(Stream inputStream) : this()
        {
            readPfm(inputStream);
        }

        // Constructor passing a string (fileName)
        public HdrImage(string fileName) : this()
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                readPfm(fileStream);
            }
        }



        // Checking if (x,y) is in range
        public bool validCoords(int x, int y)
        {
            return (x >= 0) & (x < this.width) & (y >= 0) & (y < this.height);

        }

        // Flattening the image into 1D array
        public int pixelOffset(int x, int y)
        {
            return (y * this.width + x);
        }


        public Color getPixel(int x, int y)
        {

            if (validCoords(x, y))
            {
                return pixel[pixelOffset(x, y)];
            }
            else
            {
                Console.WriteLine($"Pixel ({x},{y}) out of range!");
                return new Color(0f, 0f, 0f);
            }
        }

        public void setPixel(int x, int y, Color a)
        {
            if (validCoords(x, y))
            {
                pixel[pixelOffset(x, y)] = a;
            }
            else
            {
                Console.WriteLine($"Pixel ({x},{y}) out of range!");
            }
        }

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

        private static void _writeFloat(Stream outputStream, float rgb)
        {
            var buffer = BitConverter.GetBytes(rgb);
            outputStream.Write(buffer, 0, buffer.Length);

            return;
        }

        // Reading functions

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

        public static float readFloat(Stream inputStream, bool lEnd)
        {

            byte[] bytes = new byte[4];

            try
            {
                bytes[0] = (byte)inputStream.ReadByte();
                bytes[1] = (byte)inputStream.ReadByte();
                bytes[2] = (byte)inputStream.ReadByte();
                bytes[3] = (byte)inputStream.ReadByte();

            }
            catch
            {
                throw new InvalidPfmFileFormat("Unable to read float!");
            }

            if (lEnd)
            {
                return BitConverter.ToSingle(bytes, 0);
            }
            else
            {
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes, 0);
            }
        }

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

        public float averageLumi(double? Delta = null)
        {

            double delta = Delta ?? 1e-10;
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

        public void clampImage()
        {
            for (int i = 0; i < pixel.Count; i++)
            {
                pixel[i] = new Color(_clamp(pixel[i].r), _clamp(pixel[i].g), _clamp(pixel[i].b));
            }
            return;
        }


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
