using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    public class SpreadSheetGeoFileFormat
    {
        //spreadsheet hashcode, spreadsheet
        public Dictionary<uint, SpreadSheetFormat> SpreadSheets { get; set; } = new Dictionary<uint, SpreadSheetFormat>();
    }
}
