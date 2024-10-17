using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDBTools.Geo.SpreadSheet.DataSheetTypeHandler;

namespace EDBTools.Geo.SpreadSheet
{
    public class DataSheetCell
    {
        public byte[] Data { get; private set; }
        public SheetDataType Type { get; private set; }

        public DataSheetCell(BinaryReader reader, bool bigEndian, SheetDataType type)
        {
            this.Type = type;

            ReadBytes(reader, bigEndian, DataSheetTypeHandler.DATATYPES[type].Size);
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

        public bool IsBitField()
        {
            return
                (Type == SheetDataType.BITFIELD_U8) ||
                (Type == SheetDataType.BITFIELD_U16) ||
                (Type == SheetDataType.BITFIELD_U32);
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
                case 1: return (sbyte)Data[0];
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

        public byte GetValueBit(byte index)
        {
            if (!IsBitField()) { return 0; }

            uint val = GetValueUnsigned();

            return (byte)((val >> index) & 1);
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
                case SheetDataType.U8:
                case SheetDataType.U16:
                case SheetDataType.U32:
                    return GetValueUnsigned().ToString();
                case SheetDataType.S8:
                case SheetDataType.S16:
                case SheetDataType.S32:
                    return GetValueSigned().ToString();
                case SheetDataType.HASHCODE:
                case SheetDataType.BITFIELD_U8:
                case SheetDataType.BITFIELD_U16:
                case SheetDataType.BITFIELD_U32:
                    return GetValueUnsigned().ToString("X");
                case SheetDataType.BOOL:
                    return GetValueBool().ToString();
                case SheetDataType.FLOAT:
                    return GetValueFloat().ToString("0.000");
                default:
                    return "INVALID_TYPE";
            }
        }
    }
}
