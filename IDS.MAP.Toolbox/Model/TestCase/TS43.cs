using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Helper;
using System.Runtime.ExceptionServices;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS43 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var _error = new List<string>();
      var files = config.WorkXmlFiles;
      foreach (var file in files)
      {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(file);
        var samples = doc.DocumentNode.SelectNodes("//sample");
        if (samples == null && file.Contains("artikel"))
          _error.Add($"Die Datei {Path.GetFileName(file)} enthält keine <sample>-Einträge.\n");
        if (samples != null)
          SearchUnnannotatedSamples(ref _error, file, doc, samples);
      }

      Valid = _error.Count == 0;
      DetailErrorReport = _error.BuildErrorMessage("Folgende Fehler treten im Zusammenhang mit sample/xref auf.\n");
    }

    public override bool BreakExecution { get; } = false;

    private static void SearchUnnannotatedSamples(ref List<string> errors, string file, HtmlDocument doc, HtmlNodeCollection samples)
    {
      var unannotated = new HashSet<string>();
      var first = true;

      foreach (var x in samples)
        if (x.ChildNodes.All(c => c.Name == "#text"))
        {
          var id = x.GetAttributeValue("id", "");
          if (id == "")
          {
            Report(ref first, ref errors, file, "enthält <sample>-Einträge, ohne id.");
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
          Report(ref first, ref errors, file,"enthält <xref>-Einträge, ohne href.");
          continue;
        }
        if (unannotated.Contains(id))
          todo.Add(id);
      }

      if (todo.Count <= 0) 
        return;

      Report(ref first, ref errors, file,$"zitiert {todo.Count} Belege, die nicht annotiert sind:");
      errors.AddRange(todo);
    }

    private static void Report(ref bool first, ref List<string> errors, string file, string error)
    {
      if (first)
      {
        errors.Add($"\n{Path.GetFileName(file)}:");
        first = false;
      }
      errors.Add(error);
    }
  }
}