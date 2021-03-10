using System;
using Xunit;

namespace Color.Test
{
    public class UnitTest1
    {
        
        [Fact]
        public void TestSum(){
            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Assert.True(new Colore(5.0,7.0,9.0).is_close(col1+col2),"Sum Test failed!");
        }

        [Fact]
        public void TestDifference(){
            Colore col1 = new Colore(4.0,5.0,6.0);
            Colore col2 = new Colore(1.0, 2.0, 3.0);
            Assert.True(new Colore(3.0,3.0,3.0).is_close(col1-col2),"Difference Test failed!");
        }
    }
}
