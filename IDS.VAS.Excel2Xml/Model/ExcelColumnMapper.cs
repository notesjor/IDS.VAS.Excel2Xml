using System;
using System.Collections.Generic;
using System.Data;

namespace IDS.VAS.Excel2Xml.Model
{
  public class ExcelColumnMapper
  {
    public Dictionary<string, int> Mapping { get; set; }
      = new Dictionary<string, int>
      {
        { "#", -1 }, // 1, 2, 3, ...
        { "BELEG", -1 }, // Text
        { "EINGANG", -1 }, // 1
        { "MUSTER", -1 }, // VERBERGEN, BEZEUGEN, ...
        { "DIATHESE", -1 }, // a, m, ...
        { "DIATHESE:SUBTYP", -1 }, // spa, wpa, gko, ...
        { "AUSLÖSER(SYN)", -1 }, // SUB
        { "AUSLÖSER:ELEMENTE", -1 }, // NP
        { "FIGUR(SYN)", -1 }, // AKK, S, SUB
        { "FIGUR:ELEMENTE", -1 }, // NP, sub, V, akk ...
        { "PG:ELEMENTE", -1 }, // akk
        { "GRUND(LEX)", -1 }, // Fiskus, Problem, Zuschauer, ...
        { "GRUND(KOPF)", -1 }, // VOR
        { "GRUND(KASUS)", -1 }, // DAT
        { "BSP", -1 }, // 1
        { "PRÄDIKATSTYP", -1 }, // PRD(SYN) für XML-Artikel
        { "COSMAS-SIGLE", -1 }, // R97/MAR.16447...
        { "MUSTERTYP", -1 }, // asm, akm
        { "PRÄDIKATSKERN", -1 }, // warnen, sich_drücken...
        { "QUELLE", -1 }, // Frankfurter Rundschau,...
        { "JAHR", -1 }, // 2007... 2012 ...
        { "REL(GENERALISIERT)", -1 },
        { "REL:ELEMENTE", -1 },
        { "KEYWORDS", -1 },
        { "SONDERFORMEN", -1 }
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

      var count = 0;
      foreach (var x in Mapping)
      {
        if (x.Value > -1)
          continue;

        Console.WriteLine(x.Key);
        count++;
      }

      return count > 0;
    }
  }
}
