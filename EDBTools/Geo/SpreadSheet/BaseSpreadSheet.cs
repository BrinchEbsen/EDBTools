using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet
{
    /// <summary>
    /// Base class for all types of spreadsheet. Use the <see cref="Type"/> field to check the type.
    /// </summary>
    public abstract class BaseSpreadSheet
    {
        public SpreadSheetTypes Type { get; protected set; }
    }
}
