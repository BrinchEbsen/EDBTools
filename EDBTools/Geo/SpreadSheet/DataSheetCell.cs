using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDBTools.Geo.SpreadSheet
{
    public class DataSheetCell<T>
    {
        public T Data { get; set; }

        public DataSheetCell(T data) { Data = data; }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
