using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoParticleHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE; }
        }

        public GeoParticleHeader()
        {
        }

        public GeoParticleHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);

            return this;
        }
    }
}
