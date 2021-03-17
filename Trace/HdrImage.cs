using System;

namespace Trace
{
    // CLASSE HdrImage

    public struct HdrImage
    {
        public int width;
        public int height;
        public Color[] pixel;

        public Color getPixel(int x, int y)
        {
            assert validCoordinates(x, y);
            return pixel[pixelOffset(x,y)];
        }

        public void setPixel(int x, int y, Color a)
        {
            assert validCoordinates(x,y);
            pixel[pixelOffset(x,y)] = a;

        }

    }
}
