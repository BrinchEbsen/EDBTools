using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.Headers
{
    public struct GeoSectionHeader : IGeoCommonHeader
    {
        public uint HashCode { get; set; }
        public uint StartOffset { get; set; }
        public uint EndOffset { get; set; }
    }
}
