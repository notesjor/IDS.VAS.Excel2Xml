using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
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
    private static string _baseDir;

    static void Main(string[] args)
    {
      if (args.Length == 0)
        return;

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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

        var kwicFulltexts = new Dictionary<int, KwicFulltext>();
        var kwics = new Dictionary<int, int[]>();
        var patterns = new Dictionary<string, Pattern>();
        var articles = new List<Article>();

        var diathesis = new Dictionary<string, int>();
        var ktypes = new Dictionary<string, int>();
        var mtypes = new Dictionary<string, int>();
        var sources = new Dictionary<string, int>();


        var prd_syn = new Dictionary<string, int>();
        var prd_ele = new Dictionary<string, int>();
        var prd_lex = new Dictionary<string, int>();
        var prd_hir = new List<HItem>();
        var prd_hid = new Dictionary<string, int>();

        var trigger_syn = new Dictionary<string, int>();
        var trigger_ele = new Dictionary<string, int>();
        var trigger_hir = new List<HItem>();
        var trigger_hid = new Dictionary<string, int>();

        var figure_syn = new Dictionary<string, int>();
        var figure_ele = new Dictionary<string, int>();
        var figure_hir = new List<HItem>();
        var figure_hid = new Dictionary<string, int>();

        var ground_syn = new Dictionary<string, int>();
        var ground_ele = new Dictionary<string, int>();
        var ground_hir = new List<HItem>();
        var ground_hid = new Dictionary<string, int>();

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
              Figure = GetRowValueIndexed(row, mapper, ref figure_syn, "FIGUR(SYN)"),
              Ground = GetRowValueIndexed(row, mapper, ref ground_syn, "GRUND(KOPF)"),
              Prd = GetRowValueIndexed(row, mapper, ref prd_syn, "PRD(SYN)"),
              Trigger = GetRowValueIndexed(row, mapper, ref trigger_syn, "AUSLÖSER(SYN)"),

              ElementsFigure = GetDictonaryTokenizedIndex(row, mapper, ref figure_ele, "FIGUR:ELEMENTE"),
              ElementsGround = GetDictonaryTokenizedIndex(row, mapper, ref ground_ele, "GRUND(KASUS)"),
              ElementsPrd = GetDictonaryTokenizedIndex(row, mapper, ref prd_ele, "PG:ELEMENTE"),
              ElementsTrigger = GetDictonaryTokenizedIndex(row, mapper, ref trigger_ele, "AUSLÖSER:ELEMENTE"),

              DisplayFigure = GetRowValue(row, mapper, "FIGUR(SYN)"),
              DisplayGround = GetRowValue(row, mapper, "GRUND(KASUS)"),//GetRowValue(row, mapper, "GRUND(KOPF)") + "+" + GetRowValue(row, mapper, "GRUND(KASUS)"),
              DisplayPrd = GetRowValue(row, mapper, "PRD(SYN)"),
              DisplayTrigger = GetRowValue(row, mapper, "AUSLÖSER(SYN)"),
            };

            if (patterns.ContainsKey(pnew.Key))
              pnew = patterns[pnew.Key];
            else
            {
              pnew.Id = patterns.Count + 1;
              patterns.Add(pnew.Key, pnew);
            }

            kwicFulltexts.Add(id, new KwicFulltext { Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString() });

            // Hinweis: Wird einmal als Index aber auch von GeneratePatternArticleKwicDictionary verwendet
            // WARNUNG: Indices müssen ggf. in GeneratePatternArticleKwicDictionary angepasst werden
            kwics.Add(id, new[]
            {
              GetDictonaryIndex(row, mapper, ref sources, "QUELLE"), // 0
              GetYear(row, mapper), // 1
              GetDictonaryIndex(row, mapper, ref prd_lex, "LEXIKALISCHERPRÄDIKATSKERN", x => x.Replace("_", " ").Trim()), // 2
              GetDictonaryIndex(row, mapper, ref diathesis, "DIATHESE", FixDiathesis), // 3
              GetDictonaryIndex(row, mapper, ref ktypes, "KONSTRUKTIONSTYP"), // 4
              GetDictonaryIndex(row, mapper, ref mtypes, "MUSTERTYP"), // 5
              pnew.Id, // 6
              article.Id // 7
            });

            pnew.KwicIds.Add(id);
            pnew.ArticleIds.Add(article.Id);
            article.PatternIds.Add(pnew.Id);

            // Hierachie aufbauen
            var syn = GetRowValue(row, mapper, "PRD(SYN)");
            var ele = GetRowValue(row, mapper, "PG:ELEMENTE");
            // FIX leere ele
            if (string.IsNullOrWhiteSpace(ele))
              ele = syn;
            var lex = GetRowValue(row, mapper, "LEXIKALISCHERPRÄDIKATSKERN").Replace("_", " ").Trim();
            AddHierarchy(ref prd_hir,
                         GetDictonaryIndex($"{syn}", ref prd_hid),
                         syn,
                         GetDictonaryIndex($"{syn}_{ele}", ref prd_hid),
                         ele,
                         GetDictonaryIndex($"{syn}_{ele}_{lex}", ref prd_hid),
                         lex,
                         id);
            syn = GetRowValue(row, mapper, "AUSLÖSER(SYN)");
            ele = GetRowValue(row, mapper, "AUSLÖSER:ELEMENTE");
            AddHierarchy(ref trigger_hir,
                         GetDictonaryIndex($"{syn}", ref trigger_hid),
                         syn,
                         GetDictonaryIndex($"{syn}_{ele}", ref trigger_hid),
                         ele,
                         id);
            syn = GetRowValue(row, mapper, "FIGUR(SYN)");
            ele = GetRowValue(row, mapper, "FIGUR:ELEMENTE");
            AddHierarchy(ref figure_hir,
                         GetDictonaryIndex($"{syn}", ref figure_hid),
                         syn,
                         GetDictonaryIndex($"{syn}_{ele}", ref figure_hid),
                         ele,
                         id);
            syn = GetRowValue(row, mapper, "GRUND(KOPF)");
            ele = GetRowValue(row, mapper, "GRUND(KASUS)");
            AddHierarchy(ref ground_hir,
                         GetDictonaryIndex($"{syn}", ref ground_hid),
                         syn,
                         GetDictonaryIndex($"{syn}_{ele}", ref ground_hid),
                         ele,
                         id);
          }
        }

        // Bereinigen
        CleanHierarchy(ref prd_hir);
        CleanHierarchy(ref trigger_hir);
        CleanHierarchy(ref figure_hir);
        CleanHierarchy(ref ground_hir);

        // Speichern
        if (!Directory.Exists("output"))
          Directory.CreateDirectory("output");

        File.WriteAllText("output/kwics.json", JsonConvert.SerializeObject(kwicFulltexts, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/documents.json", JsonConvert.SerializeObject(kwics, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/patterns.json", JsonConvert.SerializeObject(patterns.Values.ToArray(), GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/articles.json", JsonConvert.SerializeObject(articles, GlobalJsonConfig.Get()), Encoding.UTF8);

        File.WriteAllText("output/meta_diathesis.json", JsonConvert.SerializeObject(diathesis, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/meta_ktype.json", JsonConvert.SerializeObject(ktypes, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/meta_mtype.json", JsonConvert.SerializeObject(mtypes, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/meta_prdlex.json", JsonConvert.SerializeObject(prd_lex, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/meta_sources.json", JsonConvert.SerializeObject(sources, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/meta_years.json", JsonConvert.SerializeObject(new HashSet<int>(kwics.Select(x => x.Value[1])), GlobalJsonConfig.Get()), Encoding.UTF8);

        File.WriteAllText("output/syn_figure.json", JsonConvert.SerializeObject(figure_syn, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/syn_ground.json", JsonConvert.SerializeObject(ground_syn, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/syn_prd.json", JsonConvert.SerializeObject(prd_syn, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/syn_trigger.json", JsonConvert.SerializeObject(trigger_syn, GlobalJsonConfig.Get()), Encoding.UTF8);

        File.WriteAllText("output/elements_figure.json", JsonConvert.SerializeObject(figure_ele, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/elements_ground.json", JsonConvert.SerializeObject(ground_ele, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/elements_prd.json", JsonConvert.SerializeObject(prd_ele, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/elements_trigger.json", JsonConvert.SerializeObject(trigger_ele, GlobalJsonConfig.Get()), Encoding.UTF8);

        File.WriteAllText("output/hierarchy_figure.json", JsonConvert.SerializeObject(figure_hir, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/hierarchy_ground.json", JsonConvert.SerializeObject(ground_hir, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/hierarchy_prd.json", JsonConvert.SerializeObject(prd_hir, GlobalJsonConfig.Get()), Encoding.UTF8);
        File.WriteAllText("output/hierarchy_trigger.json", JsonConvert.SerializeObject(trigger_hir, GlobalJsonConfig.Get()), Encoding.UTF8);

        // Key-Liste
        var pak = GeneratePatternArticleKwicDictionary(kwics);
        File.WriteAllLines("output/patterns.txt", GetPatternKeyList(articles, patterns, pak));
      }
    }

    private static void CleanHierarchy(ref List<HItem> output)
    {
      // FIX sortiere alphabetisch
      output = output.OrderBy(x => x.Name).ToList();

      // FIX Lösche leere Kinder
      foreach (var h in output)
      {
        if (h.Children.Count == 0)
          h.Children = null;
        else
        {
          var children = h.Children;
          CleanHierarchy(ref children);
          h.Children = children;
        }
      }

      // FIX - Überprüfen ob 1 Kind und Kind = Parent
      foreach (var h in output)
      {
        if (h.Children is { Count: 1 } && h.Children[0].Name == h.Name) h.Children = h.Children[0].Children;
      }
    }

    private static void AddHierarchy(ref List<HItem> output, int i1, string v1, int i2, string v2, int i3, string v3, int id)
    {
      var l1 = HierarchySearch(ref output, v1);

      // FIX für leere Werte
      if (string.IsNullOrWhiteSpace(v2))
      {
        i2 = i1;
        v2 = v1;
      }
      if (string.IsNullOrWhiteSpace(v3))
      {
        i3 = i2;
        v3 = v2;
      }
      // FIX ENDE

      if (l1 == null)
      {
        output.Add(new HItem
        {
          Id = i1,
          Name = v1,
          Children = new List<HItem>
          {
            new()
            {
              Id = i2,
              Name = v2,
              Children = new List<HItem>
              {
                new HItem
                {
                  Id = i3,
                  Name = v3,
                  Docs = new HashSet<int>{id}
                }
              }
            }
          }
        });
      }
      else
      {
        var l2 = HierarchySearch(ref l1.Children, v2);
        if (l2 == null)
        {
          l1.Children.Add(new HItem
          {
            Id = i2,
            Name = v2,
            Children = new List<HItem>
            {
              new HItem
              {
                Id = i3,
                Name = v3,
                Docs = new HashSet<int>{id}
              }
            }
          });
        }
        else
        {
          var l3 = HierarchySearch(ref l2.Children, v3);
          if (l3 == null)
          {
            l2.Children.Add(new HItem
            {
              Id = i3,
              Name = v3,
              Docs = new HashSet<int> { id }
            });
          }
          else
            l3.Docs.Add(id);
        }
      }
    }

    private static void AddHierarchy(ref List<HItem> output, int i1, string v1, int i2, string v2, int id)
    {
      var l1 = HierarchySearch(ref output, v1);

      // FIX für leere Werte
      if (string.IsNullOrWhiteSpace(v2))
      {
        i2 = i1;
        v2 = v1;
      }
      // FIX ENDE

      if (l1 == null)
      {
        output.Add(new HItem
        {
          Id = i1,
          Name = v1,
          Children = new List<HItem>
          {
            new()
            {
              Id = i2,
              Name = v2,
              Docs = new HashSet<int>{id}
            }
          }
        });
      }
      else
      {
        var l2 = HierarchySearch(ref l1.Children, v2);
        if (l2 == null)
        {
          l1.Children.Add(new HItem
          {
            Id = i2,
            Name = v2,
            Docs = new HashSet<int> { id }
          });
        }
        else
          l2.Docs.Add(id);
      }
    }

    private static HItem HierarchySearch(ref List<HItem> list, string v)
      => (from x in list where x.Name == v select x).FirstOrDefault();

    private static Dictionary<int, Dictionary<int, HashSet<int>>> GeneratePatternArticleKwicDictionary(Dictionary<int, int[]> kwics)
    {
      var res = new Dictionary<int, Dictionary<int, HashSet<int>>>();
      foreach (var x in kwics)
      {
        var patternId = x.Value[6];
        var articleId = x.Value[7];
        var kwicId = x.Key;

        if (res.ContainsKey(patternId))
        {
          if (res[patternId].ContainsKey(articleId))
            res[patternId][articleId].Add(kwicId);
          else
            res[patternId].Add(articleId, new HashSet<int> { kwicId });
        }
        else
          res.Add(patternId, new Dictionary<int, HashSet<int>> { { articleId, new HashSet<int> { kwicId } } });
      }

      return res;
    }

    private static IEnumerable<string> GetPatternKeyList(List<Article> articles,
                                                         Dictionary<string, Pattern> patterns,
                                                         Dictionary<int, Dictionary<int, HashSet<int>>> pak)
    {
      var res = new List<string>();

      var aDic = articles.ToDictionary(x => x.Id, x => x.Name);
      res.Add($"PATTERN\t{string.Join("\t", aDic.Values)}");
      var aKeys = aDic.Keys.ToArray();

      var pDic = patterns.ToDictionary(x => x.Value.Id, x => x.Value.Key);
      res.AddRange(pDic.Select(p => $"{p.Value}\t{GetPatternKeyListEntry(p.Key, aKeys, ref pak)}"));

      return res;
    }

    private static string GetPatternKeyListEntry(int p, int[] aKeys, ref Dictionary<int, Dictionary<int, HashSet<int>>> pak)
    {
      var res = new string[aKeys.Length];
      for (var i = 0; i < res.Length; i++)
        res[i] = "";

      if (!pak.ContainsKey(p))
        return string.Join("\t", res);

      for (var i = 0; i < aKeys.Length; i++)
        if (pak[p].ContainsKey(aKeys[i]))
          res[i] = string.Join(", ", pak[p][aKeys[i]]);

      return string.Join("\t", res);
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

    private static string GetRowValue(DataRow row, ExcelColumnMapper mapper, string name, Func<string, string> mod = null)
    {
      try
      {
        var txt = row.ItemArray[mapper.Mapping[name]]?.ToString();
        if (mod != null)
          txt = mod(txt);
        return txt;
      }
      catch
      {
        return null;
      }
    }

    private static IEnumerable<int> GetDictonaryTokenizedIndex(DataRow row, ExcelColumnMapper mapper, ref Dictionary<string, int> dict, string name)
    {
      try
      {
        // Sorgt dafür, dass nur einmalige Werte aufgenommen werden: V akk akk akk wird zu: V, akk
        var res = new HashSet<int>();

        var txt = row.ItemArray[mapper.Mapping[name]]?.ToString();
        txt = txt.Replace("(", "").Replace(")", "").Replace("_", " ").ToUpper();
        var tokens = txt.Split(new[] { " ", "," }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var str in tokens)
        {
          if (!dict.ContainsKey(str))
            dict.Add(str, dict.Count + 1);
          res.Add(dict[str]);
        }

        // Ordnung ist das halbe Leben - kann später für Abbruchbedingung verwendet werden.
        return res.OrderBy(x => x);
      }
      catch
      {
        return null;
      }
    }

    private static int GetRowValueIndexed(DataRow row, ExcelColumnMapper mapper, ref Dictionary<string, int> dict, string name)
    {
      try
      {
        var txt = row.ItemArray[mapper.Mapping[name]]?.ToString();
        txt = txt.Replace("(", "").Replace(")", "").Replace("_", " ").ToUpper();

        if (!dict.ContainsKey(txt))
          dict.Add(txt, dict.Count + 1);
        return dict[txt];
      }
      catch
      {
        return -1;
      }
    }

    private static int GetDictonaryIndex(DataRow row, ExcelColumnMapper mapper, ref Dictionary<string, int> dict, string name, Func<string, string> mod = null)
    {
      var res = -1;
      try
      {
        var str = row.ItemArray[mapper.Mapping[name]]?.ToString();
        res = GetDictonaryIndex(str, ref dict, mod);
      }
      catch
      {
        // ignore
      }

      return res;
    }

    private static int GetDictonaryIndex(string str, ref Dictionary<string, int> dict, Func<string, string> mod = null)
    {
      var res = -1;
      try
      {
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

    private static string PatternNameFix(string pattern)
    {
      return pattern.Substring(0, 1).ToUpper() + pattern.Substring(1).ToLower();
    }
  }
}
