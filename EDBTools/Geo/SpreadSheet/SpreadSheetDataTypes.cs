using System;
using System.Collections.Generic;

namespace EDBTools.Geo.SpreadSheet
{
    public struct SheetDataType
    {
        public int Size;
        public Type Type;
    }

    public static class SpreadSheetDataTypes
    {
        public static readonly Dictionary<string, SheetDataType> DATATYPES = new Dictionary<string, SheetDataType>
        {
            { "u8",           new SheetDataType() { Size = 1, Type = typeof(byte)   } },
            { "u16",          new SheetDataType() { Size = 2, Type = typeof(ushort) } },
            { "u32",          new SheetDataType() { Size = 4, Type = typeof(uint)   } },
            { "s8",           new SheetDataType() { Size = 1, Type = typeof(sbyte)  } },
            { "s16",          new SheetDataType() { Size = 2, Type = typeof(short)  } },
            { "s32",          new SheetDataType() { Size = 4, Type = typeof(int)    } },
            { "bool",         new SheetDataType() { Size = 1, Type = typeof(bool)   } },
            { "float",        new SheetDataType() { Size = 4, Type = typeof(float)  } },
            { "hashcode",     new SheetDataType() { Size = 4, Type = typeof(uint)   } },
            { "bitfield_u8",  new SheetDataType() { Size = 1, Type = typeof(byte)   } },
            { "bitfield_u16", new SheetDataType() { Size = 2, Type = typeof(ushort) } },
            { "bitfield_u32", new SheetDataType() { Size = 4, Type = typeof(uint)   } },
        };
    }
}
