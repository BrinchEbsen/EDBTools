using EDBTools.Common;
using Extensions;
using System.IO;

namespace EDBTools.Geo.Map.Triggers
{
    public class GeoTrigger
    {
        public const uint TRIGGER_DATA_AMOUNT = 16;
        public const uint TRIGGER_LINKS_AMOUNT = 8;

        //Trigger flag constants
        public const uint TRIGFLAGS_DATA_START  =        0x1; //Bit 0
        public const uint TRIGFLAGS_DATA_MASK   =     0xFFFF; //Bits 0-15
        public const uint TRIGFLAGS_LINKS_START =    0x10000; //Bit 16
        public const uint TRIGFLAGS_LINKS_MASK  =   0xFF0000; //Bits 16-23
        public const uint TRIGFLAGS_GFXHASH     =  0x1000000; //Bit 24
        public const uint TRIGFLAGS_GEOHASH     =  0x2000000; //Bit 25
        public const uint TRIGFLAGS_SCRIPT      =  0x4000000; //Bit 26
        public const uint TRIGFLAGS_TINT        = 0x10000000; //Bit 28

        public long Address { get; private set; }

        public ushort TypeIndex { get; private set; }

        public ushort Debug { get; private set; }

        public uint GameFlags { get; private set; }

        /// <summary>
        /// A set of flags that determine the type of data stored in this trigger:
        /// <para>Bits 0-15 determine which of the 16 data slots are included in the file.</para>
        /// <para>Bits 16-23 determine which of the 8 link slots are included in the file.</para>
        /// <para>Bit 24 determines if the trigger has a GFX hashcode reference.</para>
        /// <para>Bit 25 determines if the trigger has a Geo hashcode reference.</para>
        /// <para>Bit 26 determines if the trigger has a gamescript attached.</para>
        /// <para>Bit 28 determines if the trigger has a tint color.</para>
        /// </summary>
        public uint TrigFlags { get; private set; }

        public EXVector3 Position { get; private set; }

        public EXVector3 Rotation { get; private set; }

        public EXVector3 Scale { get; private set; }

        public EXVar32[] Data { get; private set; } = new EXVar32[TRIGGER_DATA_AMOUNT];

        public ushort[] Links { get; private set; } = new ushort[TRIGGER_LINKS_AMOUNT];

        public uint GfxHashRef { get; private set; }

        public uint GeoFileHashRef { get; private set; }


        public RGBA Tint { get; private set; }

        public uint ScriptIndex { get; private set; }

        public GeoTrigger(BinaryReader reader, bool bigEndian)
        {
            Address = reader.BaseStream.Position;

            TypeIndex = reader.ReadUInt16(bigEndian);
            Debug     = reader.ReadUInt16(bigEndian);
            GameFlags = reader.ReadUInt32(bigEndian);
            TrigFlags = reader.ReadUInt32(bigEndian);

            Position = new EXVector3(reader, bigEndian);
            Rotation = new EXVector3(reader, bigEndian);
            Scale    = new EXVector3(reader, bigEndian);

            for (int i = 0; i < TRIGGER_DATA_AMOUNT; i++)
            {
                if (HasData(i))
                {
                    Data[i] = new EXVar32(reader.ReadUInt32(bigEndian));
                }
            }

            for (int i = 0; (i < TRIGGER_LINKS_AMOUNT); i++)
            {
                if (HasLink(i))
                {
                    Links[i] = (ushort)reader.ReadUInt32(bigEndian);
                }
            }

            if (HasGfxHashRef())
            {
                GfxHashRef = reader.ReadUInt32(bigEndian);
            }

            if (HasGeoFileHashRef())
            {
                GeoFileHashRef = reader.ReadUInt32(bigEndian);
            }

            if (HasScript())
            {
                ScriptIndex = reader.ReadUInt32(bigEndian);
            }

            if (HasTint())
            {
                Tint = new RGBA(reader);
            }
        }

        /// <summary>
        /// Get the data at the specified index.
        /// </summary>
        /// <param name="index">Index into one of the 16 data slots.</param>
        /// <returns>The data, or null if data is not specified here by the trigger flags, or the index is out of bounds.</returns>
        public EXVar32 GetData(int index)
        {
            if (index < 0 || index >= TRIGGER_DATA_AMOUNT) return null;
            if (!HasData(index)) return null;

            return Data[index];
        }

        /// <summary>
        /// Check if the trigger has data stored in the specified slot.
        /// </summary>
        /// <param name="index">Index into the data slots (0-15).</param>
        /// <returns>Whether data exists in the specified slot.</returns>
        public bool HasData(int index)
        {
            if ((index < 0) || (index >= TRIGGER_DATA_AMOUNT))
                return false;

            return (TrigFlags & (TRIGFLAGS_DATA_START << index)) != 0;
        }

        /// <summary>
        /// Get the number of data slots in use by this trigger.
        /// </summary>
        /// <returns>The number of data slots in use by this trigger.</returns>
        public int CountData()
        {
            int count = 0;

            for (int i = 0; i < TRIGGER_DATA_AMOUNT; i++)
            {
                if (HasData(i)) { count++; }
            }

            return count;
        }

        /// <summary>
        /// Get the link at the specified index.
        /// </summary>
        /// <param name="index">Index into one of the 8 link slots.</param>
        /// <returns>Trigger index in the link slot, or null if the index was out of bounds.</returns>
        public ushort GetLink(int index)
        {
            if (index < 0 || index >= TRIGGER_LINKS_AMOUNT) return 0;

            return Links[index];
        }


        /// <summary>
        /// Check if the trigger has a link stored in the specified slot.
        /// </summary>
        /// <param name="index">Index into the link slots (0-7).</param>
        /// <returns>Whether a link exists in the specified slot.</returns>
        public bool HasLink(int index)
        {
            if ((index < 0) || (index >= TRIGGER_LINKS_AMOUNT))
                return false;

            return (TrigFlags & (TRIGFLAGS_LINKS_START << index)) != 0;
        }

        public int CountLinks()
        {
            int count = 0;

            for (int i = 0; i < TRIGGER_LINKS_AMOUNT; i++)
            {
                if (HasLink(i)) { count++; }
            }

            return count;
        }

        public bool HasGfxHashRef()
        {
            return (TrigFlags & TRIGFLAGS_GFXHASH) != 0;
        }

        public bool HasGeoFileHashRef()
        {
            return (TrigFlags & TRIGFLAGS_GEOHASH) != 0;
        }

        public bool HasScript()
        {
            return (TrigFlags & TRIGFLAGS_SCRIPT) != 0;
        }

        public bool HasTint()
        {
            return (TrigFlags & TRIGFLAGS_TINT) != 0;
        }
    }
}
