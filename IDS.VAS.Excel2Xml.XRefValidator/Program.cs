namespace IDS.VAS.Excel2Xml.XRefValidator
{
  internal class Program
  {
    static void Main(string[] args)
    {
      var doc = new HtmlAgilityPack.HtmlDocument();
      Console.WriteLine("START VALIDATION: " + args[0]);
      doc.Load(args[0]);

      var samples = doc.DocumentNode.SelectNodes("//sample");
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

      var output = args[0] + ".txt";
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
