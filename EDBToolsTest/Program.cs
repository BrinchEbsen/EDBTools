using EDBTools.Geo;
using EDBTools.Geo.SpreadSheet;
using EDBTools.Geo.SpreadSheet.Serialization;

namespace EDBToolsTest
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            SpreadSheetCollectionFormat format = new()
            {
                GeoFiles = new Dictionary<uint, SpreadSheetGeoFileFormat>
                {
                    {
                        0x01000050,
                        new SpreadSheetGeoFileFormat()
                        {
                            SpreadSheets = new Dictionary<uint, SpreadSheetFormat>
                            {
                                {
                                    0x14000028,
                                    new SpreadSheetFormat()
                                    {
                                        Sheets = new Dictionary<int, DataSheetFormat>
                                        {
                                            {
                                                0,
                                                new DataSheetFormat()
                                                {
                                                    RowSize = 8,
                                                    Columns = new Dictionary<string, string>
                                                    {
                                                        { "MapGeoHash", "hashcode" },
                                                        { "MaxDarkGems", "u8" },
                                                        { "MaxDragonEggs", "u8" },
                                                        { "MaxLightGems", "u8" }
                                                    }
                                                }
                                            },
                                            {
                                                1,
                                                new DataSheetFormat()
                                                {
                                                    RowSize = 6,
                                                    Columns = new Dictionary<string, string>
                                                    {
                                                        { "ElemID", "u8" },
                                                        { "Data_1", "u8" },
                                                        { "Data_2", "u8" },
                                                        { "Data_3", "u8" },
                                                        { "Data_4", "u8" },
                                                        { "Data_5", "u8" }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                {
                                    69,
                                    new SpreadSheetFormat()
                                    {
                                        Sheets = new Dictionary<int, DataSheetFormat>
                                        {
                                            {
                                                0,
                                                new DataSheetFormat()
                                                {
                                                    RowSize = 6,
                                                    Columns = new Dictionary<string, string>
                                                    {
                                                        { "Col1", "u32" },
                                                        { "Col2", "bitfield_u32" },
                                                        { "Col3", "bitfield_u32" }
                                                    },
                                                    BitFields =
                                                    [
                                                        new()
                                                        {
                                                            FieldName = "Col2",
                                                            Bits = new Dictionary<int, string>
                                                            {
                                                                { 0, "MyFirstBit" },
                                                                { 1, "MySecondBit" }
                                                            }
                                                        },
                                                        new()
                                                        {
                                                            FieldName = "Col3",
                                                            Bits = new Dictionary<int, string>
                                                            {
                                                                { 2, "MyThirdBit" },
                                                                { 3, "MyFourthBit" }
                                                            }
                                                        }
                                                    ]
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            string yaml = SpreadSheetYamlParser.FormatToYaml(format);

            SpreadSheetCollectionFormat newFormat = SpreadSheetYamlParser.YamlToFormat(doc);

            string? line = Console.ReadLine();
            if (line == null) { return; }

            if (Path.Exists(line))
            {
                using (BinaryReader reader = new(File.OpenRead(line)))
                {
                    GeoFile geoFile = new(reader);
                    Console.WriteLine(geoFile.ToString());
                }
            }

            Console.ReadLine();
        }

        static readonly string doc = @"geo_files:
  0x01000050:
    spread_sheets:
      0x14000028:
        sheets:
          0:
            row_size: 8
            columns:
              MapGeoHash: hashcode
              MaxDarkGems: u8
              MaxDragonEggs: u8
              MaxLightGems: u8
            bit_fields: []
          1:
            row_size: 6
            columns:
              ElemID: u8
              Data_1: u8
              Data_2: u8
              Data_3: u8
              Data_4: u8
              Data_5: u8
            bit_fields: []
      69:
        sheets:
          0:
            row_size: 6
            columns:
              Col1: u32
              Col2: bitfield_u32
              Col3: bitfield_u32
            bit_fields:
            - field_name: Col2
              bits:
                0: MyFirstBit
                1: MySecondBit
            - field_name: Col3
              bits:
                2: MyThirdBit";
    }
}