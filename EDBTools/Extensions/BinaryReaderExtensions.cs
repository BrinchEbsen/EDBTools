using System;
using System.IO;
using System.Text;

namespace Extensions
{
    public static class BinaryReaderExtensions
    {
        //Big-endian compatible methods

        public static ushort ReadUInt16(this BinaryReader reader, bool BigEndian)
        {
            if (!BigEndian)
            {
                return reader.ReadUInt16();
            }
            else
            {
                ushort x = reader.ReadUInt16();

                return (ushort)((ushort)((x & 0xff) << 8) | ((x >> 8) & 0xff));
            }
        }

        public static short ReadInt16(this BinaryReader reader, bool BigEndian)
        {
            if (!BigEndian)
            {
                return reader.ReadInt16();
            }
            else
            {
                return (short)ReadUInt16(reader, BigEndian);
            }
        }

        public static uint ReadUInt32(this BinaryReader reader, bool BigEndian)
        {
            if (!BigEndian)
            {
                return reader.ReadUInt32();
            }
            else
            {
                uint x = reader.ReadUInt32();

                return ((x & 0x000000ff) << 24) +
                       ((x & 0x0000ff00) << 8) +
                       ((x & 0x00ff0000) >> 8) +
                       ((x & 0xff000000) >> 24);
            }
        }

        public static int ReadInt32(this BinaryReader reader, bool BigEndian)
        {
            if (!BigEndian)
            {
                return reader.ReadInt32();
            }
            else
            {
                return (int)ReadUInt32(reader, BigEndian);
            }
        }

        public static float ReadSingle(this BinaryReader reader, bool BigEndian)
        {
            if (!BigEndian)
            {
                return reader.ReadSingle();
            }
            else
            {
                byte[] bytes = BitConverter.GetBytes(reader.ReadUInt32(true));
                return BitConverter.ToSingle(bytes, 0);
            }
        }

        //Reading special data types

        public static string ReadASCIIString(this BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();

            for (char c = (char)reader.ReadByte(); c != 0; c = (char)reader.ReadByte())
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}