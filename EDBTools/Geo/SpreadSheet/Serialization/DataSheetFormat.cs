using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a datasheet with rows, columns and a set of bitfield definitions.
    /// </summary>
    [YamlSerializable]
    public class DataSheetFormat
    {
        public int RowSize { get; set; }
        public List<DataSheetColumnFormat> Columns { get; set; } = new List<DataSheetColumnFormat>();
        public List<SheetBitFieldFormat> BitFields { get; set; } = new List<SheetBitFieldFormat>();
    }
}
