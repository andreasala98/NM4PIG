using System;
using Stream;
using BitConverter;

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

        public void savePfm(Stream outputStream)
        {
            var header = Encoding.ASCII.GetBytes($"PF\n{width} {height}\n{endianness_value}\n");
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

    

    

