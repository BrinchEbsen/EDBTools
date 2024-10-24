using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Map.Triggers
{
    public class GeoTriggerType
    {
        public uint Type { get; private set; }
        public uint SubType { get; private set; }

        public GeoTriggerType(BinaryReader reader, bool bigEndian)
        {
            Type = reader.ReadUInt32(bigEndian);
            SubType = reader.ReadUInt32(bigEndian);
            reader.BaseStream.Seek(8, SeekOrigin.Current);
        }

        public override string ToString()
        {
            return string.Format("Type: {0:X} | SubType: {1:X}", Type, SubType);
        }
    }
}
