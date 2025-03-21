using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using IDS.VAS.IndexXml2JSON.Model.Output;
using Newtonsoft.Json;

namespace IDS.VAS.IndexXml2JSON
{
  class Program
  {
    private static string _baseUrl = "/plus/map/article";

    static void Main(string[] args)
    {
      if (args.Length == 0)
        return;

      var result = new List<Group>();
      var bread = new List<BreadcrumbItem>();
      foreach (var path in args) // JOIN multi _index.xml
      {
        overview root = null;
        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
          var serializer = new XmlSerializer(typeof(overview));
          root = serializer.Deserialize(fs) as overview;
        }

        if (root == null)
          continue;

        var rGroup = new Group { Label = root.label, Id = root.id };
        RecursivAdd(ref rGroup, root.pattern, root.family);
        MakeBreadcrumb(ref bread, root);
        result.Add(rGroup);
      }

      File.WriteAllText("toc.json", JsonConvert.SerializeObject(result), Encoding.UTF8);
      File.WriteAllText("breadcrumb.json", JsonConvert.SerializeObject(bread), Encoding.UTF8);
    }

    private static void MakeBreadcrumb(ref List<BreadcrumbItem> bread, overview root)
    {
      var PRP = root.label.ToUpper();

      bread.Add(new BreadcrumbItem { prp = PRP, url = $"{_baseUrl}/{root.id}" });
      foreach (var x in root.pattern)
        bread.Add(new BreadcrumbItem { prp = PRP, art = x.label, url = $"{_baseUrl}/{root.id}/{x.id}", par = $"{_baseUrl}/{root.id}" });
      foreach (var x in root.family)
      {
        bread.Add(new BreadcrumbItem { prp = PRP, fam = x.label, url = $"{_baseUrl}/{root.id}/{x.id}", par = $"{_baseUrl}/{root.id}" });
        foreach (var y in x.Items.OfType<pattern>())
          bread.Add(new BreadcrumbItem
          {
            prp = PRP,
            fam = x.label,
            art = y.label,
            url = $"{_baseUrl}/{root.id}/{x.id}/{y.id}",
            par = $"{_baseUrl}/{root.id}/{x.id}"
          });
      }
    }

    private static void RecursivAdd(ref Group res, IEnumerable<pattern> patterns, IEnumerable<family> families)
    {
      if (patterns != null)
        foreach (var p in patterns)
          res.Entries.Add(new Entry { Label = p.label, Id = p.id });
      if (families == null)
        return;

      foreach (var f in families)
      {
        var nGroup = new Group { Label = f.label, Id = f.id };
        res.SubGroups.Add(nGroup);

        RecursivAdd(ref nGroup, f.Items.OfType<pattern>(), f.Items.OfType<family>());
      }
    }
  }
}
