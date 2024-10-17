using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a collection of spreadsheets divided into what geofile each can be found in.
    /// </summary>
    [YamlSerializable]
    public class SpreadSheetCollectionFormat
    {
        //geo hashcode, geofile with spreadsheet
        public Dictionary<uint, SpreadSheetGeoFileFormat> GeoFiles { get; set; } = new Dictionary<uint, SpreadSheetGeoFileFormat>();
    }
}
