using EDBTools.Geo.SpreadSheet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    public class SpreadSheetFormat
    {
        //sheet number, data sheet
        public Dictionary<int, DataSheetFormat> Sheets { get; set; } = new Dictionary<int, DataSheetFormat>();
    }
}
