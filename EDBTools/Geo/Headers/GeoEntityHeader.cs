using EDBTools.Common;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoEntityHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x4; }
        }

        public RelPtr LodTable { get; private set; }

        public GeoEntityHeader()
        {
        }

        public GeoEntityHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            LodTable = new RelPtr(reader, bigEndian);

            return this;
        }
    }
}
