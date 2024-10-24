using Extensions;
using System.IO;

namespace EDBTools.Common
{
    /// <summary>
    /// Relative pointer - references an address a certain amount of bytes ahead/behind of its own position.
    /// </summary>
    public class RelPtr
    {
        /// <summary>
        /// The address of this relative pointer.
        /// This will equal the absolute address when added together with <see cref="Offset"/>.
        /// </summary>
        public long Address { get; private set; }

        /// <summary>
        /// The offset contained within this relative pointer.
        /// This will equal the absolute address when added together with <see cref="Address"/>.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// The absolute address this relative pointer is referencing.
        /// Will be 0 if the stored offset is 0.
        /// </summary>
        public long AbsoluteAddress
        {
            get
            {
                if (Offset == 0) return 0;

                return Address + Offset;
            }
            set
            {
                //Set the relative offset
                Offset = (int)(value - Address);
            }
        }

        /// <summary>
        /// Initialize a relative pointer with the given <paramref name="address"/> and <paramref name="offset"/>.
        /// </summary>
        public RelPtr(long address, int offset)
        {
            Address = address;
            Offset = offset;
        }

        /// <summary>
        /// Initialize a relative pointer with the data stored at the given <paramref name="reader"/>'s current position.
        /// </summary>
        public RelPtr(BinaryReader reader, bool bigEndian)
        {
            Address = reader.BaseStream.Position;
            Offset = reader.ReadInt32(bigEndian);
        }

        public override string ToString()
        {
            return string.Format("Rel: 0x{0:X}, Abs: 0x{1:X}", Offset, AbsoluteAddress);
        }
    }
}
