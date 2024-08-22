using IDS.VAS.Excel2DataViz.ClusterBuilder.Model.Flare;
using IDS.VAS.Excel2DataViz.ClusterBuilder.Model.Toc;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.VAS.Excel2DataViz_ClusterBuilder
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var toc = JsonConvert.DeserializeObject<TocData[]>(File.ReadAllText("toc.json", Encoding.UTF8));
      var dat = JsonConvert.DeserializeObject<FlareData>(File.ReadAllText("data.json", Encoding.UTF8));

      if (Directory.Exists("output"))
        Directory.Delete("output", true);
      Directory.CreateDirectory("output");

      // Hinweis: Es wird immer ToUpper verwendet, da vom Projekt für die FLARE-Daten immer Großbuchstaben verwendet werden.

      foreach (var prp in toc)
      {
        // Cluster für PRP
        var items = new List<string>();
        items.AddRange(prp.Entries.Select(x => x.Id.ToUpper()));
        items.AddRange(prp.SubGroups.SelectMany(x => x.Entries).Select(x => x.Id.ToUpper()));
        GeneratedJson(dat, prp.Id, items);

        // Cluster für SubGroups
        foreach (var sub in prp.SubGroups)
        {
          GeneratedJson(dat, sub.Id, sub.Entries.Select(x => x.Id.ToUpper()));
          foreach (var entry in sub.Entries)
            GeneratedJson(dat, entry.Id, new[] { entry.Id.ToUpper() });
        }

        // Cluster für Entries
        foreach (var entry in prp.Entries)
          GeneratedJson(dat, entry.Id, new[] { entry.Id.ToUpper() });

      }
    }

    private static void GeneratedJson(FlareData data, string id, IEnumerable<string> items)
    {
      var res = new FlareData{ Name = "flare", Children = data.Children.Where(x => items.Contains(x.Name.ToUpper())).ToArray() };
      File.WriteAllText($"output/{id}.json", JsonConvert.SerializeObject(res, Formatting.Indented), Encoding.UTF8);
    }
  }
}
