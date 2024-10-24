using EDBTools.Common;
using Extensions;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EDBTools.Geo.SpreadSheet.Text
{
    /// <summary>
    /// Describes the position of a text string and its related data.
    /// </summary>
    public struct TextItemDescriptor
    {
        public uint HashCode;
        public RelPtr String;
        public RelPtr UserData;
        public uint SoundHashCode;
    }

    /// <summary>
    /// Contains a string of UTF-16 encoded text along with two slots of optional user data.
    /// </summary>
    public struct TextItem
    {
        public string String;
        public uint[] UserData;
    }

    /// <summary>
    /// Contains all the text-related data found within a section of a GeoFile.
    /// </summary>
    public class TextSheetData
    {
        /// <summary>
        /// Number of text items found within this section.
        /// </summary>
        public int NumTextItems { get; private set; }

        /// <summary>
        /// Number of descriptors.
        /// </summary>
        public List<TextItemDescriptor> Descriptors { get; private set; }

        /// <summary>
        /// Number of text items.
        /// </summary>
        public List<TextItem> Items { get; private set; }

        public TextSheetData(BinaryReader reader, bool bigEndian)
        {
            //Seek to relevant data
            reader.BaseStream.Seek(0x14, SeekOrigin.Current);

            NumTextItems = reader.ReadInt32(bigEndian);

            Descriptors = new List<TextItemDescriptor>();

            for(int i = 0; i < NumTextItems; i++)
            {
                Descriptors.Add(new TextItemDescriptor()
                {
                    HashCode = reader.ReadUInt32(bigEndian),
                    String = new RelPtr(reader, bigEndian),
                    UserData = new RelPtr(reader, bigEndian),
                    SoundHashCode = reader.ReadUInt32(bigEndian),
                });
            }

            Items = new List<TextItem>();
            foreach (var desc in Descriptors)
            {
                //Read UTF-16 string
                reader.BaseStream.Seek(desc.String.AbsoluteAddress, SeekOrigin.Begin);

                StringBuilder sb = new StringBuilder();

                char c = (char)reader.ReadUInt16(bigEndian);
                while (c != 0)
                {
                    sb.Append(c);
                    c = (char)reader.ReadUInt16(bigEndian);
                }

                //Read userdata
                reader.BaseStream.Seek(desc.UserData.AbsoluteAddress, SeekOrigin.Begin);

                uint[] userData = new uint[]
                {
                    reader.ReadUInt32(bigEndian),
                    reader.ReadUInt32(bigEndian)
                };

                //Insert text item
                Items.Add(new TextItem()
                {
                    String = sb.ToString(),
                    UserData = userData
                });
            }
        }

        public override string ToString()
        {
            if ((NumTextItems != Descriptors?.Count) || (NumTextItems != Items?.Count))
            {
                return "Text data invalid: Text item count differs from amount specified in section.";
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Hashcode | User Data            | Text");
            sb.AppendLine("--------------------------------------");

            for (int i = 0; i < NumTextItems; i++)
            {
                sb.Append(Descriptors[i].HashCode.ToString("X").PadLeft(8));
                sb.Append(" | [" + Items[i].UserData[0].ToString("X").PadLeft(8));
                sb.Append(", " + Items[i].UserData[1].ToString("X").PadLeft(8));
                sb.AppendLine("] | " + Items[i].String);
            }

            return sb.ToString();
        }
    }
}
