using System.IO;

namespace EDBTools.Geo.Headers
{
    public class RefPointerHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return 0x10; }
        }

        public RefPointerHeader()
        {
        }

        public RefPointerHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);

            return this;
        }
    }
}
