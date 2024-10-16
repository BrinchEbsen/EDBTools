using EDBTools.Geo;
using EDBTools.Geo.SpreadSheet;
using EDBTools.Geo.SpreadSheet.Serialization;

namespace EDBToolsTest
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            SpreadSheetCollectionFormat generateFormat = new()
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
                                                    Columns =
                                                    [
                                                        new() { Name = "MapGeoHash",    Type = "hashcode" },
                                                        new() { Name = "MaxDarkGems",   Type = "u8" },
                                                        new() { Name = "MaxDragonEggs", Type = "u8" },
                                                        new() { Name = "MaxLightGems",  Type = "u8" }
                                                    ]
                                                }
                                            },
                                            {
                                                1,
                                                new DataSheetFormat()
                                                {
                                                    RowSize = 6,
                                                    Columns =
                                                    {
                                                        new() { Name = "ElemID",    Type = "u8" },
                                                        new() { Name = "Data_1",    Type = "u8" },
                                                        new() { Name = "Data_2",    Type = "u8" },
                                                        new() { Name = "Data_3",    Type = "u8" },
                                                        new() { Name = "Data_4",    Type = "u8" },
                                                        new() { Name = "Data_5",    Type = "u8" }
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
                                                    Columns =
                                                    {
                                                        new() { Name = "Col1", Type = "u32" },
                                                        new() { Name = "Col2", Type = "bitfield_u32" },
                                                        new() { Name = "Col3", Type = "bitfield_u32" }
                                                    },
                                                    BitFields =
                                                    [
                                                        new()
                                                        {
                                                            FieldName = "Col2",
                                                            Bits =
                                                            {
                                                                new() { Num = 0, Name = "MyFirstBit" },
                                                                new() { Num = 1, Name = "MySecondBit" }
                                                            }
                                                        },
                                                        new()
                                                        {
                                                            FieldName = "Col3",
                                                            Bits =
                                                            {
                                                                new() { Num = 2, Name = "MyThirdBit" },
                                                                new() { Num = 3, Name = "MyFourthBit" }
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
            Console.WriteLine(SpreadSheetYamlParser.FormatToYaml(generateFormat));

            Console.Write("SpreadSheet YAML: ");
            string spreadSheetYaml = File.ReadAllText(Console.ReadLine());
            SpreadSheetCollectionFormat format = SpreadSheetYamlParser.YamlToFormat(spreadSheetYaml);

            Console.Write("GeoFile: ");
            string? line = Console.ReadLine();
            if (line == null) { return; }

            if (Path.Exists(line))
            {
                using (BinaryReader reader = new(File.OpenRead(line)))
                {
                    GeoFile geoFile = new(reader);
                    geoFile.ReadSpreadSheets(reader, geoFile.BigEndian, generateFormat);
                    Console.WriteLine(geoFile.ToString());
                }
            }

            Console.ReadLine();
        }

        static readonly string testYaml = @"geo_files:
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