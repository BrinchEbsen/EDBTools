using System.IO;

namespace EDBTools.Geo.Headers
{
    /// <summary>
    /// Header for a data element with a hashcode and references to the data.
    /// </summary>
    public abstract class GeoCommonHeader
    {
        /// <summary>
        /// Size of this data header in bytes.
        /// </summary>
        public virtual long HEADER_SIZE { get; }

        /// <summary>
        /// Address of the data header in bytes.
        /// </summary>
        public long HeaderAddress { get; protected set; }

        /// <summary>
        /// Hashcode of the data element this header references.
        /// </summary>
        public uint HashCode { get; protected set; }

        public GeoCommonHeader()
        {
        }

        /// <summary>
        /// Assigns own address to the current stream position of the given <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">Reader with the stream position set to the start of the header's data.</param>
        public GeoCommonHeader(BinaryReader reader, bool bigEndian)
        {
            ReadFromFile(reader, bigEndian);
        }

        /// <summary>
        /// Read the header information from the given reader's current position.
        /// </summary>
        /// <param name="reader">Reader with the current stream position set at the start of the header data.</param>
        /// <param name="bigEndian">Whether to read the data in big endian.</param> 
        /// <returns></returns>
        public virtual GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            HeaderAddress = reader.BaseStream.Position;
            return this;
        }
    }
}
