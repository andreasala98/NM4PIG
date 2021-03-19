using System;
using System.IO;
using Trace;

namespace NM4PIG
{
    class Program
    {
        static void Main(string[] args)
        {
            var MyImg = new HdrImage(1024, 1024);

            for (int i=1; i<=1024; i++){
                for (int j=1; j<=1024; j++){
                    if((i+j)%2==0) MyImg.setPixel(i-1,j-1, new Color(100f,100f,100f));
                    else MyImg.setPixel(i-1,j-1, new Color(0f,0f,100f));
                }
            }
            
            string fileName = "chessBoard.pfm";

            using (FileStream fileStream = File.OpenWrite(fileName))
            {
                MyImg.savePfm(fileStream);
            }

                Console.WriteLine($"{fileName} correctly saved!");
        }
    }
}
    