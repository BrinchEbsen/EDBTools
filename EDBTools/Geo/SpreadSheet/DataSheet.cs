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

        //[row][column]
        public DataSheetCell[][] Cells { get; private set; }

        public DataSheet(BinaryReader reader, bool bigEndian, DataSheetFormat format)
        {
            //First element is the number of rows
            RowCount = reader.ReadInt32(bigEndian);

            //initialize cells with the columns and rows
            Cells = new DataSheetCell[RowCount][];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new DataSheetCell[format.Columns.Count];
            }

            //Keep track of the base address of the current row
            long rowAddr = reader.BaseStream.Position;

            //Read all cells
            for (int i = 0; i < RowCount; i++)
            {
                int j = 0; //Column index
                foreach (DataSheetColumnFormat column in format.Columns)
                {
                    Cells[i][j] = new DataSheetCell(reader, bigEndian, column.Type);
                    j++;
                }

                //Check for overflow, in case the format is bogus
                if (reader.BaseStream.Position > (rowAddr + format.RowSize))
                {
                    long overflow = reader.BaseStream.Position - (rowAddr + format.RowSize);
                    throw new IOException("Exceeded rowsize by " + overflow + " bytes at row: " + i + ".");
                }

                //Seek to next row
                rowAddr += format.RowSize;
                reader.BaseStream.Seek(rowAddr, SeekOrigin.Begin);
            }
        }
    }
}
