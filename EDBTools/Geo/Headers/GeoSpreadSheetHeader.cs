using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoSpreadSheetHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x4; }
        }

        public uint Type { get; private set; }

        public GeoSpreadSheetHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
            Type = reader.ReadUInt32(bigEndian);
        }
    }
}
