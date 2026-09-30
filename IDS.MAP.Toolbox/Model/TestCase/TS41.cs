using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS41 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var files = new HashSet<string>(Directory.GetFiles(config.WorkArticlePath, "*.xml", SearchOption.AllDirectories).Select(Path.GetFileNameWithoutExtension));
      var errors = new List<string>();
      var issues = new List<ValidationIssue>();

      foreach(var x in config.PatternIds)
      {
        if(files.Contains(x))
          files.Remove(x);
        else
        {
          errors.Add(x);
          issues.Add(new ValidationIssue
          {
            FileName = x + ".xml",
            Line = 1,
            UserMessage = "Muster in Excel vorhanden, aber kein XML-Dokument gefunden."
          });
        }
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.Count == 0 ?
        null:
        errors.BuildErrorMessage("Nicht bearbeitete Muster (in Excel vorhanden / kein XML-Dokument):");
      DetailIssues = issues;
    }

    public override bool BreakExecution { get; } = false;
  }
}