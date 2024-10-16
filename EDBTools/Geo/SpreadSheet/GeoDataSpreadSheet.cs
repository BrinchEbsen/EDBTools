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
    public class GeoDataSpreadSheet : BaseSpreadSheet
    {
        public long Address { get; private set; }
        public uint HashCode { get; private set; }
        public int NumDataSheets { get; private set; }
        public List<RelPtr> DataSheetsOffsets { get; private set; } = new List<RelPtr>();

        public SpreadSheetFormat Format { get; private set; }

        public List<DataSheet> DataSheets { get; private set; } = new List<DataSheet>();

        public GeoDataSpreadSheet(uint hashCode)
        {
            this.Type = SpreadSheetTypes.SHEET_TYPE_DATA;
            HashCode = hashCode;
        }

        public GeoDataSpreadSheet ReadFromFile(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header)
        {
            return ReadFromFile(reader, bigEndian, header, null);
        }

        public GeoDataSpreadSheet ReadFromFile(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header, SpreadSheetFormat format)
        {
            //Check that the type matches before reading
            if (header.Type != this.Type)
            {
                throw new ArgumentException("Spreadsheet type mismatch: This method reads spreadsheets of type " + this.Type +
                    " - supplied header is for spreadsheet of type " + header.Type + ".");
            }

            //Seek to start of data
            reader.BaseStream.Seek(header.Address, SeekOrigin.Begin);
            this.Address = header.Address;

            this.Format = format;

            //Read data sheet pointers
            NumDataSheets = reader.ReadInt32(bigEndian);

            if (NumDataSheets == 0) { return this; }

            for (int i = 0; i < NumDataSheets; i++)
            {
                DataSheetsOffsets.Add(new RelPtr(reader, bigEndian));
            }

            //If we've got a format supplied, read the spreadsheet data
            if (Format != null)
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
                    //throw new IOException("Could not read data sheet, because the format does not specify anything for sheet with index " + i + ".");
                    continue;
                }

                //Seek to start of sheet data
                long addr = DataSheetsOffsets[i].AbsoluteAddress;
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                //Read sheet
                DataSheets.Add(new DataSheet(reader, bigEndian, format.Sheets[i]));
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine(string.Format("SpreadSheet {0:X}, {1} DataSheet", HashCode, NumDataSheets)
                + (NumDataSheets == 1 ? "" : "s"));

            if (DataSheets.Count == 0)
            {
                str.AppendLine("No format provided.");
                return str.ToString();
            }

            for (int i = 0; i < DataSheets.Count; i++)
            {
                str.AppendLine("DataSheet " + i + ":");
                str.AppendLine(DataSheets[i].ToString());
            }

            return str.ToString();
        }
    }
}
