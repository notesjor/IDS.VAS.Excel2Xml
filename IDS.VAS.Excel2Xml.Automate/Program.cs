using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using IDS.VAS.Excel2Xml.Automate.Properties;
using IDS.VAS.Excel2Xml.Model;

namespace IDS.VAS.Excel2Xml.Convert
{
  class Program
  {
    private static DataSet _workbook;
    private static string _mainPattern;
    private static string _baseDir;

    static void Main(string[] args)
    {
      if (args.Length == 0)
        return;

      _baseDir = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]));

      ReadExcel(args[0]);
      ReadWorkbook();
    }

    private static void ReadExcel(string path)
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

        _workbook = reader.AsDataSet(new ExcelDataSetConfiguration()
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

    private static void ReadWorkbook()
    {
      foreach (DataTable sheet in _workbook.Tables)
      {
        var mapper = new ExcelColumnMapper();
        if (mapper.Map(sheet))
          continue;

        var outputDir = Path.Combine(_baseDir, sheet.TableName);
        if (!Directory.Exists(outputDir))
          Directory.CreateDirectory(outputDir);

        var idx = mapper.Mapping["MUSTER"];
        var patterns = new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[idx].ToString());
        foreach (var pattern in patterns)
        {
          if (string.IsNullOrWhiteSpace(pattern))
            continue;

          var items = sheet.Rows.Cast<DataRow>()
                           .Where(row => row.ItemArray[mapper.Mapping["MUSTER"]].ToString() == pattern)
                           .Where(row => row.ItemArray[mapper.Mapping["EINGANG"]].ToString().Trim() == "1")
                           .ToArray();

          if (items.Length == 0)
            continue;

          var templateJ = Resources.TEMPLATE.Replace("$$$MUSTER$$$", pattern);
          templateJ = templateJ.Replace("$$$SAMPLES$$$", GetSamples(items, mapper));
          templateJ = templateJ.Replace("$$$FORMS$$$", GetForms(items, mapper));
          templateJ = templateJ.Replace("$$$MAIN_PATTERN$$$", _mainPattern); // Muss nach FORMS ausgeführt werden, da dort _mainPattern ermittelt wird
          templateJ = templateJ.Replace("$$$PREDICATES$$$", GetPredicates(items, mapper));
          File.WriteAllText(Path.Combine(outputDir, $"{pattern}_vJAN.xml"), templateJ, Encoding.UTF8);
        }
      }
    }
    
    private static string GetSamples(DataRow[] items, ExcelColumnMapper mapper)
    {
      var samples = new List<string>();
      var xrefs = new List<string>();
      foreach (var row in items)
      {
        var id = row.ItemArray[mapper.Mapping["#"]].ToString();
        var kw = KwicFix(KwicHighlight(row, row.ItemArray[mapper.Mapping["BELEG"]].ToString()));

        samples.Add($"\t\t\t<sample id=\"s_{id}\">{kw}</sample>");
        xrefs.Add($"\t<xref href=\"s_{id}\"/> {kw}");
      }
      return $"<samples>\r\n{string.Join("\r\n", samples)}\r\n\t\t</samples>\r\n<!-- TODO: Folgender Code (auskommentiert) als Beleg-Referenz an benötigten Stellen einfügen -->\r\n<!--\r\n<examples>\r\n{string.Join("\r\n", xrefs)}\r\n</examples>\r\n-->";
    }

    private static string KwicHighlight(DataRow row, string str)
    {
      return str;
    }

    private static string KwicFix(string str)
    {
      str = str.Replace(" , ", ", ")
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
      return str;
    }

    private static string GetForms(DataRow[] items, ExcelColumnMapper mapper)
    {
      var dict = new Dictionary<string, FormSlot>();
      foreach (var item in items)
      {
        var slot = new FormSlot(mapper, item);
        var key = slot.GetXml(false);
        if (dict.ContainsKey(key)) // MERGE Multi-KWICs
        {
          var pair = slot.Kwics.First();
          dict[key].Kwics.Add(pair.Key, pair.Value);
          continue;
        }
        dict.Add(key, slot);
      }

      // Detect _mainPattern
      _mainPattern = dict.OrderByDescending(x => x.Value.Kwics.Count).First().Value.GetXml(true);
      _mainPattern = _mainPattern.Replace("<prototype>", "").Replace("</prototype>", "");

      var stb = new StringBuilder();
      stb.Append("<forms>\r\n");
      stb.Append("\t\t\t<akt>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "akt"))
        stb.Append("\t\t\t" + pair.Value.GetXml(true));
      stb.Append("\t\t\t</akt>\r\n");
      stb.Append("\t\t\t<med>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "med"))
        stb.Append("\t\t\t" + pair.Value.GetXml(true));
      stb.Append("\t\t\t</med>\r\n");
      stb.Append("\t\t\t<pass>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "pass"))
        stb.Append("\t\t\t" + pair.Value.GetXml(true));
      stb.Append("\t\t\t</pass>\r\n");
      stb.Append("\t\t</forms>\r\n");

      return stb.ToString();
    }

    private static string GetPredicates(DataRow[] items, ExcelColumnMapper mapper)
    {
      var simple = new Dictionary<string, List<string>>();
      var complex = new Dictionary<string, List<string>>();

      foreach (var row in items)
      {
        var id = row.ItemArray[mapper.Mapping["#"]].ToString();
        var kw = KwicFix(row.ItemArray[mapper.Mapping["BELEG"]].ToString());
        var si = row.ItemArray[mapper.Mapping["PRÄDIKATSKERN(LEX)"]].ToString().Trim();
        var co = row.ItemArray[mapper.Mapping["KOMPLEXESPRÄDIKAT(LEX)"]].ToString().Trim().Replace("_", " ");

        if (!string.IsNullOrWhiteSpace(si))
        {
          if (simple.ContainsKey(si))
            simple[si].Add($"\t\t\t\t\t\t\t<xref href=\"s_{id}\"/> <!-- {kw} -->");
          else
            simple.Add(si, new List<string> { $"\t\t\t\t\t\t\t<xref href=\"s_{id}\"/> <!-- {kw} -->" });
        }

        if (!string.IsNullOrWhiteSpace(co))
        {
          if (complex.ContainsKey(co))
            complex[co].Add($"\t\t\t\t\t\t\t<xref href=\"s_{id}\"/> <!-- {kw} -->");
          else
            complex.Add(co, new List<string> { $"\t\t\t\t\t\t\t<xref href=\"s_{id}\"/> <!-- {kw} -->" });
        }
      }

      return $"<predicate-list label=\"Verben\">\r\n{GetPredicateItems(simple)}\r\n\t\t\t\t</predicate-list>\r\n\t\t\t\t<predicate-list label=\"Komplexe Prädikate\">\r\n{GetPredicateItems(complex)}\r\n\t\t\t\t</predicate-list>";
    }

    private static string GetPredicateItems(Dictionary<string, List<string>> items)
    {
      return string.Join("\r\n",
                         items.Select(x =>
                                        $"\t\t\t\t\t<predicate value=\"{x.Key}\" alt=\"\">\r\n\t\t\t\t\t\t<examples>\r\n{string.Join("\r\n", x.Value)}\r\n\t\t\t\t\t\t</examples>\r\n\t\t\t\t\t</predicate>"));
    }
  }
}
