using EDBTools.Common;
using Extensions;
using System.IO;

namespace EDBTools.Geo
{
    /// <summary>
    /// Descriptor for an array of data contained within a GeoFile.
    /// </summary>
    public class GeoRelArray
    {
        public long Address { get; set; }

        /// <summary>
        /// Size of the referenced array.
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// Relative pointer to the data contained in the referenced array.
        /// </summary>
        public RelPtr Offset { get; private set; }

        public GeoRelArray(BinaryReader reader, bool bigEndian)
        {
            Address = reader.BaseStream.Position;
            Size = reader.ReadInt32(bigEndian);
            Offset = new RelPtr(reader, bigEndian);
        }

        public override string ToString()
        {
            return string.Format("Size: {0} | Relative Offset: {1}", Size, Offset.Offset);
        }
    }
}
