using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using ExcelDataReader;
using IDS.VAS.Excel2Xml.Automate.Model;
using IDS.VAS.Excel2Xml.Automate.Properties;
using IDS.VAS.Excel2Xml.Convert.Model;
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

          var patternFixed = PatternNameFix(pattern);
          IdGenerator.Init(patternFixed);

          var templateJ = Resources.TEMPLATE.Replace("$$$MUSTER$$$", patternFixed);
          templateJ = templateJ.Replace("$$$SAMPLES$$$", GetSamples(items, mapper));
          templateJ = templateJ.Replace("$$$FORMS$$$", GetForms(items, mapper));
          templateJ = templateJ.Replace("$$$MAIN_PATTERN$$$", _mainPattern); // Muss nach FORMS ausgeführt werden, da dort _mainPattern ermittelt wird
          templateJ = templateJ.Replace("$$$PREDICATES$$$", GetPredicates(items, mapper));
          File.WriteAllText(Path.Combine(outputDir, $"{FileNameFix(pattern)}.xml"), templateJ, Encoding.UTF8);
        }
      }
    }

    private static string PatternNameFix(string pattern)
    {
      return pattern.Substring(0, 1).ToUpper() + pattern.Substring(1).ToLower();
    }

    private static string FileNameFix(string pattern)
    {
      return pattern.ToLower()
                    .Replace("ä", "ae")
                    .Replace("ö", "oe")
                    .Replace("ü", "ue")
                    .Replace("ß", "ss");
    }

    private static string GetSamples(DataRow[] items, ExcelColumnMapper mapper)
    {
      var samples = new List<string>();
      foreach (var row in items)
      {
        var kwic = KwicFix(KwicHighlight(row, new Kwic
        {
          Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
          Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
          Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
          Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString(),
        }));

        samples.Add($"\t\t\t<sample id=\"s_{kwic.Id}\" cosmas=\"{kwic.Sigle}\">{kwic.Text}</sample>");
      }

      return $"<samples>\r\n{string.Join("\r\n", samples)}\r\n\t\t</samples>\r\n\t\t<!--\r\n\t\tTODO: \r\n\t\tFür Beispiele im Texte <xref>-Elemente kopieren\r\n\t\t\t<examples>\r\n\t\t\t\t<xref href=\"s_1072\"/>\r\n\t\t\t</examples>\r\n\r\n\t\tAufeinander folgende Beispiele in EINEM <examples>-Element bündeln\r\n\t\t  \t<examples>\r\n\t\t\t\t<xref href=\"s_1072\"/>\r\n\t\t\t\t<xref href=\"s_4075\"/>\r\n\t\t\t</examples>\r\n\r\n\t\tAuf diese Weise referenzierte Beispiele MÜSSEN oben im <samples>-Block ausgezeichnet werden! -->\r\n";
    }

    private static Kwic KwicHighlight(DataRow row, Kwic str)
    {
      return str;
    }

    private static Kwic KwicFix(Kwic kwic)
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

    private static string GetForms(DataRow[] items, ExcelColumnMapper mapper)
    {
      var dict = new Dictionary<string, FormSlot>();
      foreach (var item in items)
      {
        var slot = new FormSlot(mapper, item);
        var key = slot.GetXml(0, false);
        if (dict.ContainsKey(key)) // MERGE Multi-KWICs
        {
          var pair = slot.Kwics.First();
          dict[key].Kwics.Add(pair.Key, pair.Value);
          continue;
        }
        dict.Add(key, slot);
      }

      // Detect _mainPattern
      _mainPattern = dict.OrderByDescending(x => x.Value.Kwics.Count).First().Value.GetXml(3, false);
      _mainPattern = _mainPattern.Replace("<prototype>", "").Replace("</prototype>", "");

      var stb = new StringBuilder();
      stb.Append("<forms>\r\n");

      var akts = dict.Where(x => x.Value.Type == "akt").ToArray();
      if (akts.Length > 0)
      {
        stb.Append("\t\t\t<akt>\r\n");
        foreach (var pair in akts)
          stb.Append("\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t</akt>\r\n");
      }

      var meds = dict.Where(x => x.Value.Type == "kon").ToArray();
      if (meds.Length > 0)
      {
        stb.Append("\t\t\t<kon>\r\n");
        foreach (var pair in meds)
          stb.Append("\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t</kon>\r\n");
      }

      var passs = dict.Where(x => x.Value.Type == "pass").ToArray();
      if (passs.Length > 0)
      {
        stb.Append("\t\t\t<pass>\r\n");
        foreach (var pair in passs)
          stb.Append("\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t</pass>\r\n");
      }

      var ambigs = dict.Where(x => x.Value.Type == "ambig").ToArray();
      if (ambigs.Length > 0)
      {
        stb.Append("\t\t\t<ambig>\r\n");
        foreach (var pair in ambigs)
          stb.Append("\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t</ambig>\r\n");
      }

      stb.Append("\t\t<!-- TODO: ggf. löschen -->\r\n\t\t\t<section label=\"Besonderheiten\">\r\n\t\t\t\t<!-- \r\n\t\t\t\t\tText und Beispiele hierher \r\n\t\t\t\t\t<p></p>\r\n\t\t\t\t\t<examples></examples>\r\n\t\t\t\t-->\r\n\t\t\t\t<p/>\r\n\t\t\t</section>\r\n");
      stb.Append("\t\t</forms>\r\n");
      return stb.ToString();
    }

    private static string GetPredicates(DataRow[] items, ExcelColumnMapper mapper)
    {
      var complex = new Dictionary<string, Dictionary<string, List<string>>>();

      foreach (var row in items)
      {
        var kwic = KwicFix(new Kwic
        {
          Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
          Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
          Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
          Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString(),
        });
        var si = row.ItemArray[mapper.Mapping["PRÄDIKATSKERN(LEX)"]].ToString().Trim();
        var co = row.ItemArray[mapper.Mapping["PRÄDIKAT(LEX)"]].ToString().Trim().Replace("_", " ");
        
        if (complex.ContainsKey(si))
        {
          if (complex[si].ContainsKey(co))
            complex[si][co].Add($"\t\t\t\t\t\t\t<!-- <xref href=\"s_{kwic.Id}\"/> --> <!-- {kwic.Text} -->");
          else
            complex[si].Add(co, new List<string> { $"\t\t\t\t\t\t\t<xref href=\"s_{kwic.Id}\"/> <!-- {kwic.Text} -->" });
        }
        else
          complex.Add(si, new Dictionary<string, List<string>>
            {
              { co, new List<string>{$"\t\t\t\t\t\t\t<xref href=\"s_{kwic.Id}\"/> <!-- {kwic.Text} -->" } }
            });
      }

      return $"<predicate-list label=\"Komplexe Prädikate\">\r\n{GetPredicateItems(complex)}\r\n\t\t\t\t</predicate-list>";
    }

    private static string GetPredicateItems(Dictionary<string, List<string>> items)
    {
      return string.Join("\r\n", items.OrderBy(x => x.Key).Select(x => GetPredicateItems(x.Key, x.Value, null)));
    }

    private static string GetPredicateItems(Dictionary<string, Dictionary<string, List<string>>> items)
    {
      var res = new List<string>();
      foreach (var cluster in items.OrderBy(x => x.Key))
        res.AddRange(cluster.Value.OrderBy(x => x.Key).Select(x => GetPredicateItems(x.Key, x.Value, cluster.Key)));

      return string.Join("\r\n", res);
    }

    private static string GetPredicateItems(string key, IEnumerable<string> values, string alt)
      => $"\t\t\t\t\t<predicate value=\"{key}\" {(string.IsNullOrEmpty(alt) ? "alt=\"\"" : $"alt=\"{alt}\"")}>\r\n\t\t\t\t\t\t<examples>\r\n{string.Join("\r\n", values)}\r\n\t\t\t\t\t\t</examples>\r\n\t\t\t\t\t</predicate>";
  }
}
