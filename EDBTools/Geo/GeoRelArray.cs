using EDBTools.Common;
using Extensions;
using System.IO;

namespace EDBTools.Geo
{
    public class GeoRelArray
    {
        public int Size { get; private set; }
        public RelPtr Offset { get; private set; }

        public GeoRelArray(BinaryReader reader, bool bigEndian)
        {
            Size = reader.ReadInt32(bigEndian);
            Offset = new RelPtr(reader, bigEndian);
        }

        public override string ToString()
        {
            return string.Format("Size: {0} | Relative Offset: {1}", Size, Offset.Offset);
        }
    }
}
