using Common;
using EDBTools.Geo.Headers;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<GeoParticleHeader> ParticleHeaders { get; private set; } = new List<GeoParticleHeader>();

        public List<GeoSpreadSheetHeader> SpreadSheetHeaders { get; private set; } = new List<GeoSpreadSheetHeader>();



        /* CONSTRUCTORS */

        public GeoFile(BinaryReader reader)
        {
            //Get endianness

            bool? endian = TestEndianness(reader);
            if (!endian.HasValue)
                throw new IOException("Indeterminate endianness for the GeoFile supplied to the reader" +
                                      " - Marker value read neither \"GEOM\" or \"MOEG\".");
            BigEndian = endian.Value;

            //Get platform

            GeoHeader = new GeoHeader(reader, endian.Value);
            GamePlatform? platform = GeoHeader.TestPlatform();
            if (!platform.HasValue)
                throw new IOException("Indeterminate game platform for the GeoFile supplied to the reader.");
            Platform = platform.Value;

            //Read data array hashcodes:

            PopulateHeaderHashCodeList(GeoSectionHashCodes,  GeoHeader.SectionList,     reader, endian.Value);
            PopulateHeaderHashCodeList(RefPointerHashCodes,  GeoHeader.RefPointerList,  reader, endian.Value);
            PopulateHeaderHashCodeList(EntityHashCodes,      GeoHeader.EntityList,      reader, endian.Value);
            PopulateHeaderHashCodeList(AnimHashCodes,        GeoHeader.AnimList,        reader, endian.Value);
            PopulateHeaderHashCodeList(AnimSkinHashCodes,    GeoHeader.AnimSkinList,    reader, endian.Value);
            PopulateHeaderHashCodeList(ScriptHashCodes,      GeoHeader.ScriptList,      reader, endian.Value);
            PopulateHeaderHashCodeList(MapHashCodes,         GeoHeader.MapList,         reader, endian.Value);
            PopulateHeaderHashCodeList(AnimModeHashCodes,    GeoHeader.AnimModeList,    reader, endian.Value);
            PopulateHeaderHashCodeList(AnimSetHashCodes,     GeoHeader.AnimSetList,     reader, endian.Value);
            PopulateHeaderHashCodeList(ParticleHashCodes,    GeoHeader.ParticleList,    reader, endian.Value);
            PopulateHeaderHashCodeList(SwooshHashCodes,      GeoHeader.SwooshList,      reader, endian.Value);
            PopulateHeaderHashCodeList(SpreadSheetHashCodes, GeoHeader.SpreadSheetList, reader, endian.Value);
            PopulateHeaderHashCodeList(FontHashCodes,        GeoHeader.FontList,        reader, endian.Value);
            PopulateHeaderHashCodeList(TextureHashCodes,     GeoHeader.TextureList,     reader, endian.Value);

            //Read data arrays:
            long addr;
            
            //Sections
            addr = GeoHeader.SectionList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.SectionList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                SectionHeaders.Add(new GeoSectionHeader(reader, endian.Value));
                addr += SectionHeaders[i].HEADER_SIZE;
            }

            //Ref Pointers
            addr = GeoHeader.RefPointerList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.RefPointerList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                RefPointerHeaders.Add(new RefPointerHeader(reader, endian.Value));
                addr += RefPointerHeaders[i].HEADER_SIZE;
            }

            //Entities
            addr = GeoHeader.EntityList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.EntityList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                EntityHeaders.Add(new GeoEntityHeader(reader, endian.Value));
                addr += EntityHeaders[i].HEADER_SIZE;
            }

            //Animations
            addr = GeoHeader.AnimList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.AnimList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                AnimHeaders.Add(new GeoAnimHeader(reader, endian.Value));
                addr += AnimHeaders[i].HEADER_SIZE;
            }

            //Animation skins
            addr = GeoHeader.AnimSkinList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.AnimSkinList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                AnimSkinHeaders.Add(new GeoAnimSkinHeader(reader, endian.Value));
                addr += AnimSkinHeaders[i].HEADER_SIZE;
            }

            //Scripts
            addr = GeoHeader.ScriptList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.ScriptList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                ScriptHeaders.Add(new GeoScriptHeader(reader, endian.Value));
                addr += ScriptHeaders[i].HEADER_SIZE;
            }

            //Particles
            addr = GeoHeader.ParticleList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.ParticleList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                ParticleHeaders.Add(new GeoParticleHeader(reader, endian.Value));
                addr += ParticleHeaders[i].HEADER_SIZE;
            }

            //SpreadSheets
            addr = GeoHeader.SpreadSheetList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.SpreadSheetList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                SpreadSheetHeaders.Add(new GeoSpreadSheetHeader(reader, endian.Value));
                addr += SpreadSheetHeaders[i].HEADER_SIZE;
            }

            //Textures
            addr = GeoHeader.SpreadSheetList.Offset.AbsoluteAddress;
            for (int i = 0; i < GeoHeader.SpreadSheetList.ArraySize; i++)
            {
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
                SpreadSheetHeaders.Add(new GeoSpreadSheetHeader(reader, endian.Value));
                addr += SpreadSheetHeaders[i].HEADER_SIZE;
            }
        }



        /* METHODS */

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

            if (reader.BaseStream.Position != 0)
            {
                originalPos = reader.BaseStream.Position;
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
            }

            char[] marker = new char[4];
            for (int i = 0; i < 4; i++)
            {
                marker[i] = (char)reader.ReadByte();
            }

            reader.BaseStream.Seek(originalPos, SeekOrigin.Begin);

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
        /// Take a list and add the HashCodes prefixed to the data array header referenced by the given <see cref="GeoCommonArray"/>.
        /// Will only add hashcodes if <see cref="GeoCommonArray.HashSize"/> is negative.
        /// </summary>
        /// <param name="list">List to populate with HashCodes.</param>
        /// <param name="array">Data array descriptor.</param>
        /// <returns>The input list, now with added HashCodes (if any).</returns>
        private List<uint> PopulateHeaderHashCodeList(List<uint> list, GeoCommonArray array, BinaryReader reader, bool bigEndian)
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

            sb.AppendLine(string.Format("GeoFile v{0} | {1} | {2}",
                GeoHeader.Version, GeoHeader.HashCode.ToString(), platformStr));
            sb.AppendLine();

            sb.AppendLine("HEADER:");
            sb.AppendLine(GeoHeader.ToString());

            return sb.ToString();
        }
    }
}
