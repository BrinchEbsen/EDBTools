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
    public class GeoAnimHeader : GeoCommonHeader
    {
        public override long HEADER_SIZE
        {
            get { return 0x20; }
        }

        public short Section { get; private set; }
        public short Debug { get; private set; }
        public long Address { get; private set; }
        public long MotionDataInfo { get; private set; }
        public uint DataSize { get; private set; }
        public uint SkinNum { get; private set; }

        public GeoAnimHeader(BinaryReader reader, bool bigEndian) : base(reader)
        {
            HashCode = reader.ReadUInt32(bigEndian);
            Section = reader.ReadInt16(bigEndian);
            Debug = reader.ReadInt16(bigEndian);
            Address = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            MotionDataInfo = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(4, SeekOrigin.Current);
            DataSize = reader.ReadUInt32(bigEndian);
            SkinNum = reader.ReadUInt32(bigEndian);
        }
    }
}
