using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoSectionHeader : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get { return 0x10; }
        }

        public uint StartOffset { get; private set; }
        public uint EndOffset { get; private set; }

        public GeoSectionHeader()
        {
        }

        public GeoSectionHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            HashCode = reader.ReadUInt32(bigEndian);
            StartOffset = reader.ReadUInt32(bigEndian);
            EndOffset = reader.ReadUInt32(bigEndian);

            return this;
        }
    }
}
