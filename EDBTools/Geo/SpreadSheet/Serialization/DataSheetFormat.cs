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
        //name, type
        public Dictionary<string, string> Columns { get; set; } = new Dictionary<string, string>();
        public List<SheetBitFieldFormat> BitFields { get; set; } = new List<SheetBitFieldFormat>();
    }
}
