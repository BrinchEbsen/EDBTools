using EDBTools.Common;
using EDBTools.Geo.Headers;
using EDBTools.Geo.SpreadSheet.Serialization;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet
{
    public class GeoDataSpreadSheet
    {
        public long Address { get; private set; }
        public int NumDataSheets { get; private set; }
        public List<RelPtr> DataSheetsOffsets { get; private set; } = new List<RelPtr>();

        public List<DataSheet> DataSheets { get; private set; } = new List<DataSheet>();

        public GeoDataSpreadSheet()
        {
            
        }

        public GeoDataSpreadSheet ReadFromFile(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header)
        {
            return ReadFromFile(reader, bigEndian, header, null);
        }

        public GeoDataSpreadSheet ReadFromFile(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header, SpreadSheetFormat format)
        {
            //Seek to start of data
            reader.BaseStream.Seek(header.Address, SeekOrigin.Begin);
            this.Address = header.Address;

            //Read data sheet offsets
            NumDataSheets = reader.ReadInt32(bigEndian);

            if (NumDataSheets == 0) { return this; }

            for (int i = 0; i < NumDataSheets; i++)
            {
                DataSheetsOffsets.Add(new RelPtr(reader, bigEndian));
            }

            if (format != null)
            {
                ReadDataSheets(reader, bigEndian, format);
            }

            return this;
        }

        public void ReadDataSheets(BinaryReader reader, bool bigEndian, SpreadSheetFormat format)
        {
            for (int i = 0; i < DataSheetsOffsets.Count; i++)
            {
                if (!format.Sheets.ContainsKey(i))
                {
                    throw new IOException("Could not read data sheet, because the format does not specify anything for sheet with index " + i);
                }

                long addr = DataSheetsOffsets[i].AbsoluteAddress;
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                DataSheets.Add(new DataSheet(reader, bigEndian, format.Sheets[i]));
            }
        }
    }
}
