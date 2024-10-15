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
            if (!SpreadSheetDataTypes.DATATYPES.ContainsKey(type))
            {
                throw new IOException("Unrecognized datatype for datasheet column: " + type);
            }

            this.Type = type;

            ReadBytes(reader, bigEndian, SpreadSheetDataTypes.DATATYPES[type].Size);
        }

        private void ReadBytes(BinaryReader reader, bool bigEndian, int amt)
        {
            if (amt > 1) { AlignReaderStream(reader.BaseStream, amt); }

            Data = new byte[amt];
            for (int i = 0; i < amt; i++)
            {
                Data[i] = reader.ReadByte();
            }

            if ((amt > 1) && bigEndian)
            {
                Data = (byte[])Data.Reverse();
            }
        }

        public T GetValue<T>(string type)
            where T : class
        {
            if (!SpreadSheetDataTypes.DATATYPES.ContainsKey(type))
            {
                throw new ArgumentException("Unrecognized argument for \"type\": " + type);
            }

            switch (type)
            {
                case "u8":
                    return Data[0] as T;
                case "u16":
                    return BitConverter.ToUInt16(Data, 0) as T;
                case "u32":
                    return BitConverter.ToUInt32(Data, 0) as T;
                case "s8":
                    return ((sbyte)Data[0]) as T;
                case "s16":
                    return BitConverter.ToInt16(Data, 0) as T;
                case "s32":
                    return BitConverter.ToInt32(Data, 0) as T;
                case "bool":
                    return (Data[0] != 0) as T;
                case "float":
                    return BitConverter.ToSingle(Data, 0) as T;
                case "hashcode":
                    return BitConverter.ToUInt32(Data, 0) as T;
                case "bitfield_u8":
                    return Data[0] as T;
                case "bitfield_u16":
                    return BitConverter.ToUInt16(Data, 0) as T;
                case "bitfield_u32":
                    return BitConverter.ToUInt32(Data, 0) as T;
                default: return null;
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
    }
}
