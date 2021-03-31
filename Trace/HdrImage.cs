using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

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

        public static string readLine(Stream s)  // CURRENTLY NOT WORKING
        {
            string result = "";

            while (true)
            {
                var curByte = s.ReadByte(); // ReadByte returns -1 as the end of stream

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

        public static bool isClose(double a, double b)
        {
            var epsilon = 1e-8F;
            return Math.Abs(a - b) < epsilon;
        }

        public double averageLumi(double? Delta = null) {
            
            double delta = Delta ?? 1e-10;
            double avarage = 0.0;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    avarage = avarage + (Math.Log(delta + this.getPixel(i, j).Luminosity(), 10));
                }
            }
            
            avarage = Math.Pow(10, avarage / (width * height));
            return avarage;
        }

        /*public void normalizeImage()
        {
            // etc
            return;
        }

        public void clampImage()
        {
            return;
        }

        public void writeLdrImage()
        {
            return;
        }*/
    }

}
