using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using HtmlAgilityPack;

namespace IDS.VAS.Excel2Xml.WebFixer
{
  public class CherryPickController
  {
    public void Process(string input, string output)
    {
      // Output wird komplett neu erzeugt.
      // Bereits erfüllt:
      // (1): Neue (unannotierte) Samples (s_): Werden hinzugefügt.
      // (2): Gelöschte Samples (s_): Werden gelöscht.

      var dict = new Dictionary<string, HtmlNode>(); // Erfüllt: (3) / (4 s. u.): Geänderte Samples (s_): Werden zwischen XML-Dokumenten ausgetauscht (inkl. Annotation)
      var trans = new Dictionary<string, List<HtmlNode>>(); // Erfüllt (5): Manuelle Samples (nicht s_): Werden immer übertragen.

      foreach (var file in GetFiles(input))
      {
        var html = new HtmlDocument();
        html.Load(file);

        foreach (var s in html.DocumentNode.SelectNodes("//sample"))
        {
          var key = s.GetAttributeValue("id", "");
          if (key == "")
            continue;

          if (key.StartsWith("s_"))
          {
            if (dict.ContainsKey(key))
              continue;
            if (s.InnerText == s.InnerHtml) // Übertrage keine unannotierten Belege
              continue;
            dict.Add(key, s);
          }
          else // Überträgt alle manuellen <sample>
          {
            if (!trans.ContainsKey(Path.GetFileName(file)))
              trans.Add(Path.GetFileName(file), new List<HtmlNode>());
            trans[Path.GetFileName(file)].Add(s);
          }
        }
      }

      var done = new HashSet<string>();

      foreach (var file in Directory.GetFiles(Path.Combine(output, "Daten"), "*.xml"))
      {
        if (file.EndsWith(_unknown))
          continue;

        var html = new HtmlDocument();
        html.Load(file);

        var selections = html.DocumentNode.SelectSingleNode("//samples");

        // Tausche Knoten aus (unannotiert (output) > annotiert (input) > output)
        for (var i = 0; i < selections.ChildNodes.Count; i++)
        {
          var key = selections.ChildNodes[i].GetAttributeValue("id", "");
          if (key == "")
            continue;
          if (!dict.ContainsKey(key))
            continue;

          selections.ChildNodes.RemoveAt(i);
          selections.ChildNodes.Insert(i, dict[key]);
          done.Add(key);
        }

        if (!trans.ContainsKey(Path.GetFileName(file)))
          continue;

        // Wenn manuelle Knoten vorhanden, füge diese an.
        foreach (var x in trans[Path.GetFileName(file)])
          selections.ChildNodes.Add(x);

        html.Save(file);
      }

      // Erfüllt (4): Überschüssige Samples (s_): Werden in einer separaten XML-Datei (UNKNWON.xml) abgelegt.
      foreach (var x in done)
        dict.Remove(x);

      if (dict.Count > 0)
        File.WriteAllText(Path.Combine(output, _unknown), $"<samples>{string.Join("", dict.Values.Select(x => x.OuterHtml))}</samples>");
    }

    private const string _unknown = "UNKNOWN.xml";

    private IEnumerable<string> GetFiles(string input)
    {
      var res = Directory.GetFiles(input, "*.xml").ToList();
      var idx = res.FindIndex(x => x.EndsWith(_unknown));
      if (idx == -1)
        return res;

      // Erfüllt (4): Überschüssige Samples (s_): ... Diese kann erneut hochgeladen werden.
      var tmp = res[idx];
      res.RemoveAt(idx);
      res.Add(tmp);
      return res;
    }
  }
}