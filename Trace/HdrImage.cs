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

        /*private void readPfm(Stream inputStream)
        {
            // string magic = LineRead(inputStream);
            // if (magic != "PF") throw new InvalidPfmFileFormat("Invalid magic PFM line!");

            // string sizeLine = LineRead(inputStream);
            // List<int> w_h = new List<int>(parseImageSize(sizeLine));
            // this.width = w_h[0];
            // this.height = w_h[1];

            // string endianLine = LineRead(inputStream);

            // bool lEnd = isLittleEndian(endianLine);

            // still need to read the raster
        }*/



        // Constructor with pixel number
        public HdrImage(int x, int y)
        {
            // x = COLS, y = ROWS
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

        /*
        // Constructor passing a stream
        public HdrImage(Stream inputStream)
        {
            readPfm(inputStream);
        }

        // Constructor passing a string (fileName)
        public HdrImage(string fileName)
        {
            using (FileStream fileStream = File.OpenRead(fileName))
            {
                readPfm(fileStream);
            }
        }
*/


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

         // READ FUNCTIONS SET-UP

        public static string LineRead(Stream s)  // CURRENTLY NOT WORKING
        {
            string result = "";

            while (true)
            {
                var curByte = s.ReadByte(); // ReadByte returns -1 as the end of stream. curByte needs then to be a var type

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

        public static float _readFloat(Stream inputStream, bool lEnd)
        {

            byte[] bytes;

            try
            {

                using (BinaryReader br = new BinaryReader(inputStream))
                {
                    bytes = br.ReadBytes(4);
                }

            }
            catch
            {
                throw new InvalidPfmFileFormat("Unable to read float!");
            }

            if (lEnd)
            {
                return BitConverter.ToSingle(bytes);
            }
            else
            {
                Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes);
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
                throw new InvalidPfmFileFormat("Invalid width/height (not numbers)");
            }

            if (widthHeight[0] < 0 || widthHeight[1] < 0) throw new InvalidPfmFileFormat("Invalid width/height (negative values)");

            return widthHeight;
        }


    }

}






