using EDBTools.Geo.SpreadSheet.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EDBTools.Geo.SpreadSheet
{
    public static class SpreadSheetYamlParser
    {
        public static string FormatToYaml(SpreadSheetCollectionFormat format)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return serializer.Serialize(format);
        }

        public static SpreadSheetCollectionFormat YamlToFormat(string yaml)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return deserializer.Deserialize<SpreadSheetCollectionFormat>(yaml);
        }
    }
}
