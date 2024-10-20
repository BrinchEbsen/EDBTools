using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a spreadsheet with a list of definitions for datasheets.
    /// </summary>
    [YamlSerializable]
    public class SpreadSheetFormat
    {
        //sheet number, data sheet
        public Dictionary<int, DataSheetFormat> Sheets { get; set; } = new Dictionary<int, DataSheetFormat>();
    }
}
