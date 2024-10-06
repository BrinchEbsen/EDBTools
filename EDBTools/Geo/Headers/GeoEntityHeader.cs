﻿using EDBTools.Common;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoEntityHeader : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get { return 0x14; }
        }

        public short Section { get; private set; }
        public short Debug1 { get; private set; }
        public uint Debug2 { get; private set; }
        public long Address { get; private set; }
        public RelPtr LodTable { get; private set; }

        public GeoEntityHeader(BinaryReader reader, bool bigEndian) : base(reader)
        {
            HashCode = reader.ReadUInt32(bigEndian);
            Section = reader.ReadInt16(bigEndian);
            Debug1 = reader.ReadInt16(bigEndian);
            Debug2 = reader.ReadUInt32(bigEndian);
            Address = reader.ReadUInt32(bigEndian);
            LodTable = new RelPtr(reader, bigEndian);
        }
    }
}
