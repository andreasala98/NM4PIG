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

namespace Trace
{
    /// <summary>
    /// PCG Uniform Pseudo-random Number Generator
    /// </summary>
    public class PCG
    {
        public ulong state { get; set; } = 0ul;
        public ulong inc { get; set; } = 0ul;

        public PCG(ulong initState = 42, ulong initSeq = 54)
        {
            this.inc = (initSeq << 1) | 1u;
            this.random();
            this.state += initState;
            this.random();
        }

        /// <summary>
        /// Return a new random number and advance PCG's internal state
        /// </summary>         
        public uint random()
        {
            ulong oldstate = this.state;
            this.state = (oldstate * 6364136223846793005ul + this.inc);
            uint xorshifted = (uint)(((oldstate >> 18) ^ oldstate) >> 27);
            int rot = (int)(oldstate >> 59);
            return (xorshifted >> rot) | (xorshifted << ((-rot) & 31));
        }

        /// <summary>
        /// Return a new float random number in [0,1) and advance PCG's internal state
        /// </summary>
        public float randomFloat()
            => this.random() / (float)UInt32.MaxValue;

    } // PCG
} // Trace