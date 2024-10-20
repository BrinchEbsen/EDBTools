using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoAnimSkinHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0xC; }
        }

        public uint BaseSkinNum { get; private set; }
        public uint MipRef { get; private set; }
        public float MipDistance { get; private set; }

        public GeoAnimSkinHeader()
        {
        }

        public GeoAnimSkinHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            BaseSkinNum = reader.ReadUInt32(bigEndian);
            MipRef = reader.ReadUInt32(bigEndian);
            MipDistance = reader.ReadSingle(bigEndian);

            return this;
        }
    }
}
