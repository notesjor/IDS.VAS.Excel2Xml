using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using ExcelDataReader;
using IDS.VAS.Excel2Json.Model;
using IDS.VAS.Excel2Xml.Model;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2Json
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
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

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

        var kwicFulltexts = new List<KwicFulltext>();
        var kwics = new Dictionary<int, int[]>();
        var patterns = new Dictionary<string, Pattern>();
        var articles = new List<Article>();

        var diathesis = new Dictionary<string, int>();
        var ktypes = new Dictionary<string, int>();
        var mtypes = new Dictionary<string, int>();
        var prdlex = new Dictionary<string, int>();
        var prdlexcore = new Dictionary<string, int>();
        var sources = new Dictionary<string, int>();

        var syn_figure = new Dictionary<string, int>();
        var syn_ground = new Dictionary<string, int>();
        var syn_prd = new Dictionary<string, int>();
        var syn_trigger = new Dictionary<string, int>();

        foreach (var pattern in new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[idx].ToString()))
        {
          if (string.IsNullOrWhiteSpace(pattern))
            continue;

          var rows = sheet.Rows.Cast<DataRow>()
                           .Where(row => row.ItemArray[mapper.Mapping["MUSTER"]].ToString() == pattern)
                           .Where(row => row.ItemArray[mapper.Mapping["EINGANG"]].ToString().Trim() == "1")
                           .ToArray();

          var articleId = articles.Count + 1;
          var article = new Article
          {
            Id = articleId,
            Name = PatternNameFix(pattern)
          };
          articles.Add(article);

          foreach (var row in rows)
          {
            // Id
            var idStr = row.ItemArray[mapper.Mapping["#"]]?.ToString();
            if (string.IsNullOrWhiteSpace(idStr))
              continue;
            var id = int.Parse(idStr);

            var pnew = new Pattern
            {
              Figure = GetDictonaryIndex(row, mapper, ref syn_figure, "FIGUR(SYN)"),
              Ground = GetDictonaryIndex(row, mapper, ref syn_ground, "GRUND(SYN)"),
              Prd = GetDictonaryIndex(row, mapper, ref syn_prd, "PRD(MUSTERSLOT)"),
              Trigger = GetDictonaryIndex(row, mapper, ref syn_trigger, "AUSLÖSER(SYN)"), 
            };

            if (patterns.ContainsKey(pnew.Key))
              pnew = patterns[pnew.Key];
            else
            {
              pnew.Id = patterns.Count + 1;
              patterns.Add(pnew.Key, pnew);
            }

            kwicFulltexts.Add(new KwicFulltext { Id = id, Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString() });

            kwics.Add(id, new[]
            {
              GetDictonaryIndex(row, mapper, ref sources, "QUELLE"),
              GetYear(row, mapper),
              GetDictonaryIndex(row, mapper, ref prdlexcore, "PRÄDIKATSKERN(LEX)", x => x.Replace("_", " ").Trim()),
              GetDictonaryIndex(row, mapper, ref prdlex, "PRÄDIKAT(LEX)", x => x.Replace("_", " ").Trim()),
              GetDictonaryIndex(row, mapper, ref diathesis, "DIATHESE", FixDiathesis),
              GetDictonaryIndex(row, mapper, ref ktypes, "KTYP"),
              GetDictonaryIndex(row, mapper, ref mtypes, "MUSTERTYP"),
              pnew.Id,
              article.Id
            });

            pnew.KwicIds.Add(id);
            pnew.ArticleIds.Add(article.Id);
            article.PatternIds.Add(pnew.Id);
          }
        }

        // Speichern
        if (!Directory.Exists("output"))
          Directory.CreateDirectory("output");

        File.WriteAllText("output/kwics.json", JsonConvert.SerializeObject(kwicFulltexts), Encoding.UTF8);
        File.WriteAllText("output/documents.json", JsonConvert.SerializeObject(kwics), Encoding.UTF8);
        File.WriteAllText("output/patterns.json", JsonConvert.SerializeObject(patterns.Values.ToArray()), Encoding.UTF8);
        File.WriteAllText("output/articles.json", JsonConvert.SerializeObject(articles), Encoding.UTF8);

        File.WriteAllText("output/meta_diathesis.json", JsonConvert.SerializeObject(diathesis), Encoding.UTF8);
        File.WriteAllText("output/meta_ktypes.json", JsonConvert.SerializeObject(ktypes), Encoding.UTF8);
        File.WriteAllText("output/meta_mtypes.json", JsonConvert.SerializeObject(mtypes), Encoding.UTF8);
        File.WriteAllText("output/meta_prdlex.json", JsonConvert.SerializeObject(prdlex), Encoding.UTF8);
        File.WriteAllText("output/meta_prdlexcore.json", JsonConvert.SerializeObject(prdlexcore), Encoding.UTF8);
        File.WriteAllText("output/meta_sources.json", JsonConvert.SerializeObject(sources), Encoding.UTF8);
        File.WriteAllText("output/meta_years.json", JsonConvert.SerializeObject(new HashSet<int>(kwics.Select(x => x.Value[1]))), Encoding.UTF8);

        File.WriteAllText("output/syn_figure.json", JsonConvert.SerializeObject(syn_figure), Encoding.UTF8);
        File.WriteAllText("output/syn_ground.json", JsonConvert.SerializeObject(syn_ground), Encoding.UTF8);
        File.WriteAllText("output/syn_prd.json", JsonConvert.SerializeObject(syn_prd), Encoding.UTF8);
        File.WriteAllText("output/syn_trigger.json", JsonConvert.SerializeObject(syn_trigger), Encoding.UTF8);
      }
    }

    private static string FixDiathesis(string diathesis)
    {
      switch (diathesis)
      {
        case "a":
          return "Aktiv";
        case "p":
          return "Passiv";
        case "k":
          return "Konvers";
        case "ambig":
          return "Ambig";
        default:
          return diathesis;
      }
    }

    private static int GetDictonaryIndex(DataRow row, ExcelColumnMapper mapper, ref Dictionary<string, int> dict, string name, Func<string, string> mod = null)
    {
      var res = -1;
      try
      {
        var str = row.ItemArray[mapper.Mapping[name]]?.ToString();
        if (mod != null)
          str = mod(str);

        if (dict.ContainsKey(str))
          res = dict[str];
        else
        {
          res = dict.Count + 1;
          dict.Add(str, res);
        }
      }
      catch
      {
        // ignore
      }

      return res;
    }

    private static int GetYear(DataRow row, ExcelColumnMapper mapper)
    {
      int year = 0;
      try
      {
        year = int.Parse(row.ItemArray[mapper.Mapping["JAHR"]].ToString());
      }
      catch
      {
        // ignore
      }

      return year;
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private static void NewMethod(DataRow row, ExcelColumnMapper mapper, HashSet<string> fsi, string name, Func<string, string> mod = null)
    {
      var val = row.ItemArray[mapper.Mapping[name]]?.ToString();
      if (!string.IsNullOrWhiteSpace(val))
        fsi.Add(mod == null ? val : mod(val));
    }


    private static string PatternNameFix(string pattern)
    {
      return pattern.Substring(0, 1).ToUpper() + pattern.Substring(1).ToLower();
    }
  }
}
