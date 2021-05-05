using System;
using System.Numerics;

namespace Trace
{

    /// <summary>
    /// Class to represent the rendering environment. It is a collection of all the present 3D shapes.
    /// This class is used to calculate ray intersections with objects and to determine the closest ones.
    /// </summary>
    public class World 
    {

        /// <summary>
        ///  Main field of the World class. It is a list of <see cref="Shape"/>
        ///  objects present in the environment.
        /// </summary>
        public List<Trace.Shape> shapes;

        public void addShape(Shape sh)
          => shapes.Add(sh);





    } // end of World

} // end of Trace