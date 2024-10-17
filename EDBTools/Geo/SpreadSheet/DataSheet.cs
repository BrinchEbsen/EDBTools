using EDBTools.Geo.SpreadSheet.Serialization;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDBTools.Geo.SpreadSheet.DataSheetTypeHandler;

namespace EDBTools.Geo.SpreadSheet
{
    /// <summary>
    /// Holds various data in cells divided into columns and rows.
    /// Each column is given a datatype.
    /// </summary>
    public class DataSheet
    {
        /// <summary>
        /// Number of rows in this sheet.
        /// </summary>
        public int RowCount { get; private set; }

        /// <summary>
        /// Formatting needed to read the data.
        /// Without it, the length, types and divisions of cells in the sheet cannot be determined from the binary.
        /// </summary>
        public DataSheetFormat Format { get; private set; }

        /// <summary>
        /// 2D array of cells in the datasheet. Outer array is rows, inner array is columns.
        /// Thus, to index a cell at row x and column y, you'd write Cells[x][y].
        /// </summary>
        public DataSheetCell[][] Cells { get; private set; }

        /// <summary>
        /// Read the binary data of the datasheet starting from the current position of the <see cref="BinaryReader"/>'s stream,
        /// and construct a new instance of this class using it.
        /// </summary>
        /// <exception cref="IOException"></exception>
        public DataSheet(BinaryReader reader, bool bigEndian, DataSheetFormat format)
        {
            //First element is the number of rows
            RowCount = reader.ReadInt32(bigEndian);
            Format = format;

            //initialize cells with the columns and rows
            Cells = new DataSheetCell[RowCount][];
            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new DataSheetCell[Format.Columns.Count];
            }

            //Keep track of the base address of the current row
            long rowAddr = reader.BaseStream.Position;

            //Read all cells
            for (int i = 0; i < RowCount; i++)
            {
                int j = 0; //Column index
                foreach (DataSheetColumnFormat column in Format.Columns)
                {
                    SheetDataType? type = DataSheetTypeHandler.GetDataType(column.Type);
                    if (type == null)
                    {
                        throw new IOException("Unrecognized datatype for datasheet column: " + column.Type);
                    }

                    Cells[i][j] = new DataSheetCell(reader, bigEndian, type.Value);
                    j++;
                }

                //Check for overflow, in case the format is bogus
                if (reader.BaseStream.Position > (rowAddr + Format.RowSize))
                {
                    long overflow = reader.BaseStream.Position - (rowAddr + Format.RowSize);
                    throw new IOException("Exceeded rowsize by " + overflow + " bytes at row: " + i + ".");
                }

                //Seek to next row
                rowAddr += format.RowSize;
                reader.BaseStream.Seek(rowAddr, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Get a string representation of the sheet formatted to look
        /// like a grid of cells when viewed in a monospace font.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            //Put all string representations of the cells into a 2D array of strings
            string[][] cellStrings = new string[Cells.Length][];
            for (int i = 0; i < cellStrings.Length; i++)
            {
                cellStrings[i] = new string[Cells[i].Length];
            }

            for (int i = 0; i < Cells.Length; i++)
            {
                for (int j = 0; j < Cells[i].Length; j++)
                {
                    cellStrings[i][j] = Cells[i][j].ToString();
                }
            }

            int numCols = Format.Columns.Count;

            //Widths of each column in characters
            int[] colWidths = new int[numCols];

            //Total character width of the table
            //Add space for the lines dividing the columns, as well as a space on either side of a column
            int totalWidth = (numCols + 1) + (numCols * 2);

            //Adjust column widths based on the column names
            for (int i = 0; i < numCols; i++)
            {
                //Name or type, whichever is wider
                colWidths[i] = Math.Max(
                    Format.Columns[i].Name.Length,
                    Format.Columns[i].Type.Length);
            }

            //Adjust column widths based on the cell strings
            for (int i = 0; i < cellStrings.Length; i++)
            {
                for (int j = 0; j < cellStrings[i].Length; j++)
                {
                    if (colWidths[j] < cellStrings[i][j].Length)
                    {
                        colWidths[j] = cellStrings[i][j].Length;
                    }
                }
            }

            //Add widths to the total width
            foreach (int w in colWidths)
                totalWidth += w;

            //Start actually building the string
            StringBuilder str = new StringBuilder();

            for (int i = 0; i < numCols; i++)
            {
                str.Append("| ");
                str.Append(Format.Columns[i].Name.PadRight(colWidths[i] + 1));
            }
            str.AppendLine("|");

            for (int i = 0; i < numCols; i++)
            {
                str.Append("| ");
                str.Append(Format.Columns[i].Type.PadRight(colWidths[i] + 1));
            }
            str.AppendLine("|");

            str.Append('-', totalWidth);
            str.AppendLine();

            for (int i = 0; i < cellStrings.Length; i++)
            {
                for (int j = 0; j < cellStrings[i].Length; j++)
                {
                    str.Append("| ");
                    str.Append(cellStrings[i][j].PadRight(colWidths[j] + 1));
                }
                str.AppendLine("|");
            }

            return str.ToString();
        }
    }
}
