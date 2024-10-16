using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet
{
    public class DataSheetCell
    {
        public byte[] Data { get; private set; }
        public string Type { get; private set; }

        public DataSheetCell(BinaryReader reader, bool bigEndian, string type)
        {
            //Check for invalid type
            if (!SpreadSheetDataTypes.DATATYPES.ContainsKey(type))
            {
                throw new IOException("Unrecognized datatype for datasheet column: " + type);
            }

            this.Type = type;

            ReadBytes(reader, bigEndian, SpreadSheetDataTypes.DATATYPES[type].Size);
        }

        private void ReadBytes(BinaryReader reader, bool bigEndian, int amt)
        {
            //Align if the data type needs it
            if (amt > 1) { AlignReaderStream(reader.BaseStream, amt); }

            //Read the requested amount of bytes
            Data = new byte[amt];
            for (int i = 0; i < amt; i++)
            {
                Data[i] = reader.ReadByte();
            }

            //Reverse if big endian
            if ((amt > 1) && bigEndian)
            {
                Data = (byte[])Data.Reverse();
            }
        }

        public uint GetValueUnsigned()
        {
            switch (Data.Length)
            {
                case 1: return Data[0];
                case 2: return BitConverter.ToUInt16(Data, 0);
                case 4: return BitConverter.ToUInt32(Data, 0);
                default: return 0;
            }
        }

        public int GetValueSigned()
        {
            switch (Data.Length)
            {
                case 1: return (int)((sbyte)Data[0]);
                case 2: return BitConverter.ToInt16(Data, 0);
                case 4: return BitConverter.ToInt32(Data, 0);
                default: return 0;
            }
        }

        public float GetValueFloat()
        {
            if (Data.Length == 4)
            {
                return BitConverter.ToSingle(Data, 0);
            } else
            {
                return 0;
            }
        }

        public bool GetValueBool()
        {
            if (Data.Length == 1)
            {
                return Data[0] != 0;
            } else
            {
                return false;
            }
        }

        //TODO: might be better served from an external class/extension method
        private static void AlignReaderStream(Stream stream, int alignment)
        {
            long offs = stream.Position % alignment;

            if (offs > 0)
            {
                stream.Seek(alignment - offs, SeekOrigin.Current);
            }
        }

        public override string ToString()
        {
            switch (Type)
            {
                case "u8":
                case "u16":
                case "u32":
                    return GetValueUnsigned().ToString();
                case "s8":
                case "s16":
                case "s32":
                    return GetValueSigned().ToString();
                case "hashcode":
                case "bitfield_u8":
                case "bitfield_u16":
                case "bitfield_u32":
                    return GetValueUnsigned().ToString("X");
                case "bool":
                    return GetValueBool().ToString();
                case "float":
                    return GetValueFloat().ToString("0.000");
                default:
                    return "INVALID_TYPE";
            }
        }
    }
}
