namespace EDBTools.Geo.SpreadSheet
{
    /// <summary>
    /// The type of a spreadsheet.
    /// </summary>
    public enum SpreadSheetTypes
    {
        /// <summary>
        /// A type of spreadsheet that contains text stored in sections that can be loaded separately.
        /// </summary>
        SHEET_TYPE_TEXT = 1,
        /// <summary>
        /// A spreadsheet with user-defined data, which can contain multiple data sheets.
        /// </summary>
        SHEET_TYPE_DATA = 2
    }
}
