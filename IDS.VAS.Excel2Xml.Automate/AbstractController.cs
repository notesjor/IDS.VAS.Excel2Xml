using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using IDS.VAS.Excel2Xml.Convert.Model;
using IDS.VAS.Excel2Xml.Model;

namespace IDS.VAS.Excel2Xml.Automate
{
  public abstract class AbstractController
  {
    public void Convert(string input, string output)
      => Convert(ReadExcel(input), output);

    private DataSet ReadExcel(string path)
    {
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        var reader = ExcelReaderFactory.CreateReader(fs, new ExcelReaderConfiguration()
        {
          // Gets or sets the encoding to use when the input XLS lacks a CodePage
          // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
          // Default: cp1252 (XLS BIFF2-5 and CSV only)
          FallbackEncoding = Encoding.GetEncoding(1252),

          // Gets or sets a value indicating whether to leave the stream open after
          // the IExcelDataReader object is disposed. Default: false
          LeaveOpen = false
        });

        return reader.AsDataSet(new ExcelDataSetConfiguration()
        {
          // Gets or sets a value indicating whether to set the DataColumn.DataType 
          // property in a second pass.
          UseColumnDataType = true,

          // Gets or sets a callback to determine whether to include the current sheet
          // in the DataSet. Called once per sheet before ConfigureDataTable.
          FilterSheet = (tableReader, sheetIndex) => sheetIndex == 0,

          // Gets or sets a callback to obtain configuration options for a DataTable. 
          ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
          {
            // Gets or sets a value indicating the prefix of generated column names.
            EmptyColumnNamePrefix = "Column",

            // Gets or sets a value indicating whether to use a row from the 
            // data as column names.
            UseHeaderRow = true,

            // Gets or sets a callback to determine whether to include the 
            // current row in the DataTable.
            FilterRow = (rowReader) => { return true; },

            // Gets or sets a callback to determine whether to include the specific
            // column in the DataTable. Called once per column after reading the 
            // headers.
            FilterColumn = (rowReader, columnIndex) => { return true; }
          }
        });
      }
    }

    protected abstract void Convert(DataSet excel, string output);

    protected List<string> GetSamples(DataRow[] items, ExcelColumnMapper mapper)
    {
      return items.Select(row => KwicFix(KwicHighlight(row, new Kwic
                   {
                     Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
                     Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
                     Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
                     Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString()
                   })))
                  .Select(kwic => $"\t\t\t<sample id=\"s_{kwic.Id}\" cosmas=\"{kwic.Sigle}\">{kwic.Text}</sample>")
                  .ToList();
    }
    
    private Kwic KwicHighlight(DataRow row, Kwic str)
    {
      return str;
    }

    protected Kwic KwicFix(Kwic kwic)
    {
      var str = kwic.Text.Replace(" , ", ", ")
                    .Replace(" : ", ": ")
                    .Replace(" ? ", "? ")
                    .Replace(" ! ", "! ")
                    .Replace(" . ", ". ")
                    .Replace(" ; ", "; ")
                    .Replace("  ", " ")
                    .Replace("&", "&amp;");
      if (str.EndsWith(" ."))
        str = str.Substring(0, str.Length - 2) + ".";
      if (str.EndsWith(" ?"))
        str = str.Substring(0, str.Length - 2) + "!";
      if (str.EndsWith(" !"))
        str = str.Substring(0, str.Length - 2) + "?";

      kwic.Text = str;

      return kwic;
    }
  }
}
