﻿using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoSwooshHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE; }
        }

        public GeoSwooshHeader()
        {
        }

        public GeoSwooshHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
        }

        public override GeoCommonHeader ReadFromFile(BinaryReader reader, bool bigEndian)
        {
            base.ReadFromFile(reader, bigEndian);

            return this;
        }
    }
}
