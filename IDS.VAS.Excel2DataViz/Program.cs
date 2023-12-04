using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using IDS.VAS.Excel2DataViz.Model;
using IDS.VAS.Excel2Xml.Model;
using IDS.Vas.ExcelReader;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2DataViz
{
  class Program
  {
    static void Main(string[] args)
    {
      var workbook = VasExcelReader.ReadExcel(args[0]);
      foreach (DataTable sheet in workbook.Tables)
      {
        var mapper = new ExcelColumnMapper();
        if (mapper.Map(sheet))
          continue;

        var rows = sheet.Rows.Cast<DataRow>()
                        .Where(row => row.ItemArray[mapper.Mapping["EINGANG"]]?.ToString()?.Trim() != "0")
                        .ToArray();

        MakeSimple(mapper, rows);
      }
    }

    private static void MakeSimple(ExcelColumnMapper mapper, DataRow[] rows)
    {
      var dict = new Dictionary<string, CirclePack>();

      foreach (var row in rows)
      {
        var idStr = row.ItemArray[mapper.Mapping["#"]]?.ToString();
        if (string.IsNullOrWhiteSpace(idStr))
          continue;

        var muster = row.ItemArray[mapper.Mapping["MUSTER"]]?.ToString();
        if (string.IsNullOrWhiteSpace(muster))
          continue;

        var pt = row.ItemArray[mapper.Mapping["PRÄDIKATSTYP"]]?.ToString();
        if (string.IsNullOrWhiteSpace(pt))
          continue;

        var pk = row.ItemArray[mapper.Mapping["PRÄDIKATSKERN"]]?.ToString();
        if (string.IsNullOrWhiteSpace(pk))
          continue;

        var gl = row.ItemArray[mapper.Mapping["GRUND(LEX)"]]?.ToString();
        if (string.IsNullOrWhiteSpace(gl))
          continue;

        var keys = new[] { pt, pk, gl };
        if (dict.ContainsKey(muster))
          dict[muster].Add(keys);
        else
        {
          var cp = new CirclePack{ Name = muster };
          cp.Add(keys);
          dict.Add(muster, cp);
        }
      }

      File.WriteAllText("data.json",
                        JsonConvert.SerializeObject(new CirclePack
                        {
                          Name = "flare",
                          Children = dict.Values.ToArray()
                        }));
    }
  }
}
