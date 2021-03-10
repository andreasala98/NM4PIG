using System;
using Xunit;

namespace Color.Test
{
    public class UnitTest1
    {
        
       [Fact]
        // Test scalar product
        public void TestScalarProduct()
        {
            Colore a = new Colore (1.0, 2.5, 3.7);
            double alfa = 2.5;
            Assert.True(new Colore (2.5, 6.25, 9.25).isClose(a * alfa), "Scalar product Test failed");
            Assert.True(new Colore (2.5, 6.25, 9.25).isClose(alfa * a), "Scalar product Test failed");
        }

        [Fact]
        public void TestSum(){
            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Assert.True(new Colore(5.0,7.0,9.0).isClose(col1+col2),"Sum Test failed!");
        }

        [Fact]
        public void TestDifference(){
            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Assert.True(new Colore(3.0,3.0,3.0).isClose(col1-col2),"Difference Test failed!");
        }

        [Fact]
        public void TestProduct(){
            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Assert.True(new Colore(4.0,10.0,18.0).isClose(col1 * col2),"Product between colours Test failed!");
        }


    }
}
