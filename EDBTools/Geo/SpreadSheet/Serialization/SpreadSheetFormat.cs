using EDBTools.Geo.SpreadSheet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
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
