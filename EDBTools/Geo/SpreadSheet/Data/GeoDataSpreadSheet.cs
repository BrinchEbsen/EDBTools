using EDBTools.Common;
using EDBTools.Geo.Headers;
using EDBTools.Geo.SpreadSheet.Serialization;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDBTools.Geo.SpreadSheet
{
    /// <summary>
    /// A spreadsheet that contains various data contained in one or more <see cref="DataSheet"/> objects.
    /// </summary>
    public class GeoDataSpreadSheet : BaseSpreadSheet
    {
        /// <summary>
        /// Amount of datasheets contained in this spreadsheet.
        /// </summary>
        public int NumDataSheets { get; private set; }

        /// <summary>
        /// Pointers to each datasheet contained in this spreadsheet.
        /// </summary>
        public List<RelPtr> DataSheetsOffsets { get; private set; } = new List<RelPtr>();

        /// <summary>
        /// Formatting for this spreadsheet.
        /// </summary>
        public SpreadSheetFormat Format { get; private set; }

        /// <summary>
        /// List of datasheets that could be read in this spreadsheet.
        /// Will only contain datasheets that were defined in <see cref="Format"/>.
        /// </summary>
        public List<DataSheet> DataSheets { get; private set; } = new List<DataSheet>();

        public GeoDataSpreadSheet(uint hashCode)
        {
            this.Type = SpreadSheetTypes.SHEET_TYPE_DATA;
            HashCode = hashCode;
        }

        /// <summary>
        /// Read the data for this spreadsheet without any formatting.
        /// </summary>
        /// <param name="header">GeoHeader for this spreadsheet.</param>
        /// <returns>This object, now with its fields populated with data read from the binary.</returns>
        public GeoDataSpreadSheet ReadFromFile(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header)
        {
            return ReadFromFile(reader, bigEndian, header, null);
        }

        /// <summary>
        /// Read the data for this spreadsheet.
        /// </summary>
        /// <param name="header">GeoHeader for this spreadsheet.</param>
        /// <param name="format">Formatting information for the datasheets.</param>
        /// <returns>This object, now with its fields populated with data read from the binary.</returns>
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
                ReadDataSheets(reader, bigEndian, Format);
            }

            return this;
        }

        /// <summary>
        /// Read all the data for this spreadsheet's datasheets using the given formatting.
        /// Will only read datasheets with formatting defined for it.
        /// </summary>
        /// <param name="format">Format for the spreadsheet.</param>
        private void ReadDataSheets(BinaryReader reader, bool bigEndian, SpreadSheetFormat format)
        {
            for (int i = 0; i < DataSheetsOffsets.Count; i++)
            {
                //Skip this datasheet if no formatting is defined for it
                if (!format.Sheets.ContainsKey(i))
                {
                    continue;
                }

                //Seek to start of sheet data
                long addr = DataSheetsOffsets[i].AbsoluteAddress;
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                //Read sheet
                DataSheets.Add(new DataSheet(reader, bigEndian, format.Sheets[i]));
            }
        }

        /// <summary>
        /// Get string representations of all datasheets in this spreadsheet.
        /// </summary>
        /// <returns></returns>
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
