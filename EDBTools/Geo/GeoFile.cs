using Common;
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
        public GeoHeader GeoHeader { get; set; }



        /* CONSTRUCTORS */

        public GeoFile(BinaryReader reader)
        {
            bool? endian = TestEndianness(reader);

            if (!endian.HasValue)
                throw new IOException("Indeterminate endianness for the GeoFile supplied to the reader" +
                                      " - Marker value read neither \"GEOM\" or \"MOEG\".");
            
            BigEndian = endian.Value;

            GeoHeader = new GeoHeader(reader, endian.Value);

            GamePlatform? platform = GeoHeader.TestPlatform();

            if (!platform.HasValue)
                throw new IOException("Indeterminate game platform for the GeoFile supplied to the reader.");

            Platform = platform.Value;
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
