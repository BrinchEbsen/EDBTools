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
        public List<DataSheetColumnFormat> Columns { get; set; } = new List<DataSheetColumnFormat>();
        public List<SheetBitFieldFormat> BitFields { get; set; } = new List<SheetBitFieldFormat>();
    }
}
