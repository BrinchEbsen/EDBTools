using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Map.Triggers
{
    /// <summary>
    /// A list of instructions organized into procedures with a set of local/global variables.
    /// </summary>
    public class GeoTriggerScript
    {
        public ushort NumLines { get; private set; }
        public byte NumVars { get; private set; }
        public byte NumGlobals { get; private set; }
        public byte NumProcs { get; private set; }

        public string Name { get; private set; }
        public byte[] VTable { get; private set; }
        public List<Procedure> Procedures { get; private set; }
        public List<CodeLine> Code { get; private set; }

        public GeoTriggerScript(BinaryReader reader, bool bigEndian)
        {
            //Seek past a null int at the start
            reader.BaseStream.Seek(0x4, SeekOrigin.Current);

            VTable = reader.ReadBytes(16);
            NumLines = reader.ReadUInt16(bigEndian);

            long temp = reader.BaseStream.Position;
            Name = reader.ReadASCIIString(7);
            //Increment address the correct amount
            reader.BaseStream.Seek(temp + 0x7, SeekOrigin.Begin);

            NumVars = reader.ReadByte();
            NumGlobals = reader.ReadByte();
            NumProcs = reader.ReadByte();

            //Seek past irrelevant data
            reader.BaseStream.Seek(0x4, SeekOrigin.Current);

            Procedures = new List<Procedure>(NumProcs);
            for (int i = 0; i < NumProcs; i++)
            {
                Procedure proc = new Procedure();

                proc.StartLine = reader.ReadUInt16(bigEndian); //0x2
                proc.Blocked = reader.ReadByte(); //0x3
                proc.Type = reader.ReadByte(); //0x4

                temp = reader.BaseStream.Position;
                proc.Name = reader.ReadASCIIString(10); //0xE
                reader.BaseStream.Seek(temp + 10, SeekOrigin.Begin);

                proc.Exclusive = reader.ReadByte(); //0xF
                proc.NumLocals = reader.ReadByte(); //0x10

                Procedures.Add(proc);
            }

            Code = new List<CodeLine>(NumLines);
            for (int i = 0; i < NumLines; i++)
            {
                CodeLine line = new CodeLine();

                line.InstructionID = reader.ReadByte();
                line.Data1 = reader.ReadByte();
                line.Data2 = reader.ReadByte();
                line.Data3 = reader.ReadByte();
                line.Data4 = reader.ReadInt32(bigEndian);

                Code.Add(line);
            }
        }
    }

    /// <summary>
    /// A logical set of instructions that can be called from other procedures. Has a defined start line and name.
    /// </summary>
    public struct Procedure
    {
        public ushort StartLine;
        public byte Blocked;
        public byte Type;
        public string Name;
        public byte Exclusive;
        public byte NumLocals;
    }

    /// <summary>
    /// An instruction with an ID and 4 units of data. The ID determines what the instruction does, with further behavior defined by the data.
    /// </summary>
    public struct CodeLine
    {
        public byte InstructionID;
        public byte Data1;
        public byte Data2;
        public byte Data3;
        public int Data4;
    }
}
