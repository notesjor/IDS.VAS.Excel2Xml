using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using IDS.VAS.Excel2Json;
using IDS.VAS.Excel2Json.Model;
using Newtonsoft.Json;

namespace IDS.VAS.Xml2Json
{
  class Program
  {
    static void Main(string[] args)
    {
      if (!Directory.Exists("output"))
        Directory.CreateDirectory("output");

      var kwicsPure = new List<KwicFulltext>();
      var kwicsAnnotated = new List<KwicFulltext>();

      foreach (var path in args)
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
            kwicsPure.Add(new KwicFulltext { Id = id, CosmasId = cosmas, Text = text});
          else
            kwicsAnnotated.Add(new KwicFulltext { Id = id, CosmasId = cosmas, Text = ParseHtml(html) });
        }
      }

      File.WriteAllText("output/kwics.json", JsonConvert.SerializeObject(kwicsPure, GlobalJsonConfig.Get()), Encoding.UTF8);
      File.WriteAllText("output/kwics_annotated.json", JsonConvert.SerializeObject(kwicsAnnotated, GlobalJsonConfig.Get()), Encoding.UTF8);
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
