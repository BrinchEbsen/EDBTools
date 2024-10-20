using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoFontHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE; }
        }

        public GeoFontHeader()
        {
        }

        public GeoFontHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);

            return this;
        }
    }
}
