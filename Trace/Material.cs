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

public abstract class BRDF { }

public abstract class Pigment { }

/// <summary>
/// A class that implements a material
/// </summary>
public class Material
{
    /// <summary>
    /// A <see cref="BRDF"/> object
    /// </summary>
    public BRDF brdf;

    /// <summary>
    /// A <see cref="Pigment"/> object
    /// </summary>
    public Pigment emittedRadiance;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="Brdf">A <see cref="BRDF"/> object, default is DiffuseBRDF()</param>
    /// <param name="EmittedRadiance">A <see cref="Pigment"/> object, default is UniformPigment(Constant.Black)</param>
    public Material(BRDF Brdf = new DiffuseBRDF(), Pigment EmittedRadiance = new UniformPigment(Constant.Black))
    {
        this.brdf = Brdf;
        this.emittedRadiance = EmittedRadiance;
    }
}

