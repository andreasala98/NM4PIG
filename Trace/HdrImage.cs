using System;


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

    }

    
}
