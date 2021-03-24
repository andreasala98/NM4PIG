using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Xunit;

namespace Trace.Test
{
    // HDR Testing

    public class HDRImageTest
    {

        HdrImage DummyImage = new HdrImage(7, 4);

        [Fact]
        public void TestValidCoords()
        {

            Assert.True(DummyImage.validCoords(0, 0));
            Assert.True(DummyImage.validCoords(6, 3));

            Assert.False(DummyImage.validCoords(-1, 0));
            Assert.False(DummyImage.validCoords(6, 4));

        }

        [Fact]
        public void TestImageCreation()
        {
            Assert.True(DummyImage.width == 7);
            Assert.True(DummyImage.height == 4);
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
            var appo = new Color(5.0f, 6.0f, 7.0f);
            DummyImage.setPixel(3, 2, appo);
            Assert.True(DummyImage.getPixel(3, 2).isClose(appo));

        }


        [Fact]
        public void TestSavePfm()
        {
            var MyImg = new HdrImage(3, 2);

            //Naive method
            MyImg.setPixel(0, 0, new Color(10.0f, 20.0f, 30.0f));
            MyImg.setPixel(1, 0, new Color(40.0f, 50.0f, 60.0f));
            MyImg.setPixel(2, 0, new Color(70.0f, 80.0f, 90.0f));
            MyImg.setPixel(0, 1, new Color(100.0f, 200.0f, 300.0f));
            MyImg.setPixel(1, 1, new Color(400.0f, 500.0f, 600.0f));
            MyImg.setPixel(2, 1, new Color(700.0f, 800.0f, 900.0f));

            byte[] ref_bytes = { 0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a, // Header 
                                 0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43, // Raster starts here 
                                 0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
                                 0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
                                 0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
                                 0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
                                 0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42 };

            using (MemoryStream memSt = new MemoryStream(84))
            {
                MyImg.savePfm(memSt);
                Assert.True(memSt.GetBuffer().SequenceEqual(ref_bytes));

            }
        }

        [Fact]
        public void TestReadLine()
        {
            byte[] byteArray = Encoding.ASCII.GetBytes("hello\nworld");

            MemoryStream line = new MemoryStream(byteArray);
            Assert.True(HdrImage.readLine(line) == "hello");
            Assert.True(HdrImage.readLine(line) == "world");
            Assert.True(HdrImage.readLine(line) == "");
        }

        [Fact]
        public void TestIsLittleEndian()
        {
            Assert.True(HdrImage.isLittleEndian("1.0") == false);
            Assert.True(HdrImage.isLittleEndian("-1.0") == true);
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.isLittleEndian("2.0"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.isLittleEndian("abc"));
        }

        [Fact]
        public void TestParseImageSize()
        {
            Assert.True(HdrImage.parseImageSize("3 2").SequenceEqual(new List<int>() { 3, 2 }));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.parseImageSize("-1 2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.parseImageSize("1 -2"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.parseImageSize("3 2 1"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.parseImageSize("3"));
            Assert.Throws<InvalidPfmFileFormat>(() => HdrImage.parseImageSize("a b"));
        }

        [Fact]
        public void TestReadPfm()
        {
            var MyImg = new HdrImage(3, 2);


            using (FileStream fs = File.OpenRead("../../../reference_le.pfm"))
            {
                MyImg.readPfm(fs);
            }

            Assert.True(MyImg.width == 3);
            Assert.True(MyImg.height == 2);
            Assert.True(MyImg.getPixel(0, 0).isClose(new Color(10.0f, 20.0f, 30.0f)));
            Assert.True(MyImg.getPixel(1, 0).isClose(new Color(40.0f, 50.0f, 60.0f)));
            Assert.True(MyImg.getPixel(2, 0).isClose(new Color(70.0f, 80.0f, 90.0f)));
            Assert.True(MyImg.getPixel(0, 1).isClose(new Color(100.0f, 200.0f, 300.0f)));
            Assert.True(MyImg.getPixel(1, 1).isClose(new Color(400.0f, 500.0f, 600.0f)));
            Assert.True(MyImg.getPixel(2, 1).isClose(new Color(700.0f, 800.0f, 900.0f)));


            using (FileStream fs = File.OpenRead("../../../reference_be.pfm"))
            {
                MyImg.readPfm(fs);
            }

            Assert.True(MyImg.width == 3);
            Assert.True(MyImg.height == 2);
            Assert.True(MyImg.getPixel(0, 0).isClose(new Color(10.0f, 20.0f, 30.0f)));
            Assert.True(MyImg.getPixel(1, 0).isClose(new Color(40.0f, 50.0f, 60.0f)));
            Assert.True(MyImg.getPixel(2, 0).isClose(new Color(70.0f, 80.0f, 90.0f)));
            Assert.True(MyImg.getPixel(0, 1).isClose(new Color(100.0f, 200.0f, 300.0f)));
            Assert.True(MyImg.getPixel(1, 1).isClose(new Color(400.0f, 500.0f, 600.0f)));
            Assert.True(MyImg.getPixel(2, 1).isClose(new Color(700.0f, 800.0f, 900.0f)));

        }
    }
}