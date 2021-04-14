/*
The MIT License (MIT)

Copyright © 2021 Tommaso Armadillo, Pietro Klausner, Andrea Sala

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
documentation files (the “Software”), to deal in the Software without restriction, including without limitation the
rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of
the Software. THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT
SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
IN THE SOFTWARE.
*/

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

            byte[] LE_REFERENCE_BYTES = {
                0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
                0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
                0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
                0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
                0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
                0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
                0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
            };

            using (MemoryStream memSt = new MemoryStream(LE_REFERENCE_BYTES.Length))
            {
                memSt.Write(LE_REFERENCE_BYTES);
                memSt.Seek(0, SeekOrigin.Begin);
                MyImg.readPfm(memSt);
            }

            Assert.True(MyImg.width == 3);
            Assert.True(MyImg.height == 2);
            Assert.True(MyImg.getPixel(0, 0).isClose(new Color(10.0f, 20.0f, 30.0f)));
            Assert.True(MyImg.getPixel(1, 0).isClose(new Color(40.0f, 50.0f, 60.0f)));
            Assert.True(MyImg.getPixel(2, 0).isClose(new Color(70.0f, 80.0f, 90.0f)));
            Assert.True(MyImg.getPixel(0, 1).isClose(new Color(100.0f, 200.0f, 300.0f)));
            Assert.True(MyImg.getPixel(1, 1).isClose(new Color(400.0f, 500.0f, 600.0f)));
            Assert.True(MyImg.getPixel(2, 1).isClose(new Color(700.0f, 800.0f, 900.0f)));

            byte[] BE_REFERENCE_BYTES = {
                0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
                0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
                0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
                0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
                0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
                0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
                0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
            };

            using (MemoryStream memSt = new MemoryStream(BE_REFERENCE_BYTES.Length))
            {
                memSt.Write(BE_REFERENCE_BYTES);
                memSt.Seek(0, SeekOrigin.Begin);
                MyImg.readPfm(memSt);
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

        [Fact]
        public void TestAvarageLumi()
        {
            HdrImage img = new HdrImage(2, 1);

            img.setPixel(0, 0, new Color(5.0f, 10.0f, 15.0f));
            img.setPixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f));
            Assert.True(Color.isClose(img.averageLumi(), 100.0f));
        }

        [Fact]
        public void TestNormalizeImage()
        {
            var img = new HdrImage(2, 1);

            img.setPixel(0, 0, new Color(0.5e1f, 1.0e1f, 1.5e1f));
            img.setPixel(1, 0, new Color(0.5e3f, 1.0e3f, 1.5e3f));

            img.normalizeImage(factor: 1000.0f, luminosity: 100.0f);
            Assert.True(img.getPixel(0, 0).isClose(new Color(0.5e2f, 1.0e2f, 1.5e2f)));
            Assert.True(img.getPixel(1, 0).isClose(new Color(0.5e4f, 1.0e4f, 1.5e4f)));
        }

        [Fact]
        public void TestClampImage()
        {
            var img = new HdrImage(2, 1);

            img.setPixel(0, 0, new Color(0.5e1f, 1.0e1f, 1.5e1f));
            img.setPixel(1, 0, new Color(0.5e3f, 1.0e3f, 1.5e3f));

            img.clampImage();

            foreach (var curPixel in img.pixel)
            {
                Assert.True((curPixel.r >= 0) && (curPixel.r <= 1));
                Assert.True((curPixel.g >= 0) && (curPixel.g <= 1));
                Assert.True((curPixel.b >= 0) && (curPixel.b <= 1));
            }

        }
    }
}