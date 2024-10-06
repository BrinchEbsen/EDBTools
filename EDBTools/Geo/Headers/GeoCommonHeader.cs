using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    /// <summary>
    /// Header for a data element with a hashcode and references to the data.
    /// </summary>
    public abstract class GeoCommonHeader
    {
        /// <summary>
        /// Size of the data header in bytes.
        /// </summary>
        public virtual long HEADER_SIZE { get; private set; }

        /// <summary>
        /// Address of the data header in bytes.
        /// </summary>
        public long HeaderAddress { get; protected set; }

        /// <summary>
        /// Hashcode of the data element this header references.
        /// </summary>
        public uint HashCode { get; protected set; }

        /// <summary>
        /// Assigns own address to the current stream position of the given <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">Reader with the stream position set to the start of the header's data.</param>
        protected GeoCommonHeader(BinaryReader reader)
        {
            HeaderAddress = reader.BaseStream.Position;
        }
    }
}
