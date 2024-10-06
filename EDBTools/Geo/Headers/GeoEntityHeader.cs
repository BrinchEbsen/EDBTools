using EDBTools.Common;
using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public class GeoEntityHeader : GeoCommonArrayElement
    {
        public override long HEADER_SIZE
        {
            get { return base.HEADER_SIZE + 0x4; }
        }

        public RelPtr LodTable { get; private set; }

        public GeoEntityHeader(BinaryReader reader, bool bigEndian) : base(reader, bigEndian)
        {
            LodTable = new RelPtr(reader, bigEndian);
        }
    }
}
