using System;
using Xunit;
using Trace;
using System.IO;
using System.Text;

namespace Trace.Test
{

    // Color testing

    public class ColorTest
    {
        
       [Fact]
        // Test scalar product
        public void TestScalarProduct()
        {
            Color a = new Color (1.0, 2.5, 3.7);
            double alfa = 2.5;
            Assert.True(new Color (2.5, 6.25, 9.25).isClose(a * alfa), "Scalar product Test failed");
            Assert.True(new Color (2.5, 6.25, 9.25).isClose(alfa * a), "Scalar product Test failed");
        }

        [Fact]
        public void TestSum(){
            Color col1 = new Color(4.0,5.0,6.0);
            Color col2 = new Color(1.0, 2.0, 3.0);
            Assert.True(new Color(5.0,7.0,9.0).isClose(col1+col2),"Sum Test failed!");
        }

        [Fact]
        public void TestDifference(){
            Color col1 = new Color(4.0,5.0,6.0);
            Color col2 = new Color(1.0, 2.0, 3.0);
            Assert.True(new Color(3.0,3.0,3.0).isClose(col1-col2),"Difference Test failed!");
        }

        [Fact]
        public void TestProduct(){
            Color col1 = new Color(4.0,5.0,6.0);
            Color col2 = new Color(1.0, 2.0, 3.0);
            Assert.True(new Color(4.0,10.0,18.0).isClose(col1 * col2),"Product between colours Test failed!");
        }


    }

    // HDR Testing

    public class HDRImageTest
    {

        HdrImage DummyImage = new HdrImage(7, 4);

        [Fact]
        public void TestImageCreation()
        {
            Assert.True(DummyImage.width == 7);
            Assert.True(DummyImage.height == 4);
        }

        [Fact]
        public void TestValidCoords()
        {
            
            Assert.True(DummyImage.validCoords(0, 0));
            Assert.True(DummyImage.validCoords(6, 3));

            Assert.False(DummyImage.validCoords(-1, 0));
            Assert.False(DummyImage.validCoords(6, 4));

        }

        [Fact]
        public void TestPixelOffset()
        {
            Assert.True(DummyImage.pixelOffset(3, 2) == 17);
            Assert.True(DummyImage.pixelOffset(2, 3) == 23);
        }

        [Fact]
        public void TestGetSetPixel()
        {
            var appo = new Color(5.0, 6.0, 7.0);
            DummyImage.setPixel(3, 2, appo);
            Assert.True(DummyImage.getPixel(3, 2).isClose(appo));

        }


        [Fact]
        public void TestPfm()
        {
            var MyImg = new HdrImage(3, 2);

            //Naive method
            MyImg.setPixel(0, 0, new Color(10.0, 20.0, 30.0));
            MyImg.setPixel(1, 0, new Color(40.0, 50.0, 60.0));
            MyImg.setPixel(2, 0, new Color(70.0, 80.0, 90.0));
            MyImg.setPixel(0, 1, new Color(100.0, 200.0, 300.0));
            MyImg.setPixel(1, 1, new Color(400.0, 500.0, 600.0));
            MyImg.setPixel(2, 1, new Color(700.0, 800.0, 900.0));

            /*byte[] ref_bytes = { 0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
                                 0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
                                 0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
                                 0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
                                 0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
                                 0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
                                 0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43 };*/

            byte[] ref_bytes = {
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x59, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x69, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x72, 0x40,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x79, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x40, 0x7f, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x82, 0x40,
  0x00, 0x00, 0x00, 0x00, 0x00, 0xe0, 0x85, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x89, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x20, 0x8c, 0x40,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x24, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x34, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x3e, 0x40,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x44, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x49, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4e, 0x40,
  0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x51, 0x40, 0x00, 0x00, 0x00, 0x00,
  0x00, 0x00, 0x54, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80, 0x56, 0x40
            };                     

            using (MemoryStream memSt = new MemoryStream(144))
            {
                MyImg.savePfm(memSt);
                Assert.True(memSt.GetBuffer() == ref_bytes);
            }
        }

    }
}
