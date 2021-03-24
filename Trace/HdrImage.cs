using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Trace
{
    
    public struct HdrImage
    {
        public int width;
        public int height;
        public Color[] pixel;

        private void readPfm(Stream inputStream)
        {
            string magic = LineRead(inputStream);
            if (magic != "PF") throw new InvalidPfmFileFormat("Invalid magic PFM line!");

            string sizeLine = LineRead(inputStream);
            int[] w_h = parseImageSize(sizeLine);
            this.width = w_h[0];
            this.height = w_h[1];

            string endianLine = LineRead(inputStream);

            bool lEnd = isLittleEndian(endianLine);

            // still need to read the raster
        }



        // Constructor with pixel number
        public HdrImage(int x, int y)
        {
            // x = COLS, y = ROWS
            this.width = x;
            this.height = y;
            this.pixel= new Color[6];
            
            // Initializing all pixels to black.
            this.pixel = new Color[x * y];
            for (int nrow = 0; nrow < y; nrow++)
            {
                for (int ncol = 0; ncol < x; ncol++)
                {
                    this.pixel [nrow * width + ncol] = new Color(0f, 0f, 0f);
                }
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
                return pixel[pixelOffset(x,y)];
            }
            else 
            {
                Console.WriteLine($"Pixel ({x},{y}) out of range!");
                return new Color(0f,0f,0f);
            }
        }


        public void setPixel(int x, int y, Color a)
        {
            if (validCoords(x,y))
            {
                pixel[pixelOffset(x,y)] = a;
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



            for (int x=0; x<this.height; x++){
                for(int y=0; y<this.width;y++){
                    // Bottom left to top right
                    Color col = this.getPixel(y, this.height-1-x);
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

        public string LineRead(Stream s)
        {
            byte[] result = Convert.FromBase64String("");
            byte[] vuoto = Convert.FromBase64String("");
            byte[] aCapo = Convert.FromBase64String("\n");
            //Encoding.ASCII.GetBytes(acapo);


            while (true)
            {
                byte[] curByte;
                using (BinaryReader br = new BinaryReader(s)) { curByte = br.ReadBytes(1); }
                byte[] result;
                if (curByte == vuoto) 
                {
                    return System.Text.Encoding.ASCII.GetString(curByte);
                }
                else if (curByte == aCapo)
                {
                   return System.Text.Encoding.ASCII.GetString(curByte);
                }
                else 
                {
                    List<byte> list = new List<byte>();
                    list.AddRange(result);
                    list.AddRange(curByte);

                    result= list.ToArray();
                    
                    //result = addByteToArray(result, curByte);
                }
            } 
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
    
    }



    
}

    

    

