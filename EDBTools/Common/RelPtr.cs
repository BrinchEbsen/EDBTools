using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Common
{
    /// <summary>
    /// Relative pointer - references a address a certain amount of bytes ahead/behind of its own position.
    /// </summary>
    public class RelPtr
    {
        /// <summary>
        /// The address of this relative pointer.
        /// </summary>
        public long Address { get; private set; }
        /// <summary>
        /// The offset contained within this relative pointer.
        /// </summary>
        public int Offset { get; private set; }

        public RelPtr(long addr, int offs) 
        { 
            Address = addr;
            Offset = offs;
        }

        public RelPtr(BinaryReader reader, bool bigEndian)
        {
            Address = reader.BaseStream.Position;
            Offset = reader.ReadInt32(bigEndian);
        }

        /// <summary>
        /// Get the absolute address this relative pointer is referencing.
        /// </summary>
        /// <returns>The absolute address this relative pointer is referencing.
        /// Will be 0 if offset is 0 (null pointer).</returns>
        public long ToAbsolute()
        {
            if (Offset == 0) return 0;

            return Address + Offset;
        }
    }
}
