using Extensions;
using System;
using System.IO;

namespace EDBTools.Common
{
    /// <summary>
    /// Represents a point or vector with three <see cref="float"/> components.
    /// </summary>
    public class EXVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public double Length
        {
            get
            {
                return Math.Sqrt((X*X) + (Y*Y) + (Z*Z));
            }
        }

        public EXVector3()
        {
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public EXVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public EXVector3(BinaryReader reader, bool bigEndian)
        {
            ReadFromStream(reader, bigEndian);
        }

        public void ReadFromStream(BinaryReader reader, bool bigEndian)
        {
            X = reader.ReadSingle(bigEndian);
            Y = reader.ReadSingle(bigEndian);
            Z = reader.ReadSingle(bigEndian);
        }

        public override string ToString()
        {
            return string.Format("X: {0:0.000}\nY: {1:0.000}\nZ: {2:0.000}", X, Y, Z);
        }
    }
}
