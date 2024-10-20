using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a bitfield in a datasheet, with a set of definitions for each bit.
    /// </summary>
    [YamlSerializable]
    public class SheetBitFieldFormat
    {
        public string FieldName { get; set; }
        public List<DataSheetBitColumnFormat> Bits { get; set; } = new List<DataSheetBitColumnFormat>();
    }
}
