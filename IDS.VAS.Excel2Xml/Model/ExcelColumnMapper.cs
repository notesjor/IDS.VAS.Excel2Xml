using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace IDS.VAS.Excel2Xml.Model
{
  public class ExcelColumnMapper
  {
    public Dictionary<string, int> Mapping { get; set; }
      = new Dictionary<string, int>
      {
        {"#", -1},
        {"BELEG", -1},
        {"MUSTER", -1},
        {"PRÄDIKATSKERN(LEX)", -1},
        {"PRÄDIKAT(TYP)", -1},
        {"MUSTERPRÄDIKAT", -1},
        {"AUSLÖSER(SYN)", -1},
        {"FIGUR(SYN)", -1},
        {"GRUND(LEX)", -1},
        {"GRUND(SYN)", -1},
        {"ARG-STR", -1}
      };

    public bool Map(DataTable table)
    {
      for (var i = 0; i < table.Columns.Count; i++)
      {
        var c = table.Columns[i];
        var n = c.ColumnName.ToUpper().Replace(" ", "");
        if (Mapping.ContainsKey(n))
          Mapping[n] = i;
      }

      return Mapping.Any(x => x.Value == -1);
    }
  }
}
