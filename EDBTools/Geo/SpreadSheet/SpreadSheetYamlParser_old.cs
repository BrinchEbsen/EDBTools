using EDBTools.Geo.SpreadSheet.Serialization;
using System;
using System.IO;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace EDBTools.Geo.SpreadSheet
{
    public static class SpreadSheetYamlParser_old
    {
        public static SpreadSheetCollectionFormat YamlToFormat(string yamlStr)
        {
            var yaml = new YamlStream();
            yaml.Load(new StringReader(yamlStr));

            SpreadSheetCollectionFormat output = new SpreadSheetCollectionFormat();

            //Read information in nodes
            try
            {
                var geoFileHashes = (YamlMappingNode)yaml.Documents[0].RootNode;

                foreach (var entry in geoFileHashes.Children)
                {
                    output.GeoFiles.Add(
                        Convert.ToUInt32(((YamlScalarNode)entry.Key).Value, 16),
                        ReadGeoFileFormat((YamlMappingNode)entry.Value)
                    );
                }
            }
            catch (Exception ex)
            {
                throw new IOException("Incorrect format for SpeadSheet YAML document: " + ex.Message);
            }

            return output;
        }

        private static SpreadSheetGeoFileFormat ReadGeoFileFormat(YamlMappingNode root)
        {
            SpreadSheetGeoFileFormat output = new SpreadSheetGeoFileFormat();

            foreach (var entry in root.Children)
            {
                output.SpreadSheets.Add(
                    Convert.ToUInt32(((YamlScalarNode)entry.Key).Value, 16),
                    ReadSpreadSheetFormat((YamlMappingNode)entry.Value)
                );
            }

            return output;
        }

        private static SpreadSheetFormat ReadSpreadSheetFormat(YamlMappingNode root)
        {
            SpreadSheetFormat output = new SpreadSheetFormat();

            foreach (var entry in root.Children)
            {
                output.Sheets.Add(
                    int.Parse(((YamlScalarNode)entry.Key).Value),
                    ReadDataSheetFormat((YamlMappingNode)entry.Value)
                );
            }

            return output;
        }

        private static DataSheetFormat ReadDataSheetFormat(YamlMappingNode root)
        {
            DataSheetFormat output = new DataSheetFormat
            {
                RowSize = int.Parse(((YamlScalarNode)root.Children[new YamlScalarNode("row_size")]).Value)
            };

            var columnsRoot = (YamlSequenceNode)root.Children[new YamlScalarNode("columns")];

            foreach (var column in columnsRoot.Children.Cast<YamlMappingNode>())
            {
                output.Columns.Add(
                    ((YamlScalarNode)column.Children[new YamlScalarNode("name")]).Value,
                    ((YamlScalarNode)column.Children[new YamlScalarNode("type")]).Value
                );
            }

            //Check if bitfields list exists before reading it
            var bitfieldsKey = new YamlScalarNode("bitfields");
            if (root.Children.ContainsKey(bitfieldsKey))
            {
                var bitfieldsRoot = (YamlSequenceNode)root.Children[bitfieldsKey];

                foreach (var bitfield in bitfieldsRoot.Children.Cast<YamlMappingNode>())
                {
                    output.BitFields.Add(ReadBitFieldFormat(bitfield));
                }
            }

            return output;
        }

        private static SheetBitFieldFormat ReadBitFieldFormat(YamlMappingNode root)
        {
            SheetBitFieldFormat output = new SheetBitFieldFormat
            {
                FieldName = ((YamlScalarNode)root.Children[new YamlScalarNode("field")]).Value
            };

            var bitsRoot = (YamlSequenceNode)root.Children[new YamlScalarNode("bits")];

            foreach (var bit in bitsRoot.Children.Cast<YamlMappingNode>())
            {
                output.Bits.Add(
                    int.Parse(((YamlScalarNode)bit.Children[new YamlScalarNode("num")]).Value),
                    ((YamlScalarNode)bit.Children[new YamlScalarNode("name")]).Value
                );
            }

            return output;
        }
    }
}
