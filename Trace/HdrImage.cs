using System;

namespace Trace
{
    // CLASSE HdrImage


    public struct HdrImage
    {
        public int width;
        public int height;
        public Color[] pixel;

        public HdrImage(int x, int y,)
        {
            // x NUMBER OF COLUMNS, y NUMBER OF ROWS
            width = x;
            height = y;
            
            // SE NECESSARIO, INIZIALIZZA TUTTI GLI ELEMENTI DEL VETTORE Color pixel A 0.
            /*for (int nrow = 0, y-1, nrow++)
            {
                for (int ncol = 0, x-1, ncol++)
                {
                    pixel[nrow * width + ncol] = new Color(0.0, 0.0, 0.0);
                }
            }*/
        }

        public Color getPixel(int x, int y)
        {
            assert validCoords(x, y);
            return pixel[pixelOffset(x,y)];
        }

        public void setPixel(int x, int y, Color a)
        {
            assert validCoords(x,y);
            pixel[pixelOffset(x,y)] = a;

        }

    }
}
