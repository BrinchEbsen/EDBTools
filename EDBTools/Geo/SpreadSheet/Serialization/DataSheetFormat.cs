using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    public class DataSheetFormat
    {
        public int RowSize { get; set; }
        public Dictionary<string, string> Columns { get; set; }
    }
}
