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
        /// Number of columns the source spreadsheet would have.
        /// Counts all defined bits of a bitfield as a column, as well as its markers.
        /// </summary>
        public int FullColumnCount {
            get
            {
                int num = 0;

                for (int i = 0; i < Cells[0].Length; i++)
                {
                    var Cell = Cells[0][i];

                    if (Cell.IsBitField())
                    {
                        var fmt = Format.Columns[i];
                        var bitfield = Format.BitFields.Find((x) => x.FieldName == fmt.Name);
                        if (bitfield == null)
                        {
                            num++; continue;
                        }

                        foreach(var bit in bitfield.Bits)
                        {
                            num++;
                        }

                        //add 2 for the bitfield marker on either side
                        num += 2;
                    } else
                    {
                        num++;
                    }
                }

                return num;
            }
        }

        public SheetBitFieldFormat FindBitsFormatForField(string fieldName)
        {
            foreach (var field in Format.BitFields)
            {
                if (field.FieldName == fieldName) { return field; }
            }

            return null;
        }

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
                    SheetDataType? type = DataSheetTypeHandler.GetDataType(column.Type)
                        ?? throw new IOException("Unrecognized datatype for datasheet column: " + column.Type);
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
            /*
             * I would put comments on this thing but it sucks and i hate it
             */

            int numCols = FullColumnCount;
            int numRows = RowCount;

            int[] colWidths = new int[numCols];
            int totalWidth = (numCols + 1) + (numCols * 2);

            string[] headers = new string[numCols];
            string[] types = new string[numCols];
            string[][] cellStrings = new string[numRows][];
            for(int row = 0; row < numRows; row++)
            {
                cellStrings[row] = new string[numCols];
            }

            bool doneHeaders = false;

            int cellIndex;
            for (int row = 0; row < numRows; row++)
            {
                cellIndex = 0;

                for (int col = 0; col < numCols; col++)
                {
                    var cell = Cells[row][cellIndex];

                    if (cell.IsBitField())
                    {
                        var bitfield = FindBitsFormatForField(Format.Columns[cellIndex].Name);
                        if (bitfield == null)
                        {
                            if (!doneHeaders)
                            {
                                headers[col] = Format.Columns[cellIndex].Name;
                                types[col] = Format.Columns[cellIndex].Type;
                            }
                            cellStrings[row][col] = cell.ToString();
                        } else
                        {
                            headers[col] = "";
                            types[col] = Format.Columns[cellIndex].Type;
                            cellStrings[row][col] = "";

                            col++;

                            foreach (var bit in bitfield.Bits)
                            {
                                if (!doneHeaders)
                                {
                                    headers[col] = bit.Name;
                                    types[col] = "bit_" + bit.Num;
                                }
                                cellStrings[row][col] = cell.GetValueBit((byte)bit.Num).ToString();

                                col++;
                            }

                            headers[col] = "";
                            types[col] = Format.Columns[cellIndex].Type;
                            cellStrings[row][col] = "";

                            col++;

                            //Revert one to compensate for the ++ in the for loop
                            col--;
                        }
                    } else
                    {
                        if (!doneHeaders)
                        {
                            headers[col] = Format.Columns[cellIndex].Name;
                            types[col] = Format.Columns[cellIndex].Type;
                        }
                        cellStrings[row][col] = cell.ToString();
                    }

                    cellIndex++;
                }

                doneHeaders = true;
            }

            int[] colWidth_values = new int[numCols];
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    colWidth_values[col] = Math.Max(colWidth_values[col], cellStrings[row][col].Length);
                    colWidths[col] = Math.Max(Math.Max(headers[col].Length, types[col].Length), colWidth_values[col]);
                }
            }

            for (int col = 0; col < numCols; col++)
            {
                totalWidth += colWidths[col];
            }

            StringBuilder headersStr = new StringBuilder();
            StringBuilder typesStr = new StringBuilder();
            StringBuilder valuesStr = new StringBuilder();

            doneHeaders = false;

            //build headers and types
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    if (!doneHeaders)
                    {
                        headersStr.Append("| ");
                        headersStr.Append(headers[col].PadRight(colWidths[col] + 1));
                        typesStr.Append("| ");
                        typesStr.Append(types[col].PadRight(colWidths[col] + 1));
                    }

                    valuesStr.Append("| ");
                    valuesStr.Append(cellStrings[row][col].PadRight(colWidths[col] + 1));
                }

                if (!doneHeaders)
                {
                    headersStr.Append("|");
                    typesStr.Append("|");
                }
                doneHeaders = true;

                valuesStr.AppendLine("|");
            }

            StringBuilder finalStr = new StringBuilder();

            finalStr.AppendLine(headersStr.ToString());
            finalStr.AppendLine(typesStr.ToString());
            finalStr.AppendLine(new string('-', totalWidth));
            finalStr.Append(valuesStr.ToString());

            return finalStr.ToString();
        }
    }
}
