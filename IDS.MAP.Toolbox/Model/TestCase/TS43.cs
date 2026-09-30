using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS43 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      var issues = new List<ValidationIssue>();
      var files = config.WorkXmlFiles;
      foreach (var file in files)
      {
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.Load(file);
        var samples = doc.DocumentNode.SelectNodes("//sample");
        if (samples == null && file.Contains("artikel"))
        {
          var message = "enthält keine <sample>-Einträge.";
          errors.Add($"Die Datei {Path.GetFileName(file)} {message}\n");
          issues.Add(new ValidationIssue
          {
            FileName = Path.GetFileName(file),
            Line = 1,
            UserMessage = message
          });
        }

        if (samples != null)
          SearchUnnannotatedSamples(ref errors, ref issues, file, doc, samples);
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.BuildErrorMessage("Folgende Fehler treten im Zusammenhang mit sample/xref auf.\n");
      DetailIssues = issues;
    }

    public override bool BreakExecution { get; } = false;

    private static void SearchUnnannotatedSamples(ref List<string> errors, ref List<ValidationIssue> issues, string file, HtmlDocument doc, HtmlNodeCollection samples)
    {
      var unannotated = new HashSet<string>();
      var first = true;

      foreach (var x in samples)
        if (x.ChildNodes.All(c => c.Name == "#text"))
        {
          var id = x.GetAttributeValue("id", "");
          if (id == "")
            Report(ref first, ref errors, ref issues, file, x.Line, "enthält <sample>-Einträge, ohne id.");

          unannotated.Add(id);
        }

      var xrefs = doc.DocumentNode.SelectNodes("//xref");
      var todo = new Dictionary<int, string>();

      if (xrefs != null)
        foreach (var x in xrefs)
        {
          var id = x.GetAttributeValue("href", "");
          if (id == "")
          {
            Report(ref first, ref errors, ref issues, file, x.Line, "enthält <xref>-Einträge, ohne href.");
            continue;
          }
          if (unannotated.Contains(id))
            todo[x.Line] = id;
        }

      if (todo.Count <= 0)
        return;

      errors.Add($"zitiert {todo.Count} Belege, die nicht annotiert sind:");
      foreach (var x in todo)
        Report(ref first, ref errors, ref issues, file, x.Key, $"xref zu {x.Value}.");
    }

    private static void Report(ref bool first, ref List<string> errors, ref List<ValidationIssue> issues, string file, int lineNo, string error)
    {
      if (first)
      {
        errors.Add($"\n{Path.GetFileName(file)}:");
        first = false;
      }

      var normalizedLine = lineNo > 0 ? lineNo : 1;
      errors.Add($" Zeile ({normalizedLine}): {error}");
      issues.Add(new ValidationIssue
      {
        FileName = Path.GetFileName(file),
        Line = normalizedLine,
        UserMessage = error
      });
    }
  }
}
