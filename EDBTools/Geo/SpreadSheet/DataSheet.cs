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
        public DataSheetCell[][] Cells { get; private set; }

        public DataSheet(BinaryReader reader, bool bigEndian, DataSheetFormat format)
        {
            RowCount = reader.ReadInt32(bigEndian);

            //initialize cells
            Cells = new DataSheetCell[format.Columns.Count][];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new DataSheetCell[RowCount];
            }

            long addr = reader.BaseStream.Position;
            for (int i = 0; i < RowCount; i++)
            {
                int j = 0;
                foreach (KeyValuePair<string, string> pair in format.Columns)
                {
                    Cells[j][i] = new DataSheetCell(reader, bigEndian, pair.Value);
                    j++;
                }

                if (reader.BaseStream.Position > (addr + format.RowSize))
                {
                    long overflow = reader.BaseStream.Position - (addr + format.RowSize);
                    throw new IOException("Exceeded rowsize by " + overflow + " bytes at row: " + i + ".");
                }

                addr += format.RowSize;
                reader.BaseStream.Seek(addr, SeekOrigin.Begin);
            }
        }
    }
}
