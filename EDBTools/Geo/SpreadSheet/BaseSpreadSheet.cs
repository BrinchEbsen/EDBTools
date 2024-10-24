namespace EDBTools.Geo.SpreadSheet.Data
{
    /// <summary>
    /// Base class for all types of spreadsheet. Use the <see cref="Type"/> field to check the type.
    /// </summary>
    public abstract class BaseSpreadSheet
    {
        /// <summary>
        /// Base address of this spreadsheet's binary data in the file.
        /// </summary>
        public long Address { get; protected set; }

        /// <summary>
        /// Hashcode for this spreadsheet. Section HT_SpreadSheet (0x14XXXXXX).
        /// </summary>
        public uint HashCode { get; protected set; }

        /// <summary>
        /// The type of this spreadsheet, which determines what kind of data is to be stored within.
        /// </summary>
        public SpreadSheetTypes Type { get; protected set; }
    }
}
