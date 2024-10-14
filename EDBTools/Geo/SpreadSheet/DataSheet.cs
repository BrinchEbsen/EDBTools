using EDBTools.Geo.SpreadSheet.Serialization;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet
{
    public class DataSheet
    {
        public int RowCount { get; private set; }

        //[column][row]
        public object[][] Cells { get; private set; }

        public DataSheet(BinaryReader reader, bool bigEndian, DataSheetFormat format)
        {
            RowCount = reader.ReadInt32(bigEndian);

            //initialize cells
            Cells = new object[format.Columns.Count][];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new object[RowCount];
            }

            long addr = reader.BaseStream.Position;
            for (int i = 0; i < RowCount; i++)
            {
                int j = 0;
                foreach (KeyValuePair<string, string> pair in format.Columns)
                {
                    ReadCell(reader, bigEndian, ref Cells[j][i], pair.Value);
                    j++;
                }

                addr += format.RowSize;
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            }
        }

        private void ReadCell(BinaryReader reader, bool bigEndian, ref object cellObj, string Type)
        {
            if (!SpreadSheetDataTypes.DATATYPES.Contains(Type))
            {
                throw new IOException("Unrecognized datatype for datasheet column: " + Type);
            }

            switch (Type)
            {
                case "u8":
                    cellObj = new DataSheetCell<byte>(reader.ReadByte());
                    break;
                case "u16":
                    AlignReaderStream(reader.BaseStream, 2);
                    cellObj = new DataSheetCell<ushort>(reader.ReadUInt16(bigEndian));
                    break;
                case "u32":
                    AlignReaderStream(reader.BaseStream, 4);
                    cellObj = new DataSheetCell<uint>(reader.ReadUInt32(bigEndian));
                    break;
                case "s8":
                    cellObj = new DataSheetCell<sbyte>(reader.ReadSByte());
                    break;
                case "s16":
                    AlignReaderStream(reader.BaseStream, 2);
                    cellObj = new DataSheetCell<short>(reader.ReadInt16(bigEndian));
                    break;
                case "s32":
                    AlignReaderStream(reader.BaseStream, 4);
                    cellObj = new DataSheetCell<int>(reader.ReadInt32(bigEndian));
                    break;
                case "bool":
                    cellObj = new DataSheetCell<bool>(reader.ReadByte() != 0);
                    break;
                case "float":
                    AlignReaderStream(reader.BaseStream, 4);
                    cellObj = new DataSheetCell<float>(reader.ReadSingle(bigEndian));
                    break;
                case "hashcode":
                    AlignReaderStream(reader.BaseStream, 4);
                    cellObj = new DataSheetCell<uint>(reader.ReadUInt32(bigEndian));
                    break;
            }

            return;
        }

        private void AlignReaderStream(Stream stream, int alignment)
        {
            long offs = stream.Position % alignment;

            if (offs > 0)
            {
                stream.Seek(alignment - offs, SeekOrigin.Current);
            }
        }
    }
}
