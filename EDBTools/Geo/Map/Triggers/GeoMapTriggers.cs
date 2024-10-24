using EDBTools.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Map.Triggers
{
    public class GeoMapTriggers
    {
        public GeoMapTriggerHeader Header { get; private set; }

        public List<RelPtr> TriggerTable { get; private set; }

        public List<RelPtr> TriggerScriptTable { get; private set; }

        public List<GeoTriggerScript> TriggerScripts { get; private set; }

        public List<GeoTrigger> Triggers { get; private set; }

        public List<GeoTriggerType> TriggerTypes { get; private set; }

        public GeoMapTriggers(BinaryReader reader, bool bigEndian)
        {
            Header = new GeoMapTriggerHeader(reader, bigEndian);

            //Read trigger header table
            TriggerTable = new List<RelPtr>(Header.TriggerArray.Size);

            reader.BaseStream.Seek(Header.TriggerArray.Offset.AbsoluteAddress, SeekOrigin.Begin);
            for (int i = 0; i < Header.TriggerArray.Size; i++)
            {
                TriggerTable.Add(new RelPtr(reader, bigEndian));
                reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            //Read triggers
            Triggers = new List<GeoTrigger>(TriggerTable.Count);

            int highestScriptIndex = 0;
            int highestTypeIndex = 0;

            foreach(RelPtr ptr in TriggerTable)
            {
                reader.BaseStream.Seek(ptr.AbsoluteAddress, SeekOrigin.Begin);
                var trigger = new GeoTrigger(reader, bigEndian);
                Triggers.Add(trigger);

                highestScriptIndex = Math.Max(highestScriptIndex, (int)trigger.ScriptIndex);
                highestTypeIndex   = Math.Max(highestTypeIndex,   (int)trigger.TypeIndex);
            }

            //Read triggertypes
            TriggerTypes = new List<GeoTriggerType>(highestTypeIndex);
            
            reader.BaseStream.Seek(Header.TriggerTypes.AbsoluteAddress, SeekOrigin.Begin);
            for (int i = 0; i < highestTypeIndex; i++)
            {
                TriggerTypes.Add(new GeoTriggerType(reader, bigEndian));
            }

            //Read trigger scripts table
            TriggerScriptTable = new List<RelPtr>(highestScriptIndex);

            reader.BaseStream.Seek(Header.TriggerScripts.AbsoluteAddress, SeekOrigin.Begin);
            for (int i = 0; i < highestScriptIndex; i++)
            {
                TriggerScriptTable.Add(new RelPtr(reader, bigEndian));
                reader.BaseStream.Seek(0x4, SeekOrigin.Current);
            }

            //Read trigger scripts
            TriggerScripts = new List<GeoTriggerScript>(TriggerScriptTable.Count);

            foreach(RelPtr ptr in TriggerScriptTable)
            {
                reader.BaseStream.Seek(ptr.AbsoluteAddress, SeekOrigin.Begin);
                TriggerScripts.Add(new GeoTriggerScript(reader, bigEndian));
            }
        }
    }
}
