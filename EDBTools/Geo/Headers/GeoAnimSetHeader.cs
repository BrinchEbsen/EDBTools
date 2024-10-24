﻿using Extensions;
using System.IO;

namespace EDBTools.Geo.Headers
{
    public class GeoAnimSetHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x4; }
        }

        public uint NumAnimSets { get; private set; }

        public GeoAnimSetHeader()
        {
        }

        public GeoAnimSetHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);
            NumAnimSets = reader.ReadUInt32(bigEndian);

            return this;
        }
    }
}
