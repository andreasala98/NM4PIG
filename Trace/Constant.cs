using System;

namespace Trace
{
 /// <summary>
    /// A class that contains some useful constants, which are used many times inside the library.
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// A float version of Math.PI
        /// </summary>
        public static float PI = (float)Math.PI;

        /// <summary>
        /// The unit vector for the x-axis
        /// </summary>
        public static Vec VEC_X = new Vec(1.0f, 0.0f, 0.0f);
        public static Normal VEC_X_N = new Normal(1.0f, 0.0f, 0.0f);

        /// <summary>
        /// The unit vector for the y-axis
        /// </summary>
        public static Vec VEC_Y = new Vec(0.0f, 1.0f, 0.0f);
        public static Normal VEC_Y_N = new Normal(0.0f, 1.0f, 0.0f);

        /// <summary>
        /// The unit vector for the z-axis
        /// </summary>
        public static Vec VEC_Z = new Vec(0.0f, 0.0f, 1.0f);
        public static Normal VEC_Z_N = new Normal(0.0f, 0.0f, 1.0f);

        /// <summary>
        /// <see cref="Point"> object do describe the origin of axes, i.e. (0,0,0)
        /// </summary>
        public static Point Origin = new Point(0.0f, 0.0f, 0.0f);


        public static Color White = new Color(1.0f, 1.0f, 1.0f);

        public static Color Black = new Color(0.0f, 0.0f, 0.0f);

        public static Color Red = new Color(1.0f, 0.0f, 0.0f);

        public static Color Pink = new Color(1.0f, 0.7529f, 0.7961f);

        public static Color Green = new Color(0.0f, 1.0f, 0.0f);

        public static Color Blue = new Color(0.0f, 0.0f, 1.0f);

        public static Color Yellow = new Color(1.0f, 1.0f, 0.0f);

        public static Color Orange = new Color(1.0f, 0.5f, 0.0f);

        public static Color SkyBlue = new Color(0.529f, 0.808f, 0.922f);

        public static Color BroomYellow = new Color(1.0f, .9451f, .1490f);
        public static Color BlueChill = new Color(0.05f, 0.6f, 0.6f);
        public static Color BrightGreen = new Color(.41176f, .9647f, .17254f);
        public static Color LightRed = new Color(1f, .3f, .3f);



        public static Material skyMat = new Material(new DiffuseBRDF(new UniformPigment(SkyBlue)), new UniformPigment(SkyBlue));
        public static Material groundMat = new Material(new DiffuseBRDF(), new CheckeredPigment(BrightGreen, BlueChill));
        public static Material BWgroundMat = new Material(new DiffuseBRDF(), new CheckeredPigment(White, Black));
        public static Material refMat = new Material(new SpecularBRDF(new UniformPigment(BroomYellow)));
        public static Material greenMat = new Material(new DiffuseBRDF(new UniformPigment(BrightGreen)));
        public static Material redMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(170f / 255, 1f / 255, 20f / 255))));
        public static Material blueMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(0f, 78f / 255, 255f / 255))));


        public static Shape wikiShape() { 

            Shape C1 = new Cylinder(Origin, radius: 0.8f, height: 2.5f, Constant.VEC_X, greenMat);
            Shape C2 = new Cylinder(Origin, radius: 0.8f, height: 2.5f, Constant.VEC_Y, greenMat);
            Shape C3 = new Cylinder(Origin, radius: 0.8f, height: 2.5f, Constant.VEC_Z, greenMat);

            Shape S1 = new Sphere(Transformation.Scaling(1.4f), blueMat);
            Shape B1 = new Box(material: redMat);

            Shape tot = (S1 * B1) - ((C1 + C2) + C3);

            return tot;
        }

        public static Sphere SKY = new Sphere(Transformation.Scaling(500f), skyMat);



    }
}