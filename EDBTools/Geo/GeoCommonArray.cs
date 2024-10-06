﻿using EDBTools.Common;
using EDBTools.Geo.Headers;
using EDBTools.HashCodes;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo
{
    /// <summary>
    /// Descriptor for an array of GeoFile data headers.
    /// </summary>
    public class GeoCommonArray
    {
        /// <summary>
        /// Size of the data array.
        /// </summary>
        public short ArraySize { get; private set; }
        /// <summary>
        /// Size of the hashcode array.
        /// </summary>
        public short HashSize { get; private set; }
        /// <summary>
        /// Relative pointer to the start of the array.
        /// </summary>
        public RelPtr Offset { get; private set; }

        public GeoCommonArray(BinaryReader reader, bool bigEndian)
        {
            ArraySize = reader.ReadInt16(bigEndian);
            HashSize = reader.ReadInt16(bigEndian);
            Offset = new RelPtr(reader, bigEndian);
        }

        public override string ToString()
        {
            return string.Format("ArraySize: {0} | HashSize: {1} | Relative Offset: {2}",
                ArraySize, HashSize, Offset.Offset);
        }
    }
}
