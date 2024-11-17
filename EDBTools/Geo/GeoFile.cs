using Common;
using EDBTools.Geo.Font;
using EDBTools.Geo.Headers;
using EDBTools.Geo.Map;
using EDBTools.Geo.SpreadSheet;
using EDBTools.Geo.SpreadSheet.Data;
using EDBTools.Geo.SpreadSheet.Serialization;
using EDBTools.Geo.SpreadSheet.Text;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDBTools.Geo
{
    /// <summary>
    /// <para>
    /// A <see cref="GeoFile"/> or .EDB (Eurocom DataBase) file, is a binary general-purpose container
    /// in Eurocom's EngineX titles. The file can contain any number of different assets used by the game at runtime,
    /// and is created by exporting an .ELF (EuroLand File) from Eurocom's editor, Euroland.
    /// </para>
    /// 
    /// <para>
    /// A GeoFile has a header and a list of sections, which can be loaded in pieces by the game
    /// depending on what data is needed.
    /// </para>
    /// <see href="https://sphinxandthecursedmummy.fandom.com/wiki/EDB"/>
    /// </summary>
    public class GeoFile
    {
        /* GLOBALS */

        /// <summary>
        /// Versions that will be accepted by the read methods on the Geo set of classes.
        /// </summary>
        public static readonly int[] SUPPORTED_VERSIONS = new int[] 
        {
            240 //Spyro: A Hero's Tail retail GeoFile version.
        };

        /// <summary>
        /// The sections of a GeoFile are aligned to a byte boundary which varies in size depending on the platform.
        /// </summary>
        public static readonly Dictionary<GamePlatform, int> SECTION_ALIGNMENT = new Dictionary<GamePlatform, int>
        {
            { GamePlatform.Xbox,         0x800 },
            { GamePlatform.PlayStation2, 0x800 },
            { GamePlatform.GameCube,     0x20  }
        };



        /* VARIABLES */

        /// <summary>
        /// Whether this file is stored in big or little endian.
        /// </summary>
        public bool BigEndian { get; private set; }

        /// <summary>
        /// The platform this GeoFile was outputted for.
        /// </summary>
        public GamePlatform Platform { get; private set; }

        /// <summary>
        /// The header section containing the base information for this GeoFile.
        /// </summary>
        public GeoHeader GeoHeader { get; private set; }



        /* DATA ARRAY HASHCODES */

        public List<uint> GeoSectionHashCodes  { get; private set; } = new List<uint>();
        public List<uint> RefPointerHashCodes  { get; private set; } = new List<uint>();
        public List<uint> EntityHashCodes      { get; private set; } = new List<uint>();
        public List<uint> AnimHashCodes        { get; private set; } = new List<uint>();
        public List<uint> AnimSkinHashCodes    { get; private set; } = new List<uint>();
        public List<uint> ScriptHashCodes      { get; private set; } = new List<uint>();
        public List<uint> MapHashCodes         { get; private set; } = new List<uint>();
        public List<uint> AnimModeHashCodes    { get; private set; } = new List<uint>();
        public List<uint> AnimSetHashCodes     { get; private set; } = new List<uint>();
        public List<uint> ParticleHashCodes    { get; private set; } = new List<uint>();
        public List<uint> SwooshHashCodes      { get; private set; } = new List<uint>();
        public List<uint> SpreadSheetHashCodes { get; private set; } = new List<uint>();
        public List<uint> FontHashCodes        { get; private set; } = new List<uint>();
        public List<uint> TextureHashCodes     { get; private set; } = new List<uint>();



        /* DATA ARRAY HEADERS */

        public List<GeoSectionHeader> SectionHeaders { get; private set; } = new List<GeoSectionHeader>();
        public List<RefPointerHeader> RefPointerHeaders { get; private set; } = new List<RefPointerHeader>();
        public List<GeoEntityHeader> EntityHeaders { get; private set; } = new List<GeoEntityHeader>();
        public List<GeoAnimHeader> AnimHeaders { get; private set; } = new List<GeoAnimHeader>();
        public List<GeoAnimSkinHeader> AnimSkinHeaders { get; private set; } = new List<GeoAnimSkinHeader>();
        public List<GeoScriptHeader> ScriptHeaders { get; private set; } = new List<GeoScriptHeader>();
        public List<GeoMapHeader> MapHeaders { get; private set; } = new List<GeoMapHeader>();
        public List<GeoAnimModeHeader> AnimModeHeaders { get; private set; } = new List<GeoAnimModeHeader>();
        public List<GeoAnimSetHeader> AnimSetHeaders { get; private set; } = new List<GeoAnimSetHeader>();
        public List<GeoParticleHeader> ParticleHeaders { get; private set; } = new List<GeoParticleHeader>();
        public List<GeoSwooshHeader> SwooshHeaders { get; private set; } = new List<GeoSwooshHeader>();
        public List<GeoSpreadSheetHeader> SpreadSheetHeaders { get; private set; } = new List<GeoSpreadSheetHeader>();
        public List<GeoFontHeader> FontHeaders { get; private set; } = new List<GeoFontHeader>();
        public List<GeoTextureHeader> TextureHeaders { get; private set; } = new List<GeoTextureHeader>();



        /* DATA */

        public List<GeoMap> Maps { get; private set; }

        public List<BaseSpreadSheet> SpreadSheets { get; private set; }

        public List<GeoFont> Fonts { get; private set; }



        /* CONSTRUCTORS */

        public GeoFile(BinaryReader reader)
        {
            //Get endianness

            bool? endian = TestEndianness(reader);
            if (!endian.HasValue)
                throw new IOException("Indeterminate endianness for the GeoFile supplied to the reader" +
                                      " - Marker value read neither \"GEOM\" or \"MOEG\".");
            BigEndian = endian.Value;

            //Read header and get platform

            GeoHeader = new GeoHeader(reader, BigEndian);

            GamePlatform? platform = GeoHeader.TestPlatform();
            if (!platform.HasValue)
                throw new IOException("Indeterminate game platform for the GeoFile supplied to the reader.");
            Platform = platform.Value;

            //Read data array hashcodes:

            PopulateHeaderHashCodeList(GeoSectionHashCodes,  GeoHeader.SectionList,     reader, BigEndian);
            PopulateHeaderHashCodeList(RefPointerHashCodes,  GeoHeader.RefPointerList,  reader, BigEndian);
            PopulateHeaderHashCodeList(EntityHashCodes,      GeoHeader.EntityList,      reader, BigEndian);
            PopulateHeaderHashCodeList(AnimHashCodes,        GeoHeader.AnimList,        reader, BigEndian);
            PopulateHeaderHashCodeList(AnimSkinHashCodes,    GeoHeader.AnimSkinList,    reader, BigEndian);
            PopulateHeaderHashCodeList(ScriptHashCodes,      GeoHeader.ScriptList,      reader, BigEndian);
            PopulateHeaderHashCodeList(MapHashCodes,         GeoHeader.MapList,         reader, BigEndian);
            PopulateHeaderHashCodeList(AnimModeHashCodes,    GeoHeader.AnimModeList,    reader, BigEndian);
            PopulateHeaderHashCodeList(AnimSetHashCodes,     GeoHeader.AnimSetList,     reader, BigEndian);
            PopulateHeaderHashCodeList(ParticleHashCodes,    GeoHeader.ParticleList,    reader, BigEndian);
            PopulateHeaderHashCodeList(SwooshHashCodes,      GeoHeader.SwooshList,      reader, BigEndian);
            PopulateHeaderHashCodeList(SpreadSheetHashCodes, GeoHeader.SpreadSheetList, reader, BigEndian);
            PopulateHeaderHashCodeList(FontHashCodes,        GeoHeader.FontList,        reader, BigEndian);
            PopulateHeaderHashCodeList(TextureHashCodes,     GeoHeader.TextureList,     reader, BigEndian);

            //Read data arrays:

            PopulateHeaderList<GeoSectionHeader>    (SectionHeaders,     GeoHeader.SectionList,     reader, BigEndian);
            PopulateHeaderList<RefPointerHeader>    (RefPointerHeaders,  GeoHeader.RefPointerList,  reader, BigEndian);
            PopulateHeaderList<GeoEntityHeader>     (EntityHeaders,      GeoHeader.EntityList,      reader, BigEndian);
            PopulateHeaderList<GeoAnimHeader>       (AnimHeaders,        GeoHeader.AnimList,        reader, BigEndian);
            PopulateHeaderList<GeoAnimSkinHeader>   (AnimSkinHeaders,    GeoHeader.AnimSkinList,    reader, BigEndian);
            PopulateHeaderList<GeoScriptHeader>     (ScriptHeaders,      GeoHeader.ScriptList,      reader, BigEndian);
            PopulateHeaderList<GeoMapHeader>        (MapHeaders,         GeoHeader.MapList,         reader, BigEndian);
            PopulateHeaderList<GeoAnimModeHeader>   (AnimModeHeaders,    GeoHeader.AnimModeList,    reader, BigEndian);
            PopulateHeaderList<GeoAnimSetHeader>    (AnimSetHeaders,     GeoHeader.AnimSetList,     reader, BigEndian);
            PopulateHeaderList<GeoParticleHeader>   (ParticleHeaders,    GeoHeader.ParticleList,    reader, BigEndian);
            PopulateHeaderList<GeoSwooshHeader>     (SwooshHeaders,      GeoHeader.SwooshList,      reader, BigEndian);
            PopulateHeaderList<GeoSpreadSheetHeader>(SpreadSheetHeaders, GeoHeader.SpreadSheetList, reader, BigEndian);
            PopulateHeaderList<GeoFontHeader>       (FontHeaders,        GeoHeader.FontList,        reader, BigEndian);
            PopulateHeaderList<GeoTextureHeader>    (TextureHeaders,     GeoHeader.TextureList,     reader, BigEndian);
        }



        /* METHODS */

        public List<GeoMap> ReadMaps(BinaryReader reader, bool bigEndian)
        {
            Maps = new List<GeoMap>(MapHeaders.Count);

            if (MapHeaders.Count == 0) { return Maps; }

            foreach(var header in MapHeaders)
            {
                GeoMap map = new GeoMap();
                Maps.Add(map.ReadFromStream(reader, bigEndian, header));
            }

            return Maps;
        }

        public List<BaseSpreadSheet> ReadSpreadSheets(BinaryReader reader, bool bigEndian)
        {
            return ReadSpreadSheets(reader, bigEndian, (SpreadSheetGeoFileFormat)null);
        }

        public List<BaseSpreadSheet> ReadSpreadSheets(BinaryReader reader, bool bigEndian, SpreadSheetCollectionFormat format)
        {
            SpreadSheetGeoFileFormat sheetFmt = null;
            if (format.GeoFiles.ContainsKey(GeoHeader.HashCode))
            {
                sheetFmt = format.GeoFiles[GeoHeader.HashCode];
            }

            return ReadSpreadSheets(reader, bigEndian, sheetFmt);
        }

        public List<BaseSpreadSheet> ReadSpreadSheets(BinaryReader reader, bool bigEndian, SpreadSheetGeoFileFormat format)
        {
            SpreadSheets = new List<BaseSpreadSheet> (SpreadSheetHeaders.Count);

            if (SpreadSheetHeaders.Count == 0) { return SpreadSheets; }

            foreach(var header in SpreadSheetHeaders)
            {
                switch (header.Type)
                {
                    case SpreadSheetTypes.SHEET_TYPE_TEXT:
                    {
                        GeoTextSpreadSheet sheet = new GeoTextSpreadSheet(header.HashCode);
                        sheet.ReadFromStream(reader, bigEndian, header, SectionHeaders);
                        SpreadSheets.Add(sheet);
                    }
                    break;
                    case SpreadSheetTypes.SHEET_TYPE_DATA:
                    {
                        SpreadSheetFormat sheetFormat = null;

                        //Assign format for this sheet if it exists
                        if (format != null)
                        {
                            if (format.SpreadSheets.ContainsKey(header.HashCode))
                            {
                                sheetFormat = format.SpreadSheets[header.HashCode];
                            }
                        }

                        GeoDataSpreadSheet sheet = new GeoDataSpreadSheet(header.HashCode);
                        sheet.ReadFromStream(reader, bigEndian, header, sheetFormat);
                        SpreadSheets.Add(sheet);
                    }
                    break;
                }
            }

            return SpreadSheets;
        }

        public List<GeoFont> ReadFonts(BinaryReader reader, bool bigEndian)
        {
            Fonts = new List<GeoFont>(FontHeaders.Count);

            if (FontHeaders.Count == 0) { return Fonts; }

            foreach (var header in FontHeaders)
            {
                GeoFont font = new GeoFont();
                Fonts.Add(font.ReadFromStream(reader, bigEndian, header));
            }

            return Fonts;
        }

        /// <summary>
        /// Read the marker file on the GeoFile and determine the endianness it was encoded with.
        /// Will return the stream's position to where it was before reading.
        /// </summary>
        /// <param name="reader">Reader containing the filestream of the GeoFile to be read.</param>
        /// <returns>True if the file is in big endian, false if not. Null if the endianness could not be determined.</returns>
        public static bool? TestEndianness(BinaryReader reader)
        {
            //Store original stream position so it can be restored later.
            long originalPos = 0;

            //Seek to start of file
            if (reader.BaseStream.Position != 0)
            {
                originalPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }

            //Read 4-byte marker
            char[] marker = new char[4];
            for (int i = 0; i < 4; i++)
            {
                marker[i] = (char)reader.ReadByte();
            }

            //seek back to original position
            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

            //Test if marker reads "GEOM" (big endian) or the reverse, "MOEG" (little endian).
            string markerStr = new string(marker);
            switch (markerStr)
            {
                case "GEOM":
                    return true;
                case "MOEG":
                    return false;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Take a list and add the HashCodes prefixed to the data array header referenced by the given <see cref="GeoArray"/>.
        /// Will only add hashcodes if <see cref="GeoArray.HashSize"/> is negative.
        /// </summary>
        /// <param name="list">List to populate with HashCodes.</param>
        /// <param name="array">Data array descriptor.</param>
        /// <returns>The input list, now with added HashCodes (if any).</returns>
        private List<uint> PopulateHeaderHashCodeList(List<uint> list, GeoArray array, BinaryReader reader, bool bigEndian)
        {
            if (array.HashSize >= 0) return list;

            long startAddress = array.Offset.AbsoluteAddress + (array.HashSize * 4);
            reader.BaseStream.Seek(startAddress, SeekOrigin.Begin);

            for(int i = 0; i < Math.Abs(array.HashSize); i++)
            {
                list.Add(reader.ReadUInt32(bigEndian));
            }

            return list;
        }

        /// <summary>
        /// Take a list and add the data array headers referenced by the given <see cref="GeoArray"/>.
        /// </summary>
        /// <typeparam name="T">A header which derives from <see cref="GeoArray"/>
        /// and contains a parameterless constructor.</typeparam>
        /// <param name="list">List to populate with headers.</param>
        /// <param name="array">Data array descriptor.</param>
        /// <returns>The input list, now with added headers (if any).</returns>
        private List<T> PopulateHeaderList<T>
            (List<T> list, GeoArray array, BinaryReader reader, bool bigEndian) 
            where T : GeoCommonHeader, new()
        {
            list.Clear();

            long addr = array.Offset.AbsoluteAddress;
            for (int i = 0; i < array.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                GeoCommonHeader header = new T();
                list.Insert(i, header.ReadFromFile(reader, bigEndian) as T);
                addr += header.HEADER_SIZE;
            }

            return list;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            string platformStr;

            switch (Platform)
            {
                case GamePlatform.Xbox:         platformStr = "Xbox"; break;
                case GamePlatform.PlayStation2: platformStr = "PlayStation 2"; break;
                case GamePlatform.GameCube:     platformStr = "GameCube"; break;
                default: platformStr = "Unknown"; break;
            }

            sb.AppendLine(string.Format("GeoFile v{0} | {1:X} | {2}",
                GeoHeader.Version, GeoHeader.HashCode, platformStr));
            sb.AppendLine();

            sb.AppendLine("HEADER:");
            sb.AppendLine(GeoHeader.ToString());

            return sb.ToString();
        }
    }
}
