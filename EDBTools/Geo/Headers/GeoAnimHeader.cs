using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoAnimHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x10; }
        }

        public long MotionDataInfo { get; private set; }
        public uint DataSize { get; private set; }
        public uint SkinNum { get; private set; }

        public GeoAnimHeader()
        {
        }

        public GeoAnimHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            MotionDataInfo = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            DataSize = reader.ReadUInt32(bigEndian);
            SkinNum = reader.ReadUInt32(bigEndian);

            return this;
        }
    }
}
