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

using Xunit;

namespace Trace.Test
{

    // Color testing
    public class TestPCG
    {

        [Fact]
        public void TestRandom()
        {
            PCG pcg = new PCG();
            Assert.True(pcg.state == 1753877967969059832UL, "TestRandom failed - 1/8");
            Assert.True(pcg.inc == 109UL, "TestRandom failed - 2/8");

            uint[] expected = {
                                2707161783U,
                                2068313097U,
                                3122475824U,
                                2211639955U,
                                3215226955U,
                                3421331566U
                            };
            int count = 3;
            foreach (uint rand in expected)
            {
                uint result = pcg.random();
                Assert.True(rand == result, $"TestRandom failed - Assert {count}/8");
                count++;
            }
        }
    }

}