using HtmlAgilityPack;
using IDS.VAS.Excel2Xml.XRefValidator.Properties;
using System.Text;

namespace IDS.VAS.Excel2Xml.XRefValidator
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var file = args[0];

      var doc = new HtmlAgilityPack.HtmlDocument();
      Console.WriteLine("START VALIDATION: " + file);
      doc.Load(file);

      var samples = doc.DocumentNode.SelectNodes("//sample");
      SearchUnnannotatedSamples(file, doc, samples);
      GenerateAnnotatedHtml(file, samples);

      Console.WriteLine("END VALIDATION: " + file);
      Console.ReadLine();
    }

    private static void GenerateAnnotatedHtml(string file, HtmlNodeCollection samples)
    {
      var output = file + ".html";
      if (File.Exists(output))
        File.Delete(output);

      var stb2 = new StringBuilder();
      foreach(HtmlNode sample in samples)
      {
        var stb1 = new StringBuilder();
        foreach(HtmlNode child in sample.ChildNodes)
        {
          if(child.Name == "#text")
            stb1.AppendLine(child.InnerText);
          else
            stb1.AppendLine(Resources.Template_SPAN.Replace("{{CLASS}}", child.Name).Replace("{{TXT}}", child.InnerText));
        }
        stb2.AppendLine(Resources.Template_SAMPLE.Replace("{{ID}}", sample.GetAttributeValue("id", "")).Replace("{{CONTENT}}", stb1.ToString()));
      }

      File.WriteAllText(output, Resources.Template_HTML.Replace("{{CSS}}", Resources.Template_CSS).Replace("{{SAMPLES}}", stb2.ToString()));
    }

    private static void SearchUnnannotatedSamples(string file, HtmlDocument doc, HtmlNodeCollection samples)
    {
      var unannotated = new HashSet<string>();

      foreach (var x in samples)
        if (x.ChildNodes.All(c => c.Name == "#text"))
        {
          var id = x.GetAttributeValue("id", "");
          if (id == "")
          {
            Console.WriteLine("ERROR: sample without or empty  id !!!");
            continue;
          }
          unannotated.Add(id);
        }

      var xrefs = doc.DocumentNode.SelectNodes("//xref");
      var todo = new HashSet<string>();

      foreach (var x in xrefs)
      {
        var id = x.GetAttributeValue("href", "");
        if (id == "")
        {
          Console.WriteLine("ERROR: xref without or empty href !!!");
          continue;
        }
        if (unannotated.Contains(id))
          todo.Add(id);
      }

      var output = file + ".txt";
      if (File.Exists(output))
        File.Delete(output);

      if (todo.Count > 0)
      {
        Console.WriteLine("UNANNOTATED SAMPLES: " + todo.Count);
        File.AppendAllLines(output, todo.OrderBy(x => x).ToArray());
      }
      else
      {
        Console.WriteLine("ALL SAMPLES ARE ANNOTATED");
      }
    }
  }
}
