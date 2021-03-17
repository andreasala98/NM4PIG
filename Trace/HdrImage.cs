using System;
using System.IO;

namespace Trace
{
    // CLASSE HdrImage
    public struct HdrImage
    {
        public int width;
        public int height;
        public Color[] pixel;

        public HdrImage(int x, int y)
        {
            // x NUMBER OF COLUMNS, y NUMBER OF ROWS
            this.width = x;
            this.height = y;
            
            this.pixel= new Color[6];
            // SE NECESSARIO, INIZIALIZZA TUTTI GLI ELEMENTI DEL VETTORE Color pixel A 0.
            for (int nrow = 0; nrow<2; nrow++)
            {
                for (int ncol = 0; ncol<3; ncol++)
                {
                    this.pixel[nrow * width + ncol] = new Color(0.0, 0.0, 0.0);
                }
            }
        }

        public bool validCoords(int x, int y)
        {
            return true;
        }

        public int pixelOffset(int x, int y)
        {
            return y*this.height + x;
        }

        public Color getPixel(int x, int y)
        {
            return pixel[pixelOffset(x,y)];
        }

        public void setPixel(int x, int y, Color a)
        {
            this.pixel[pixelOffset(x,y)] = a;

        }

        public void savePfm(Stream outputStream)
        {
            var endiannessValue = BitConverter.IsLittleEndian;
            var header = Encoding.ASCII.GetBytes($"PF\n{this.width} {this.height}\n{endiannessValue}\n");
            var img = new HdrImage(7, 4);

            for(int x=0; x<this.height; x++){
                for(int y=0; y<this.width;y++){
                    Color col = this.getPixel(this.height-1-x, y);
                    _writeFloat(outputStream, col.r);
                    _writeFloat(outputStream, col.g);
                    _writeFloat(outputStream, col.b);
                }
            }
        }
       
        private static void _writeFloat(Stream outputStream, double value)
        {
            var seq = BitConverter.GetBytes(value);
            outputStream.Write(seq, 0, seq.Length);
        }   
    
    }
}

    

    

