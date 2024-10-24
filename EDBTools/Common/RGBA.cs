using System.Drawing;
using System.IO;

namespace EDBTools.Common
{
    public class RGBA
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public byte A { get; set; }

        public RGBA()
        {
            R = 0;
            G = 0;
            B = 0;
            A = 0;
        }

        public RGBA(BinaryReader reader)
        {
            R = reader.ReadByte();
            G = reader.ReadByte();
            B = reader.ReadByte();
            A = reader.ReadByte();
        }

        public RGBA(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public RGBA(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = 0;
        }

        public RGBA(Color col)
        {
            R = col.R;
            G = col.G;
            B = col.B;
            A = col.A;
        }

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }
}
