using EDBTools.Geo.SpreadSheet;
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

        public SpreadSheetTypes Type { get; private set; }

        public GeoSpreadSheetHeader()
        {
        }

        public GeoSpreadSheetHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            uint type = reader.ReadUInt32();
            if (Enum.IsDefined(typeof(SpreadSheetTypes), type))
            {
                Type = (SpreadSheetTypes)reader.ReadUInt32(bigEndian);
            } else
            {
                throw new IOException("Error reading spreadsheet header: Unknown spreadsheet type: " + type);
            }

            return this;
        }
    }
}
