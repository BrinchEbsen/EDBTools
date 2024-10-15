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
        //num, name
        public Dictionary<int, string> Bits { get; set; } = new Dictionary<int, string>();
    }
}
