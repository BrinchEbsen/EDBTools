using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EDBTools.Geo.SpreadSheet.Data.DataSheetTypeHandler;

namespace EDBTools.Geo.SpreadSheet.Data
{
    /// <summary>
    /// A cell in a <see cref="DataSheet"/>, which has a specific type that determines how its binary data should be interpreted.
    /// </summary>
    public class DataSheetCell
    {
        /// <summary>
        /// The data read directly from the binary. Will be as big as the amount of data the <see cref="Type"/> requires.
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Type that determines how the data in this cell should be interpreted.
        /// </summary>
        public SheetDataType Type { get; private set; }

        /// <summary>
        /// Read an amount of binary data from the current position of the <paramref name="reader"/>'s stream
        /// corresponding to the <see cref="SheetDataType"/>.
        /// </summary>
        /// <param name="type">Type of data this cell will contain.</param>
        public DataSheetCell(BinaryReader reader, bool bigEndian, SheetDataType type)
        {
            this.Type = type;

            ReadBytes(reader, bigEndian, DataSheetTypeHandler.DATATYPES[type].Size);
        }

        /// <summary>
        /// Read a sequence of bytes with a given length from the current position of the <paramref name="reader"/>'s stream.
        /// </summary>
        /// <param name="amt">Number of bytes to read.</param>
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

        /// <summary>
        /// Check if this cell contains a bitfield.
        /// </summary>
        /// <returns>True if this cell contains a bitfield.</returns>
        public bool IsBitField()
        {
            return
                (Type == SheetDataType.BITFIELD_U8) ||
                (Type == SheetDataType.BITFIELD_U16) ||
                (Type == SheetDataType.BITFIELD_U32);
        }

        /// <summary>
        /// Get an unsigned integer representation of the data in this cell.
        /// </summary>
        /// <returns>An unsigned integer representation of the data in this cell.</returns>
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

        /// <summary>
        /// Get a signed integer representation of the data in this cell.
        /// </summary>
        /// <returns>A signed integer representation of the data in this cell.</returns>
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

        /// <summary>
        /// Get a single-precision floating point representation of the data in this cell.
        /// </summary>
        /// <returns>A single-precision floating point representation of the data in this cell.</returns>
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

        /// <summary>
        /// Get a boolean representation of the data in this cell.
        /// </summary>
        /// <returns>A boolean representation of the data in this cell.</returns>
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

        /// <summary>
        /// Get the value of a bit in the data of this cell.
        /// </summary>
        /// <param name="index">Index of the bit.</param>
        /// <returns>Value of the bit at the given <paramref name="index"/>, 0 if the cell is not a bitfield.</returns>
        public byte GetValueBit(byte index)
        {
            if (!IsBitField()) { return 0; }

            uint val = GetValueUnsigned();

            return (byte)((val >> index) & 1);
        }

        /// <summary>
        /// Ensure the current stream is at a specific byte alignment. This will always move the stream FORWARD.
        /// </summary>
        /// <param name="stream">Stream to align.</param>
        /// <param name="alignment">Byte alignment.</param>
        private static void AlignReaderStream(Stream stream, int alignment)
        {
            long offs = stream.Position % alignment;

            if (offs > 0)
            {
                stream.Seek(alignment - offs, SeekOrigin.Current);
            }
        }

        /// <summary>
        /// Get a string representation of the data contained within this cell.
        /// </summary>
        /// <returns>A string representation of the data contained within this cell.</returns>
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
