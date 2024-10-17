using EDBTools.Geo;
using EDBTools.Geo.SpreadSheet;
using EDBTools.Geo.SpreadSheet.Serialization;

namespace EDBToolsTest
{
    internal static class Program
    {
        static string yamlDir = "../../../spreadsheets.yml";

        static void Main(string[] args)
        {
            if (!File.Exists(yamlDir)) { return; }

            string spreadSheetYaml = File.ReadAllText(yamlDir);
            SpreadSheetCollectionFormat format = SpreadSheetYamlParser.YamlToFormat(spreadSheetYaml);

            Console.Write("GeoFile: ");
            string? line = Console.ReadLine();
            if (line == null) { return; }

            if (Path.Exists(line))
            {
                using (BinaryReader reader = new(File.OpenRead(line)))
                {
                    GeoFile geoFile = new(reader);
                    Console.WriteLine(geoFile.ToString());

                    geoFile.ReadSpreadSheets(reader, geoFile.BigEndian, format);

                    foreach(BaseSpreadSheet spreadSheet in geoFile.SpreadSheets)
                    {
                        Console.WriteLine(spreadSheet.ToString());
                    }
                }
            }

            Console.ReadLine();
        }

        public static void TestYamlGeneration()
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
                                    0x14000005,
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
        }
    }
}