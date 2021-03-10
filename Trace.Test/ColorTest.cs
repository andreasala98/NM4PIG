using System;
using Xunit;
using Trace;

namespace Trace.Test
{
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
}
