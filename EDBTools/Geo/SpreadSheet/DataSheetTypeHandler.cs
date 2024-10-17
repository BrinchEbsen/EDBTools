using System;
using System.Collections.Generic;

namespace EDBTools.Geo.SpreadSheet
{
    public struct SheetDataTypeInfo
    {
        public string Str;
        public int Size;
        public Type Type;
    }

    public static class DataSheetTypeHandler
    {
        public enum SheetDataType
        {
            None = 0,
            U8,
            U16,
            U32,
            S8,
            S16,
            S32,
            BOOL,
            FLOAT,
            HASHCODE,
            BITFIELD_U8,
            BITFIELD_U16,
            BITFIELD_U32
        }

        public static readonly Dictionary<SheetDataType, SheetDataTypeInfo> DATATYPES = new Dictionary<SheetDataType, SheetDataTypeInfo>
        {
            { SheetDataType.U8,           new SheetDataTypeInfo() { Str = "u8",           Size = 1, Type = typeof(byte)   } },
            { SheetDataType.U16,          new SheetDataTypeInfo() { Str = "u16",          Size = 2, Type = typeof(ushort) } },
            { SheetDataType.U32,          new SheetDataTypeInfo() { Str = "u32",          Size = 4, Type = typeof(uint)   } },
            { SheetDataType.S8,           new SheetDataTypeInfo() { Str = "s8",           Size = 1, Type = typeof(sbyte)  } },
            { SheetDataType.S16,          new SheetDataTypeInfo() { Str = "s16",          Size = 2, Type = typeof(short)  } },
            { SheetDataType.S32,          new SheetDataTypeInfo() { Str = "s32",          Size = 4, Type = typeof(int)    } },
            { SheetDataType.BOOL,         new SheetDataTypeInfo() { Str = "bool",         Size = 1, Type = typeof(bool)   } },
            { SheetDataType.FLOAT,        new SheetDataTypeInfo() { Str = "float",        Size = 4, Type = typeof(float)  } },
            { SheetDataType.HASHCODE,     new SheetDataTypeInfo() { Str = "hashcode",     Size = 4, Type = typeof(uint)   } },
            { SheetDataType.BITFIELD_U8,  new SheetDataTypeInfo() { Str = "bitfield_u8",  Size = 1, Type = typeof(byte)   } },
            { SheetDataType.BITFIELD_U16, new SheetDataTypeInfo() { Str = "bitfield_u16", Size = 2, Type = typeof(ushort) } },
            { SheetDataType.BITFIELD_U32, new SheetDataTypeInfo() { Str = "bitfield_u32", Size = 4, Type = typeof(uint)   } },
        };

        public static SheetDataType? GetDataType(string str)
        {
            foreach (var type in DATATYPES)
            {
                if (type.Value.Str == str)
                {
                    return type.Key;
                }
            }

            return null;
        }
    }
}
