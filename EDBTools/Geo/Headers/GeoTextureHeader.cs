﻿using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoTextureHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0xC; }
        }

        public ushort Width { get; private set; }
        public ushort Height { get; private set; }
        public uint GameFlags { get; private set; }
        public uint Flags { get; private set; }

        public GeoTextureHeader()
        {
        }

        public GeoTextureHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            Width = reader.ReadUInt16(bigEndian);
            Height = reader.ReadUInt16(bigEndian);
            GameFlags = reader.ReadUInt32(bigEndian);
            Flags = reader.ReadUInt32(bigEndian);

            return this;
        }
    }
}
