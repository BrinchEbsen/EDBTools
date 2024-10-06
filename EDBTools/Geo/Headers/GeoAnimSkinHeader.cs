using EDBTools.Common;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoAnimSkinHeader : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get { return 0x1C; }
        }

        public short Section { get; private set; }
        public short Debug { get; private set; }
        public long Address { get; private set; }
        public uint BaseSkinNum { get; private set; }
        public uint MipRef { get; private set; }
        public float MipDistance { get; private set; }

        public GeoAnimSkinHeader(BinaryReader reader, bool bigEndian) : base(reader)
        {
            HashCode = reader.ReadUInt32(bigEndian);
            Section = reader.ReadInt16(bigEndian);
            Debug = reader.ReadInt16(bigEndian);
            Address = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            BaseSkinNum = reader.ReadUInt32(bigEndian);
            MipRef = reader.ReadUInt32(bigEndian);
            MipDistance = reader.ReadSingle(bigEndian);
        }
    }
}
