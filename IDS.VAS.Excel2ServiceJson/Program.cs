using HtmlAgilityPack;
using IDS.Vas.ExcelReader;
using Meilisearch;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace IDS.VAS.Excel2ServiceJson
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var table = VasExcelReader.ReadExcel(args[0]).Tables[0];
      var annotations = LoadAnnotations(args[0]);

      var header = LoadHeader(table);

      // Für die folgenden Daten wird je eine Liste in output.json erstellt
      var unique = LoadUniqueList();
      var tokenizer = LoadTokenized();
      foreach (var t in tokenizer)
        unique.Add(t, new HashSet<string>());
      var cleaner = LoadCleaner();
      foreach (var x in cleaner)
        unique.Add(x, new HashSet<string>());

      var hierarchy = LoadHierarchy(); // Notwendig zum Erstellen von Hierarchien      

      foreach (var item in header)
        Console.WriteLine(item);

      Console.Write("Headers ok (y/n)? ");
      if (Console.ReadLine().ToLower() != "y")
        return;

      var eingang_idx = header["EINGANG"];

      var res = new List<Dictionary<string, object>>();

      foreach (DataRow row in table.Rows)
      {
        if (row.ItemArray[eingang_idx].ToString().Trim() == "0")
          continue;

        // Erstelle Sucheinträge
        var r = new Dictionary<string, object>();
        foreach (var item in header)
        {
          var cell = row.ItemArray[item.Value].ToString().Trim();

          // NULL-Bereinigung
          if (string.IsNullOrEmpty(cell))
            cell = "–";

          // CLEANER
          if (cleaner.Contains(item.Key))
            cell = cell.Replace("_", " ");

          // TOKENIZER
          if (tokenizer.Contains(item.Key))
          {
            var tokens = cell.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
            r.Add(item.Key, tokens);
            foreach (var token in tokens)
              unique[item.Key].Add(token);
            continue;
          }
          else
            r.Add(item.Key, cell);

          if (unique.ContainsKey(item.Key))
            unique[item.Key].Add(cell);
        }
        res.Add(r);

        // Hierarchy
        foreach (var h in hierarchy)
        {
          var fields = h.Key;
          var f1 = r[fields.Item1].ToString();
          var f2 = r[fields.Item2].ToString();

          if (h.Value.ContainsKey(f1))
            h.Value[f1].Add(f2);
          else
            h.Value.Add(f1, new HashSet<string>() { f2 });
        }
      }

      // setze alle Belege standardmäßig auf "nicht annotiert"
      foreach (var r in res)
        r.Add("~", "f");

      // Ersetze annotierte Belege
      foreach (var r in res)
      {
        var id = int.Parse(r["#"].ToString());
        if (annotations.ContainsKey(id))
        {
          r["BELEG"] = annotations[id];
          r["~"] = "t"; // annotiert
        }
      }

      File.WriteAllText("output.json", JsonConvert.SerializeObject(res, Formatting.Indented), Encoding.UTF8);

      MeilisearchClient client = new MeilisearchClient("http://lexik08.ids-mannheim.de:7700/", "8jRAqq_GbtjdjveIOCxIlnztXjwFbcaMYp-e50HtbrQ");
      try
      {
        client.DeleteIndexAsync("map").Wait();
      }
      catch { }

      var index = client.Index("map");
      index.UpdateFilterableAttributesAsync(unique.Keys.ToArray()).Wait();

      index.AddDocumentsJsonAsync(JsonConvert.SerializeObject(res), "#").Wait();
      File.WriteAllText("output.json", JsonConvert.SerializeObject(unique), Encoding.UTF8);
      File.WriteAllText("hierarchy.json", JsonConvert.SerializeObject(hierarchy.ToDictionary(x => $"{x.Key.Item1}_{x.Key.Item2}", x => x.Value)), Encoding.UTF8);
    }

    private static Dictionary<int, string> LoadAnnotations(string path)
    {
      var res = new Dictionary<int, string>();

      var files = Directory.GetFiles(Path.GetDirectoryName(path), "*.xml", SearchOption.AllDirectories);
      foreach (var file in files)
      {
        var doc = new HtmlDocument();
        using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
          doc.Load(fs);

        try
        {
          foreach (var n in doc.DocumentNode.SelectNodes("//sample"))
          {
            var idStr = n.GetAttributeValue("id", "");
            if (string.IsNullOrEmpty(idStr))
              continue;

            if (n.ChildNodes.Count == 1 && n.ChildNodes.First().Name == "#text")
              continue;

            try
            {
              var id = int.Parse(idStr.Substring(2));
              var html = ParseHtml(n.InnerHtml.Trim());

              if (res.ContainsKey(id))
                res[id] = html;
              else
                res.Add(id, html);
            }
            catch
            {
              Console.WriteLine($"Error in {file} - {idStr}");
            }
          }
        }
        catch
        {
          // ignore
        }
      }

      return res;
    }

    private static string[] _slots = new[] { "rel", "val", "vrb", "prp", "effector", "figure", "ground" };

    private static string ParseHtml(string html)
    {
      // Start-Tags
      html = _slots.Aggregate(html, (current, slot) => current.Replace($"<{slot}>", $"<span class=\"{slot}\">"));
      // End-Tags
      html = _slots.Aggregate(html, (current, slot) => current.Replace($"</{slot}>", "</span>"));

      return $"<div class=\"sample-txt\">{html}</div>";
    }

    private static HashSet<string> LoadTokenized()
    {
      return new HashSet<string>(File.ReadAllLines("VALUE_TOKENIZE.txt", Encoding.UTF8).Select(x => x.Trim()));
    }

    private static HashSet<string> LoadCleaner()
    {
      return new HashSet<string>(File.ReadAllLines("VALUE_CLEAN.txt", Encoding.UTF8).Select(x => x.Trim()));
    }

    private static Dictionary<Tuple<string, string>, Dictionary<string, HashSet<string>>> LoadHierarchy()
    {
      var cols = File.ReadAllLines("HIERARCHY.txt", Encoding.UTF8);
      var res = new Dictionary<Tuple<string, string>, Dictionary<string, HashSet<string>>>();

      foreach (var col in cols)
      {
        var split = col.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        if (split.Length != 2)
          continue;

        res.Add(new Tuple<string, string>(split[0], split[1]), new Dictionary<string, HashSet<string>>());
      }

      return res;
    }

    private static Dictionary<string, HashSet<string>> LoadUniqueList()
    {
      var cols = File.ReadAllLines("VALUE.txt", Encoding.UTF8).Select(x => x.Trim()).ToArray();
      var res = new Dictionary<string, HashSet<string>>();
      foreach (var col in cols)
        res.Add(col, new HashSet<string>());

      // Ist der Beleg annotiert?
      res.Add("~", new HashSet<string> { "t", "f" });

      return res;
    }

    private static Dictionary<string, int> LoadHeader(DataTable table)
    {
      var ignore = new HashSet<string>(File.ReadAllLines("IGNORE.txt", Encoding.UTF8).Select(x => x.Trim()));

      var res = new Dictionary<string, int>();
      for (var i = 0; i < table.Columns.Count; i++)
      {
        var c = table.Columns[i];
        var key = c.ColumnName.ToUpper().Replace(" ", "");
        if (ignore.Contains(key))
          continue;
        if (key.StartsWith("KOMM_"))
          continue;
        if (res.ContainsKey(key))
          continue;
        res.Add(key, i);
      }

      return res;
    }
  }
}
