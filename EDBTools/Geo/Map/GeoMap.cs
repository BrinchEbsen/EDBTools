using EDBTools.Geo.Headers;
using EDBTools.Geo.Map.Triggers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Map
{
    public class GeoMap
    {
        /// <summary>
        /// Base address of this map's binary data in the file.
        /// </summary>
        public long Address { get; private set; }

        /// <summary>
        /// Hashcode for this map. Section HT_Map (0x05XXXXXX, local: 0x85XXXXXX).
        /// </summary>
        public uint HashCode { get; private set; }

        public MapHeader Header { get; private set; }

        public GeoMapTriggers MapTriggers { get; private set; }

        public GeoMap() { }

        public GeoMap ReadFromStream(BinaryReader reader, bool bigEndian, GeoMapHeader header)
        {
            reader.BaseStream.Seek(header.Address, SeekOrigin.Begin);
            Address = header.Address;
            HashCode = header.HashCode;

            Header = new MapHeader(reader, bigEndian);

            reader.BaseStream.Seek(Header.RP_TriggerHeader.AbsoluteAddress, SeekOrigin.Begin);
            MapTriggers = new GeoMapTriggers(reader, bigEndian);

            return this;
        }
    }
}
