using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for spreadsheets inside a geofile.
    /// </summary>
    [YamlSerializable]
    public class SpreadSheetGeoFileFormat
    {
        //spreadsheet hashcode, spreadsheet
        public Dictionary<uint, SpreadSheetFormat> SpreadSheets { get; set; } = new Dictionary<uint, SpreadSheetFormat>();
    }
}
