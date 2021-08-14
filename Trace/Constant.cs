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
        public static Color PiggyPink = new Color(.99f, .71f, .756f);



        public static Material skyMat = new Material(new DiffuseBRDF(new UniformPigment(SkyBlue)), new UniformPigment(SkyBlue));
        public static Material groundMat = new Material(new DiffuseBRDF(), new CheckeredPigment(BrightGreen, BlueChill));
        public static Material BWgroundMat = new Material(new DiffuseBRDF(), new CheckeredPigment(White, Black));
        public static Material refMat = new Material(new SpecularBRDF(new UniformPigment(BroomYellow)));
        public static Material greenMat = new Material(new DiffuseBRDF(new UniformPigment(Green)));
        public static Material yellowMat = new Material(new DiffuseBRDF(new UniformPigment(Yellow)));
        public static Material redMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(255f / 255, 0f / 255, 0f / 255))));
        public static Material blueMat = new Material(new DiffuseBRDF(new UniformPigment(Blue)));
        public static Material pigMat = new Material(new DiffuseBRDF(new UniformPigment(new Color(240f / 255, 144f / 255, 137f / 255))));
        public static Material blackMat = new Material(new DiffuseBRDF(new UniformPigment(Black)));
        public static Material whiteMat = new Material(new DiffuseBRDF(new UniformPigment(White)));

        /// <summary>
        ///  An interesting case of Constructive SOlid Geometry
        /// </summary>
        /// <param name="transf"> A <see cref="Transformation"/> object associated to the shape.</param>
        /// <returns></returns>
        public static Shape wikiShape(Transformation? transf = null)
        {

            Shape C1 = new Cylinder(Origin, radius: 0.7f, height: 2.5f, Constant.VEC_X, yellowMat);
            Shape C2 = new Cylinder(Origin, radius: 0.7f, height: 2.5f, Constant.VEC_Y, yellowMat);
            Shape C3 = new Cylinder(Origin, radius: 0.7f, height: 2.5f, Constant.VEC_Z, yellowMat);

            Shape S1 = new Sphere(Transformation.Scaling(1.35f), blueMat);
            Shape B1 = new Box(material: redMat);

            Shape tot = (S1 * B1) - ((C1 + C2) + C3);

            if (transf.HasValue) tot.transformation = transf.Value;
            return tot;
        }

        /// <summary>
        /// An emissive sphere representing the sky.
        /// </summary>
        public static Sphere SKY = new Sphere(Transformation.Scaling(500f), skyMat);


        /// <summary>
        /// A complete pig constructed with CSG
        /// </summary>
        /// <param name="transf">A <see cref="Transformation"/> object associated to the shape.</param>
        /// <returns></returns>
        public static Shape pig(Transformation? transf = null)
        {
            // Body
            Shape leg1 = new Cylinder(new Point(x: 1f, y: 0.5f, z: 0.4f), radius: 0.25f, height: 2f, direction: Constant.VEC_Z, material: pigMat);
            Shape leg2 = new Cylinder(new Point(x: 1f, y: -0.5f, z: 0.4f), radius: 0.25f, height: 2f, direction: Constant.VEC_Z, material: pigMat);
            Shape leg3 = new Cylinder(new Point(x: -1f, y: 0.5f, z: 0.4f), radius: 0.25f, height: 2f, direction: Constant.VEC_Z, material: pigMat);
            Shape leg4 = new Cylinder(new Point(x: -1f, y: -0.5f, z: 0.4f), radius: 0.25f, height: 2f, direction: Constant.VEC_Z, material: pigMat);
            Shape torso = new Sphere(transformation: Transformation.Translation(0f, 0f, 1.1f) * Transformation.Scaling(1.85f, 1.05f, 1.05f), material: pigMat);

            Shape body = torso + leg1 + leg2 + leg3 + leg4;

            // Head 
            Shape ball = new Sphere(transformation: Transformation.Translation(-2.1f, 0f, 1.85f) * Transformation.Scaling(0.76f), material: pigMat);
            Shape nose = new Cylinder(new Point(-2.8f, 0f, 1.8f), 0.25f, 0.3f, Constant.VEC_X, pigMat);
            Shape nostril1 = new Sphere(Transformation.Translation(-2.95f, 0.1f, 1.8f) * Transformation.Scaling(0.05f, 0.05f, 0.1f), blackMat);
            Shape nostril2 = new Sphere(Transformation.Translation(-2.95f, -0.1f, 1.8f) * Transformation.Scaling(0.05f, 0.05f, 0.1f), blackMat);
            Shape eye1 = new Sphere(Transformation.Translation(-2.65f, 0.22f, 2.35f) * Transformation.Scaling(0.165f), whiteMat);
            Shape eye2 = new Sphere(Transformation.Translation(-2.65f, -0.22f, 2.35f) * Transformation.Scaling(0.165f), whiteMat);
            Shape pupil1 = new Sphere(Transformation.Translation(-2.76f, 0.2f, 2.3f) * Transformation.Scaling(0.06f), blackMat);
            Shape pupil2 = new Sphere(Transformation.Translation(-2.76f, -0.2f, 2.3f) * Transformation.Scaling(0.06f), blackMat);
            Shape eyebrow1 = new Cylinder(new Point(-2.65f, 0.22f, 2.57f), 0.03f, 0.32f, new Vec(0f, 1f, -0.1f), blackMat);
            Shape eyebrow2 = new Cylinder(new Point(-2.65f, -0.24f, 2.57f), 0.03f, 0.3f, new Vec(0f, 1f, 0.4f), blackMat);
            Shape mouth = new Sphere(Transformation.Translation(-2.6f, 0f, 1.4f) * Transformation.Scaling(0.2f), blackMat);

            Shape head = ball + nose - nostril1 - nostril2 + eye1 + eye2 + pupil1 + pupil2 + eyebrow1 + eyebrow2 - mouth;

            // Construct Pig
            Shape pig = body + head;

            // Global Transformation
            if (transf.HasValue) pig.transformation = transf.Value;
            return pig;
        }
    }
}