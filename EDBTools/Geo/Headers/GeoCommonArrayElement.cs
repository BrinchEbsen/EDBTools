using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    /// <summary>
    /// Common structure for a data array header with a section number and an address to its referenced data.
    /// </summary>
    public abstract class GeoCommonArrayElement : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get
            {
                return 0x10;
            }
        }

        /// <summary>
        /// Index of the section the data is present in.
        /// </summary>
        public short Section { get; private set; }
        /// <summary>
        /// Debug data.
        /// </summary>
        public short Debug { get; private set; }
        /// <summary>
        /// Address of the referenced data element.
        /// </summary>
        public long Address { get; private set; }

        public GeoCommonArrayElement()
        {
        }

        public GeoCommonArrayElement(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            HashCode = reader.ReadUInt32(bigEndian);
            Section = reader.ReadInt16(bigEndian);
            Debug = reader.ReadInt16(bigEndian);
            Address = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(4, SeekOrigin.Current);

            return this;
        }
    }
}
