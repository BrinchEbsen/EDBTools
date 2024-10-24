using EDBTools.Geo.SpreadSheet.Serialization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace EDBTools.Geo.SpreadSheet
{
    /// <summary>
    /// Converts between spreadsheet formatting information stored in yaml string and serializable object format.
    /// </summary>
    public static class SpreadSheetYamlParser
    {
        /// <summary>
        /// Convert a serializable spreadsheet format collection into a yaml string.
        /// </summary>
        /// <returns>The serialized yaml string representing the formatting collection.</returns>
        public static string FormatToYaml(SpreadSheetCollectionFormat format)
        {
            var serializer = new SerializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return serializer.Serialize(format);
        }

        /// <summary>
        /// Convert a yaml string into a serializable spreadsheet format collection.
        /// </summary>
        /// <returns>The deserialized formatting collection.</returns>
        public static SpreadSheetCollectionFormat YamlToFormat(string yaml)
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            return deserializer.Deserialize<SpreadSheetCollectionFormat>(yaml);
        }
    }
}
