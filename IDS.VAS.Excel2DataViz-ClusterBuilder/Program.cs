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
      var dat = JsonConvert.DeserializeObject<CirclePackSimple[]>(File.ReadAllText("data.json", Encoding.UTF8));

      if (Directory.Exists("output"))
        Directory.Delete("output", true);
      Directory.CreateDirectory("output");

      // Hinweis: Es wird immer ToUpper verwendet, da vom Projekt für die FLARE-Daten immer Großbuchstaben verwendet werden.

      foreach (var prp in toc)
      {
        // Cluster für PRP
        var items = new List<string>();
        items.AddRange(prp.Entries.Select(x => x.Label.ToUpper()));
        items.AddRange(prp.SubGroups.SelectMany(x => x.Entries).Select(x => x.Label.ToUpper()));
        GeneratedJson(dat, prp.Id, prp.Id, items.ToArray());

        // Cluster für SubGroups
        foreach (var sub in prp.SubGroups)
        {
          GeneratedJson(dat, prp.Id, sub.Id, sub.Entries.Select(x => x.Label.ToUpper()).ToArray());
          foreach (var entry in sub.Entries)
            // Cluster für Entries in SubGroup
            GeneratedJson(dat, prp.Id,  entry.Id, new[] { entry.Label.ToUpper() });
        }

        // Cluster für Entries in PRP
        foreach (var entry in prp.Entries)
          GeneratedJson(dat, prp.Id, entry.Id, new[] { entry.Label.ToUpper() });

      }
    }

    private static void GeneratedJson(CirclePackSimple[] data, string prp, string id, string[] items)
    {
      if (!Directory.Exists($"output/{prp}"))
        Directory.CreateDirectory($"output/{prp}");
      File.WriteAllText($"output/{prp}/{id}.json", JsonConvert.SerializeObject(data.Where(x => items.Contains(x.Name.ToUpper())).ToArray(), Formatting.Indented), Encoding.UTF8);
    }
  }
}
