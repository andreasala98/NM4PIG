using System;
using Xunit;

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
            /*for (int nrow = 0, y-1, nrow++)
            {
                for (int ncol = 0, x-1, ncol++)
                {
                    pixel[nrow * width + ncol] = new Color(0.0, 0.0, 0.0);
                }
            }*/
        }

        // CONTROLLA CHE LE COORDINATE x, y SIANO ALL'INTERNO DELL'IMMAGINE.
        public bool validCoords(int x, int y, this)
        {
            return (x >= 0) & (x <= this.width) & (y >= 0) & (y <= this.height);
     
        }

        // RESTITUISCE L'INDICE DEL VETTORE pixel CHE RAPPRESENTA IL PUNTO CON COORDINATE (y, x) [-> riga = y, colonna = x]
        public int pixelOffset(int x, int y, this)
        {
            return (y * this.width + x);
        }
        public Color getPixel(int x, int y)
        {
            Assert.True(validCoords(x, y));
            return pixel[pixelOffset(x,y)];
        }

        public void setPixel(int x, int y, Color a)
        {
            Assert.True(validCoords(x,y));
            pixel[pixelOffset(x,y)] = a;

        }

    }

    
}
