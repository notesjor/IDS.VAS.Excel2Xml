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
        {"#", -1}, // 1, 2, 3, ...
        {"BELEG", -1}, // Text
        {"EINGANG", -1 }, // 1
        {"MUSTER", -1}, // VERBERGEN, BEZEUGEN, ...
        {"DIATHESE", -1}, // a, m, ...
        {"AUSLÖSER(SYN)", -1}, // SUB
        {"AUSLÖSER:ELEMENTE", -1}, // NP
        {"FIGUR(SYN)", -1}, // AKK, S, SUB
        {"FIGUR:ELEMENTE", -1}, // NP, sub, V, akk ...
        {"PRÄDIKATSTYP", -1}, // V, V-m
        {"PG:ELEMENTE", -1}, // akk
        {"GRUND(LEX)", -1}, // Fiskus, Problem, Zuschauer, ...
        {"GRUND(KOPF)", -1}, // VOR
        {"GRUND(KASUS)", -1}, // DAT
        {"BSP", -1}, // 1
        {"KONSTRUKTIONSTYP", -1}, // zu-inf
        {"COSMAS-SIGLE", -1 }, // R97/MAR.16447...
        {"MUSTERTYP", -1 }, // asm, akm
        {"LEXIKALISCHERPRÄDIKATSKERN",-1 }, // warnen, sich_drücken...
        {"QUELLE", -1}, // Frankfurter Rundschau,...
        {"JAHR", -1}, // 2007... 2012 ...
        {"TAGS", -1} // ...
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
