using EDBTools.Geo;

namespace EDBToolsTest
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            string? line = Console.ReadLine();
            if (line == null) { return; }

            if (Path.Exists(line))
            {
                using (BinaryReader reader = new BinaryReader(File.OpenRead(line)))
                {
                    GeoFile geoFile = new GeoFile(reader);
                    Console.WriteLine(geoFile.ToString());
                }
            }

            Console.ReadLine();
        }
    }
}