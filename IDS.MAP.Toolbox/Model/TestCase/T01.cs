using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.Vas.ExcelReader;
using IDS.VAS.Excel2Xml.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class T01 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      try
      {
        var data = VasExcelReader.ReadExcel(Path.Combine(config.TmpPath, "excel", "data.xlsx"));

        var schema = new ExcelColumnMapper();
        schema.Map(data.Tables[0]);

        var missing = schema.Mapping.Where(x => x.Value == -1).Select(x => x.Key).ToArray();
        if (missing.Length == 0)
          Valid = true;
        else
        {
          Valid = false;
          var error = new List<string> { "Folgende Spalten (erste Zeile) fehlen in der excel.xlsx" };
          error.AddRange(missing);
          DetailErrorReport = string.Join("\r\n", error);
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
