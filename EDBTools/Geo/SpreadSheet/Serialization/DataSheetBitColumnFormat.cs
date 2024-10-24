﻿using YamlDotNet.Serialization;

namespace EDBTools.Geo.SpreadSheet.Serialization
{
    /// <summary>
    /// Yaml-serializable object containing formatting information for a datasheet column defining a bit in a bitfield.
    /// </summary>
    [YamlSerializable]
    public class DataSheetBitColumnFormat
    {
        public int Num { get; set; }
        public string Name { get; set; }
    }
}
