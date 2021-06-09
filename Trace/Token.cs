// daje 

namespace Trace
{

    /// <summary>
    ///  A lexical token, used when parsing a scene file.
    /// </summary>
    public class Token
    {

        /// <summary>
        /// The location of the source file
        /// </summary>
        public SourceLocation sourceLoc;

        public Token(SourceLocation sL)
        {
            this.sourceLoc = sL;

        }

    }

    /// <summary>
    /// A token representing the end of a file: this is its only use.
    /// </summary>
    public class StopToken : Token
    {

        public StopToken(SourceLocation sourceLoc) : base(sourceLoc) { }

    }


    public enum KeywordEnum { 

        New=1, Material, Plane, Sphere, Diffuse, Specular, Uniform, Checkered,
        Image, Identity, Translation, RotationX, RotationY, RotationZ, Scaling,
        Camera, Orthogonal, Perspective, Float



    }


    public class KeywordToken : Token
    {
        /// <summary>
        /// A token containing a keyword
        /// </summary>

        public KeywordToken(SourceLocation sourceLoc, KeywordEnum key) : base(sourceLoc) { }

        def __init__(self, location: SourceLocation, keyword: KeywordEnum):
        super().__init__(location= location)
        self.keyword = keyword

    def __str__(self) -> str:
        return str(self.keyword)

    }

}



