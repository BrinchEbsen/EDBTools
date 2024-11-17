using EDBTools.Common;
using EDBTools.Geo.Headers;
using EDBTools.Geo.Map;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Font
{
    public struct GeoFontChar
    {
        public char Char;
        public uint Scaling;
        public override string ToString()
        {
            return "'" + Char.ToString() + "' Scale: " + Scaling.ToString("X");
        }
    }

    public class GeoFont
    {
        /// <summary>
        /// Base address of this font's binary data in the file.
        /// </summary>
        public long Address { get; private set; }

        /// <summary>
        /// Hashcode for this font. Section HT_Font (0x07XXXXXX).
        /// </summary>
        public uint HashCode { get; private set; }

        /// <summary>
        /// Index of the font texture used by this font.
        /// </summary>
        public uint TextureIndex { get; private set; }

        /// <summary>
        /// Table of pointers indexed by the upper byte of the unicode character, which point to other tables,
        /// which contain data for the character indexed by the lower byte of the unicode character.
        /// </summary>
        public RelPtr[] UniCodeTable { get; private set; }

        /// <summary>
        /// Unicode characters supported by this font.
        /// </summary>
        public List<GeoFontChar> SupportedChars { get; private set; }

        public GeoFont() { }

        public GeoFont ReadFromStream(BinaryReader reader, bool bigEndian, GeoFontHeader header)
        {
            Address = reader.BaseStream.Position;
            HashCode = header.HashCode;

            reader.BaseStream.Seek(header.Address, SeekOrigin.Begin);

            TextureIndex = reader.ReadUInt32(bigEndian);

            //Populate pointer list
            UniCodeTable = new RelPtr[256];
            for (int i = 0; i < 256; i++)
            {
                UniCodeTable[i] = new RelPtr(reader, bigEndian);
            }

            //Populate character list
            SupportedChars = new List<GeoFontChar>();
            for (int i = 0; i < 256; i++)
            {
                /*
                 * A unicode character exists for this font if the pointer isn't null
                 * and the data in the appointed table isn't null
                 */
                
                var ptr = UniCodeTable[i];

                if (!ptr.IsNull)
                {
                    reader.BaseStream.Seek(ptr.AbsoluteAddress, SeekOrigin.Begin);

                    for (int j = 0; j < 256; j++)
                    {
                        uint scl = reader.ReadUInt32(bigEndian);

                        if (scl != 0)
                        {
                            GeoFontChar c = new GeoFontChar
                            {
                                Char = (char)((i << 8) | j),
                                Scaling = scl
                            };

                            SupportedChars.Add(c);
                        }
                    }
                }
            }

            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hashcode: " + HashCode.ToString("X"));
            sb.AppendLine("Texture Index: " + TextureIndex);

            sb.AppendLine("Supported Characters:");
            int wrapCounter = 0;
            foreach(var c in SupportedChars)
            {
                sb.Append(c.Char);
                wrapCounter++;
                if(wrapCounter > 30)
                {
                    wrapCounter = 0;
                    sb.AppendLine();
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
