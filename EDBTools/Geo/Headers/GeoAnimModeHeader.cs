using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoAnimModeHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x4; }
        }

        public uint NumAnimModes { get; private set; }

        public GeoAnimModeHeader()
        {
        }

        public GeoAnimModeHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            NumAnimModes = reader.ReadUInt32(bigEndian);

            return this;
        }
    }
}
