using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a column in a datasheet.
    /// </summary>
    [YamlSerializable]
    public class DataSheetColumnFormat
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
