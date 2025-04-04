using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.Vas.ExcelReader;
using IDS.VAS.Excel2Xml.Model;
using System;
using System.Data;
using System.Linq;
using IDS.MAP.Toolbox.Model.Struktur;
using System.Collections.Generic;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS21 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
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
          config.PatternNames = new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[schema.Mapping["MUSTER"]].ToString().ToLower().Replace("ä","ae").Replace("ö","oe").Replace("ü","ue").Replace("ß","ss").Replace(" ", "_"));
        }
        else
        {
          Valid = false;
          DetailErrorReport = missing.BuildErrorMessage("Folgende Spalten (erste Zeile) fehlen in der excel.xlsx");
        }
      }
      catch (Exception ex)
      {
        DetailErrorReport = ex.Message;
      }
    }

    public override bool BreakExecution { get; } = true;
  }
}
