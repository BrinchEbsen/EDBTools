using EDBTools.Common;
using System.IO;

namespace EDBTools.Geo.Map.Triggers
{
    public class GeoMapTriggerHeader
    {
        public GeoRelArray TriggerArray { get; private set; }
        public RelPtr TriggerScripts { get; private set; }
        public RelPtr TriggerTypes { get; private set; }
        public RelPtr TriggerCollisions { get; private set; }

        public GeoMapTriggerHeader(BinaryReader reader, bool bigEndian)
        {
            TriggerArray = new GeoRelArray(reader, bigEndian);
            TriggerScripts = new RelPtr(reader, bigEndian);
            TriggerTypes = new RelPtr(reader, bigEndian);
            TriggerCollisions = new RelPtr(reader, bigEndian);
        }
    }
}
