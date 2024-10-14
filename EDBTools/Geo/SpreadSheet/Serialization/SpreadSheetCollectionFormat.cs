using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    public class SpreadSheetCollectionFormat
    {
        public Dictionary<uint, SpreadSheetGeoFileFormat> GeoFiles { get; set; }
    }
}
