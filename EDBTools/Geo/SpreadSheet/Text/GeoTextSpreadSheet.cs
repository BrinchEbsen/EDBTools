using EDBTools.Geo.Headers;
using EDBTools.Geo.SpreadSheet.Data;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDBTools.Geo.SpreadSheet.Text
{
    /// <summary>
    /// A section of a GeoFile containing text data.
    /// </summary>
    public struct TextSection
    {
        public uint HashCode;
        public int Section;
    }

    /// <summary>
    /// Contains text-related data split into sections that can be loaded seperately.
    /// </summary>
    public class GeoTextSpreadSheet : BaseSpreadSheet
    {
        public GeoTextSpreadSheet(uint hashcode)
        {
            Type = SpreadSheetTypes.SHEET_TYPE_TEXT;
            HashCode = hashcode;
        }

        /// <summary>
        /// Number of text data sections referenced by this spreadsheet.
        /// </summary>
        public int NumSections { get; private set; }

        /// <summary>
        /// List of sections with text data.
        /// </summary>
        public List<TextSection> TextSectionList { get; private set; }

        /// <summary>
        /// Sections containing text data, indexed from the section after
        /// the one containing the base list for this spreadsheet.
        /// 
        /// <para>Key: Section index, Value: Text Data Object</para>
        /// </summary>
        public Dictionary<int, TextSheetData> TextData { get; private set; }

        public GeoTextSpreadSheet ReadFromStream(BinaryReader reader, bool bigEndian, GeoSpreadSheetHeader header, List<GeoSectionHeader> sectionList)
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

            NumSections = reader.ReadInt32(bigEndian);

            TextSectionList = new List<TextSection>();

            for(int i = 0; i < NumSections; i++)
            {
                TextSectionList.Add(new TextSection()
                {
                    HashCode = reader.ReadUInt32(bigEndian),
                    Section = reader.ReadInt32(bigEndian)
                });
            }

            TextData = new Dictionary<int, TextSheetData>();

            foreach(var textSection in TextSectionList)
            {
                //Sections containing text data starts one section ahead of the section containing the list of text sections (word salad i know)
                var section = sectionList[header.Section + textSection.Section + 1];

                reader.BaseStream.Seek(section.StartOffset, SeekOrigin.Begin);
                TextData.Add(textSection.Section, new TextSheetData(reader, bigEndian));
            }

            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("Text SpreadSheet {0:X}, Sections: {1}", HashCode, TextSectionList.Count));

            foreach(var textSection in TextSectionList)
            {
                sb.AppendLine(string.Format("Text Section {0:X}:", textSection.HashCode));
                sb.AppendLine(TextData[textSection.Section].ToString());
            }

            return sb.ToString();
        }
    }
}
