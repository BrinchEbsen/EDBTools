using EDBTools.Common;
using EDBTools.Geo.Headers;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Map
{
    public class MapHeader
    {
        public RelPtr RP_BSPTree { get; set; }
        public GeoArray PathsArray { get; set; }
        public GeoArray LightsArray { get; set; }
        public GeoArray CamerasArray { get; set; }
        public GeoArray SpecialsArray { get; set; }
        public GeoRelArray ClustersArray { get; set; }
        public GeoArray SoundsArray { get; set; }
        public GeoRelArray PortalsArray { get; set; }
        public GeoRelArray SkysArray { get; set; }
        public GeoRelArray PlacementsArray { get; set; }
        public GeoRelArray PlacementGroupsArray { get; set; }
        public RelPtr RP_TriggerHeader { get; set; }
        public uint[] ZoneSectionList { get; set; }
        public EXVector3[] BoundsBox { get; set; }
        public uint NumZones { get; set; }
        //public List<GeoMapZone> Zones { get; set; }

        public MapHeader(BinaryReader reader, bool bigEndian)
        {
            //Skip past first 4 bytes as they're used for the vtable in-game
            reader.BaseStream.Seek(0x4, SeekOrigin.Current);

            RP_BSPTree = new RelPtr(reader, bigEndian);
            PathsArray = new GeoArray(reader, bigEndian);
            LightsArray = new GeoArray(reader, bigEndian);
            CamerasArray = new GeoArray(reader, bigEndian);
            SpecialsArray = new GeoArray(reader, bigEndian);
            ClustersArray = new GeoRelArray(reader, bigEndian);
            SoundsArray = new GeoArray(reader, bigEndian);
            PortalsArray = new GeoRelArray(reader, bigEndian);
            SkysArray = new GeoRelArray(reader, bigEndian);
            PlacementsArray = new GeoRelArray(reader, bigEndian);
            PlacementGroupsArray = new GeoRelArray(reader, bigEndian);
            RP_TriggerHeader = new RelPtr(reader, bigEndian);

            ZoneSectionList = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                ZoneSectionList[i] = reader.ReadUInt32(bigEndian);
            }

            BoundsBox = new EXVector3[]
            {
                new EXVector3(reader, bigEndian),
                new EXVector3(reader, bigEndian)
            };
        }
    }
}
