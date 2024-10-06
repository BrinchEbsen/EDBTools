﻿using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoSectionHeader : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get { return 0x10; }
        }

        public uint StartOffset { get; private set; }
        public uint EndOffset { get; private set; }

        public GeoSectionHeader(BinaryReader reader, bool bigEndian) : base(reader)
        {
            HashCode = reader.ReadUInt32(bigEndian);
            StartOffset = reader.ReadUInt32(bigEndian);
            EndOffset = reader.ReadUInt32(bigEndian);
        }
    }
}
