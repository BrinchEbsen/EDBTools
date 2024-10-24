using EDBTools.Geo.SpreadSheet.Data;
using Extensions;
using System;
using System.IO;

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
            int type = reader.ReadInt32(bigEndian);
            if (Enum.IsDefined(typeof(SpreadSheetTypes), type))
            {
                Type = (SpreadSheetTypes)type;
            } else
            {
                throw new IOException("Error reading spreadsheet header: Unknown spreadsheet type: " + type);
            }

            return this;
        }
    }
}
