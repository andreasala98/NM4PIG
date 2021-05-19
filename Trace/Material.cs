
using System;

namespace Trace
{

    public abstract class Pigment {

        public Color color;
        public Color getColor(Vec2D uv) {
            return new Color(0f, 0f, 0f);
        }


    }


/// <summary>
/// An abstract class representing a Bidirectional Reflectance Distribution Function.
/// Use only derivate classes in the program.
/// </summary>

    public abstract class BRDF
    {

        public Pigment pigment;
        public float reflectance;
        public BRDF() { }
        public abstract Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv);

    }

    public class DiffuseBRDF : BRDF {



        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            return this.pigment.getColor(uv) * (this.reflectance / Constant.PI);
        }

    }




} // Trace