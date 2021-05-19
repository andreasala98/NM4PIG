
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


    /// <summary>
    ///   A derivate class of BRDF representing an ideal diffusive BRDF (also called «Lambertian»)
    /// </summary>
    public class DiffuseBRDF : BRDF {

        public DiffuseBRDF(Pigment pig= new UniformPigment(Constant.White), float refl=1.0f) {
            this.pigment = pig;
            this.reflectance = refl;
        }

        public override Color Eval(Normal normal, Vec inDir, Vec outDir, Vec2D uv)
        {
            return this.pigment.getColor(uv) * (this.reflectance / Constant.PI);
        }

    }




} // Trace