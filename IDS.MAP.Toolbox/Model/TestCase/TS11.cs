using System.Collections.Generic;
using System.IO;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS11 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      DetailIssues = new List<ValidationIssue>();
      var excelFile = "data.xlsx";

      if (string.IsNullOrEmpty(config.MapPath) || !Directory.Exists(config.MapPath))
      {
        Valid = false;
        DetailErrorReport = "Der MAP-Pfad ist ungültig oder nicht vorhanden.";
        DetailIssues.Add(new ValidationIssue
        {
          FileName = excelFile,
          Line = 1,
          UserMessage = "Der MAP-Pfad ist ungültig oder nicht vorhanden."
        });
        return;
      }

      var fullPath = Path.Combine(config.MapPath, "excel", "data.xlsx");
      Valid = File.Exists(fullPath);
      if (Valid)
      {
        DetailErrorReport = null;
        return;
      }

      DetailErrorReport = "Die Datei data.xlsx wurde nicht gefunden.";
      DetailIssues.Add(new ValidationIssue
      {
        FileName = excelFile,
        Line = 1,
        UserMessage = "Die Datei data.xlsx wurde nicht gefunden."
      });
    }

    public override bool BreakExecution { get; } = true;
  }
}