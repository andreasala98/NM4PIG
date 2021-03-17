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

            this.pixel = new Color[x * y];
            for (int nrow = 0; nrow < y; nrow++)
            {
                for (int ncol = 0; ncol < x; ncol++)
                {
                    this.pixel [nrow * width + ncol] = new Color(0.0, 0.0, 0.0);
                }
            }
        }

        // CONTROLLA CHE LE COORDINATE x, y SIANO ALL'INTERNO DELL'IMMAGINE.
        public bool validCoords(int x, int y)
        {
            return (x >= 0) & (x < this.width) & (y >= 0) & (y < this.height);
    
        }

        // RESTITUISCE L'INDICE DEL VETTORE pixel CHE RAPPRESENTA IL PUNTO CON COORDINATE (y, x) [-> riga = y, colonna = x]
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
                //Color a = new Color(0.0, 0.0, 0.0);
                Console.WriteLine("Pixel fuori dall'immagine; errore!");
                return new Color(0,0,0);
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
                Console.WriteLine("Pixel fuori dall'immagine; errore!");
            }
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

    

    

