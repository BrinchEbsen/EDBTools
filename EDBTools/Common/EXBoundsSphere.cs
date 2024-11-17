using Extensions;
using System.IO;

namespace EDBTools.Common
{
    public class EXBoundsSphere
    {
        public EXVector3 Position { get; set; }
        public float Radius { get; set; }

        public EXBoundsSphere() { }

        public EXBoundsSphere(float x, float y, float z, float radius)
        {
            Position = new EXVector3(x, y, z);
            Radius = radius;
        }

        public EXBoundsSphere(EXVector3 pos, float radius)
        {
            Position = new EXVector3(pos);
            Radius = radius;
        }

        public EXBoundsSphere(BinaryReader reader, bool bigEndian)
        {
            Position = new EXVector3(reader, bigEndian);
            Radius = reader.ReadSingle(bigEndian);
        }
    }
}
