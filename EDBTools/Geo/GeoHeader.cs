﻿using Common;
using EDBTools.Geo;
using Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace EDBTools
{
    /// <summary>
    /// The header of a GeoFile, which will contain basic information such as
    /// version number, hashcode, filesize etc.,
    /// as well as a set of descriptors for each type of asset stored within the GeoFile.
    /// </summary>
    public class GeoHeader
    {
        /* VARIABLES */

        /// <summary>
        /// <para>The 4-byte marker at the very start of the GeoFile.</para>
        /// <para>Works as a magic value to identify the file to be a GeoFile.
        /// Will read "GEOM" when big endian and "MOEG" when little endian.</para>
        /// </summary>
        public char[] Marker { get; private set; }

        /// <summary>
        /// HashCode for this file. Section HT_File (0x01XXXXXX).
        /// </summary>
        public uint HashCode { get; private set; }

        /// <summary>
        /// The GeoFile version. Retail Spyro: A Hero's Tail uses version 240.
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Flags enumerating platform, content-type and Euroland-provided information at output time.
        /// </summary>
        public uint Flags { get; private set; }

        /// <summary>
        /// 4-byte unix timestamp denoting when the source .ELF project file was last saved.
        /// </summary>
        public uint TimeStamp { get; private set; }

        /// <summary>
        /// The total file size, in bytes. With all the sections loaded, except the optional, trailing debug section. 
        /// </summary>
        public uint FileSize { get; private set; }

        /// <summary>
        /// The partial file size, as reported in the Filelist.
        /// Usually the bare minimum memory this GeoFile file will take, when only the first section is loaded. 
        /// </summary>
        public uint BaseFileSize { get; private set; }

        /// <summary>
        /// 6 platform-specific version values. First value will be 1 on Xbox.
        /// </summary>
        public uint[] Versions { get; private set; }

        /// <summary>
        /// Offset in bytes of the debug section in this GeoFile.
        /// </summary>
        public uint DebugSectionOffset { get; private set; }

        /// <summary>
        /// Offset in bytes of the end of the debug section in this GeoFile.
        /// </summary>
        public uint DebugSectionEndOffset { get; private set; }



        /* SECTION DESCRIPTORS */

        public GeoArray SectionList { get; private set; }
        public GeoArray RefPointerList { get; private set; }
        public GeoArray EntityList { get; private set; }
        public GeoArray AnimList { get; private set; }
        public GeoArray AnimSkinList { get; private set; }
        public GeoArray ScriptList { get; private set; }
        public GeoArray MapList { get; private set; }
        public GeoArray AnimModeList { get; private set; }
        public GeoArray AnimSetList { get; private set; }
        public GeoArray ParticleList { get; private set; }
        public GeoArray SwooshList { get; private set; }
        public GeoArray SpreadSheetList { get; private set; }
        public GeoArray FontList { get; private set; }
        public GeoArray TextureList { get; private set; }
        public GeoRelArray TextureUpdateList { get; private set; }



        /* CONSTRUCTORS */

        public GeoHeader(BinaryReader reader, bool bigEndian)
        {
            if (reader.BaseStream.Position != 0)
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

            Marker = new char[4];
            for (int i = 0; i < 4; i++)
            {
                Marker[i] = (char)reader.ReadByte();
            }

            HashCode = reader.ReadUInt32(bigEndian);
            Version = reader.ReadInt32(bigEndian);

            if (!GeoFile.SUPPORTED_VERSIONS.Contains(Version))
            {
                throw new IOException("Could not read contents of the GeoFile - version " + Version + " not supported!");
            }

            Flags = reader.ReadUInt32(bigEndian);
            TimeStamp = reader.ReadUInt32(bigEndian);
            FileSize = reader.ReadUInt32(bigEndian);
            BaseFileSize = reader.ReadUInt32(bigEndian);

            Versions = new uint[6];
            for (int i = 0; i < 6; i++)
            {
                Versions[i] = reader.ReadUInt32(bigEndian);
            }

            DebugSectionOffset = reader.ReadUInt32(bigEndian);
            DebugSectionEndOffset = reader.ReadUInt32(bigEndian);

            //Seek past runtime-only pointers.
            reader.BaseStream.Seek(4 * 6, SeekOrigin.Current);

            SectionList     = new GeoArray(reader, bigEndian);
            RefPointerList  = new GeoArray(reader, bigEndian);
            EntityList      = new GeoArray(reader, bigEndian);
            AnimList        = new GeoArray(reader, bigEndian);
            AnimSkinList    = new GeoArray(reader, bigEndian);
            ScriptList      = new GeoArray(reader, bigEndian);
            MapList         = new GeoArray(reader, bigEndian);
            AnimModeList    = new GeoArray(reader, bigEndian);
            AnimSetList     = new GeoArray(reader, bigEndian);
            ParticleList    = new GeoArray(reader, bigEndian);
            SwooshList      = new GeoArray(reader, bigEndian);
            SpreadSheetList = new GeoArray(reader, bigEndian);
            FontList        = new GeoArray(reader, bigEndian);
            TextureList     = new GeoArray(reader, bigEndian);

            TextureUpdateList = new GeoRelArray(reader, bigEndian);
        }



        /* METHODS */

        /// <summary>
        /// Use the GeoHeader's marker and version values to test what platform this GeoFile was outputted for.
        /// </summary>
        /// <returns>The platform determined to be the one the GeoFile was outputted for, 
        /// null if it could not be determined or if the GeoFile did not have the necessary data.</returns>
        public GamePlatform? TestPlatform()
        {
            if (Marker == null || Versions == null) return null;

            /*
             * The marker at the start of the file will read "GEOM" if
             * the file is in big endian, and "MOEG" if little.
             * 
             * The first element in the 6 platform version variables will
             * be 1 if the platform is Xbox, and 0 if not.
             */

            //Check the marker
            string markerStr = new string(Marker);
            bool bigEndian;
            switch (markerStr)
            {
                case "GEOM":
                    bigEndian = true;
                    break;
                case "MOEG":
                    bigEndian = false;
                    break;
                default:
                    return null;
            }

            //Use the endianness together with the version variables to determine platform
            if (bigEndian && (Versions[0] == 0))
            {
                return GamePlatform.GameCube;
            } else if (!bigEndian && (Versions[0] == 0))
            {
                return GamePlatform.PlayStation2;
            } else if (!bigEndian && (Versions[0] == 1))
            {
                return GamePlatform.Xbox;
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// Convert the 4-byte unix timestamp in this GeoHeader to a <see cref="DateTime"/> object.
        /// </summary>
        /// <returns>The unix timestamp of this GeoHeader converted to a <see cref="DateTime"/> object.</returns>
        public DateTime? GetDateTimeStamp()
        {
            if (TimeStamp == 0) return null;

            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).AddSeconds(TimeStamp);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Basic info
            sb.AppendLine(string.Format("HashCode: 0x{0:X}", HashCode));
            sb.AppendLine("Version: " + Version);
            sb.AppendLine(string.Format("Flags: 0x{0:X}", Flags));
            sb.AppendLine("Timestamp: " + GetDateTimeStamp().ToString());
            sb.AppendLine("Filesize: " + FileSize + " bytes");
            sb.AppendLine("Base filesize: " + BaseFileSize + " bytes");
            sb.AppendLine("Debug section offset: " + DebugSectionOffset);
            sb.AppendLine("Debug section end offset: " + DebugSectionEndOffset);

            //Data arrays
            sb.AppendLine("Section list: " + SectionList.ToString());
            sb.AppendLine("Ref Pointer list: " + RefPointerList.ToString());
            sb.AppendLine("Entity list: " + EntityList.ToString());
            sb.AppendLine("Animation list: " + AnimList.ToString());
            sb.AppendLine("Animation skin list: " + AnimSkinList.ToString());
            sb.AppendLine("Script list: " + ScriptList.ToString());
            sb.AppendLine("Map list: " + MapList.ToString());
            sb.AppendLine("Animation mode list: " + AnimModeList.ToString());
            sb.AppendLine("Animation set list: " + AnimSetList.ToString());
            sb.AppendLine("Particle list: " + ParticleList.ToString());
            sb.AppendLine("Swoosh list: " + SwooshList.ToString());
            sb.AppendLine("Spreadsheet list: " + SpreadSheetList.ToString());
            sb.AppendLine("Font list: " + FontList.ToString());
            sb.AppendLine("Texture list: " + TextureList.ToString());

            sb.AppendLine("Texture update list: " + TextureUpdateList.ToString());

            return sb.ToString();
        }
    }
}
