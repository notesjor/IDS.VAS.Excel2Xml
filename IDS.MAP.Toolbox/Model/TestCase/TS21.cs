using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.Vas.ExcelReader;
using IDS.VAS.Excel2Xml.Model;
using System;
using System.Data;
using System.Linq;
using IDS.MAP.Toolbox.Model.Struktur;
using IDS.MAP.Toolbox.Model.Validation;
using System.Collections.Generic;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS21 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var issues = new List<ValidationIssue>();
      var excelFile = "data.xlsx";

      try
      {
        var data = VasExcelReader.ReadExcel(config.WorkExcelPath);

        var schema = new ExcelColumnMapper();
        schema.Map(data.Tables[0]);

        var missing = schema.Mapping.Where(x => x.Value == -1).Select(x => x.Key).ToArray();
        if (missing.Length == 0)
        {
          Valid = true;

          var sheet = data.Tables[0];
          config.PatternNames = new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[schema.Mapping["MUSTER"]].ToString().ToLower().Replace("ä", "ae").Replace("ö", "oe").Replace("ü", "ue").Replace("ß", "ss").Replace(" ", "_"));
        }
        else
        {
          Valid = false;
          DetailErrorReport = missing.BuildErrorMessage("Folgende Spalten (erste Zeile) fehlen in der excel.xlsx");
          foreach (var col in missing)
          {
            issues.Add(new ValidationIssue
            {
              FileName = excelFile,
              Line = 1,
              UserMessage = $"Fehlende Spalte: {col}."
            });
          }
        }

        // TEST: Kein PRP-Eintrag
        var errors = new List<string>();
        var patterns = new HashSet<string>(from DataRow row in data.Tables[0].Rows select row.ItemArray[schema.Mapping["MUSTER"]].ToString());
        foreach (var pattern in patterns)
        {
          var items = data.Tables[0].Rows.Cast<DataRow>()
            .Where(row => row.ItemArray[schema.Mapping["MUSTER"]].ToString() == pattern)
            .Where(row => row.ItemArray[schema.Mapping["EINGANG"]].ToString().Trim() == "1" || row.ItemArray[schema.Mapping["EINGANG"]].ToString().Trim() == "2")
            .Where(row => string.IsNullOrWhiteSpace(row.ItemArray[schema.Mapping["PRP"]].ToString()))
            .ToArray();

          if (items.Length != 0)
          {
            errors.Add($"Muster '{pattern}' hat {items.Length} Einträge ohne PRP-Eintrag.");
            issues.Add(new ValidationIssue
            {
              FileName = excelFile,
              Line = 1,
              UserMessage = $"Muster '{pattern}' hat {items.Length} Einträge ohne PRP-Eintrag."
            });
          }
        }

        if (errors.Count > 0)
        {
          Valid = false;
          DetailErrorReport = string.IsNullOrWhiteSpace(DetailErrorReport)
            ? string.Join("\n", errors)
            : DetailErrorReport + "\n" + string.Join("\n", errors);
        }
      }
      catch (Exception ex)
      {
        Valid = false;
        DetailErrorReport = ex.Message;
        issues.Add(new ValidationIssue
        {
          FileName = excelFile,
          Line = 1,
          UserMessage = ex.Message
        });
      }

      DetailIssues = issues;
    }

    public override bool BreakExecution { get; } = true;
  }
}
