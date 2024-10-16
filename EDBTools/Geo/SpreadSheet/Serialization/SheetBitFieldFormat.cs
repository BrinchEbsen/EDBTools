using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    public class SheetBitFieldFormat
    {
        public string FieldName { get; set; }
        public List<DataSheetBitColumnFormat> Bits { get; set; } = new List<DataSheetBitColumnFormat>();
    }
}
