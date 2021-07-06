using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using IDS.VAS.Excel2Json;
using IDS.VAS.Excel2Json.Model;
using IDS.VAS.Xml2Json.Model;
using Newtonsoft.Json;

namespace IDS.VAS.Xml2Json
{
  class Program
  {
    static void Main(string[] args)
    {
      if (!Directory.Exists("output"))
        Directory.CreateDirectory("output");

      var kwicsPure = new Dictionary<int, KwicFulltext>();
      var kwicsAnnotated = new Dictionary<int, KwicFulltext>();
      var articleInfos = new Dictionary<string, string>();
      var indices = new List<SimpleItem>();

      foreach (var path in args)
      {
        if (Path.GetFileName(path) == "_index.xml")
          ConvertIndex(path, ref indices);
        else
          ConvertArticle(path, ref kwicsPure, ref kwicsAnnotated, ref articleInfos);
      }

      File.WriteAllText("output/kwics.json", JsonConvert.SerializeObject(kwicsPure, GlobalJsonConfig.Get()), Encoding.UTF8);
      File.WriteAllText("output/kwics_annotated.json", JsonConvert.SerializeObject(kwicsAnnotated, GlobalJsonConfig.Get()), Encoding.UTF8);
      File.WriteAllText("output/article_info.json", JsonConvert.SerializeObject(articleInfos, GlobalJsonConfig.Get()), Encoding.UTF8);
      File.WriteAllText("output/indices.json", JsonConvert.SerializeObject(indices, GlobalJsonConfig.Get()), Encoding.UTF8);
    }

    private static void ConvertIndex(string path, ref List<SimpleItem> indices)
    {
      var doc = new HtmlDocument();
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        doc.Load(fs);

      foreach (var pNode in doc.DocumentNode.SelectNodes("//overview"))
      {
        var pItem = new SimpleItem { Name = pNode.GetAttributeValue("label", ""), Type = "PRD" };
        ConvertIndexRecursive(pNode, ref pItem);
        pItem.Finalize();
        indices.Add(pItem);
      }
    }

    private static void ConvertIndexRecursive(HtmlNode pNode, ref SimpleItem pItem)
    {
      foreach (var cNode in pNode.ChildNodes)
      {
        if (cNode.Name == "#text")
          continue;

        var cItem = new SimpleItem { Name = cNode.GetAttributeValue("label", ""), Type = cNode.Name };
        pItem.Children.Add(cItem);

        ConvertIndexRecursive(cNode, ref cItem);
      }
    }

    private static void ConvertArticle(string path,
                                       ref Dictionary<int, KwicFulltext> kwicsPure,
                                       ref Dictionary<int, KwicFulltext> kwicsAnnotated,
                                       ref Dictionary<string, string> articleInfos)
    {
      var doc = new HtmlDocument();
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        doc.Load(fs);

      var samples = doc.DocumentNode.SelectNodes("//sample");

      foreach (var sample in samples)
      {
        var id = int.Parse(sample.GetAttributeValue("id", "").Replace("s_", ""));
        var cosmas = sample.GetAttributeValue("cosmas", null);

        var html = sample.InnerHtml;
        var text = sample.InnerText;

        if (html.Length == text.Length)
          kwicsPure.Add(id, new KwicFulltext { CosmasId = cosmas, Text = text });
        else
          kwicsAnnotated.Add(id, new KwicFulltext { CosmasId = cosmas, Text = ParseHtml(html) });
      }

      var info = string.Join(" ", doc.DocumentNode.SelectNodes("//prototype/p").Select(x => x.InnerHtml));
      articleInfos.Add(Path.GetFileNameWithoutExtension(path),
                       ParseHtml(info.Replace("\r", " ").Replace("\n", " ").Replace("\t", "").Replace("  ", " ")
                                     .Replace("  ", " ").Replace("  ", " ").Trim()));
    }

    private static string[] _slots = new[] { "prd", "effector", "figure", "ground" };

    private static string ParseHtml(string html)
    {
      // Start-Tags
      html = _slots.Aggregate(html, (current, slot) => current.Replace($"<{slot}>", $"<span class=\"{slot}\">"));
      // End-Tags
      return _slots.Aggregate(html, (current, slot) => current.Replace($"</{slot}>", "</span>"));
    }
  }
}
