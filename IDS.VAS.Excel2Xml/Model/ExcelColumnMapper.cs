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
        { "AUSLÖSER(LEX)", -1 }, // Amerikaner, Kultur
        //{ "AUSLÖSER:ELEMENTE", -1 }, // NP -- Gelöscht 2023-10 >> "AUSLÖSER(LEX)"
        { "FIGUR(SYN)", -1 }, // AKK, S, SUB
        { "FIGUR(LEX)", -1 },
        //{ "FIGUR:ELEMENTE", -1 }, // NP, sub, V, akk ... -- Gelöscht 2023-10 >> "FIGUR(LEX)"
        { "PG:ELEMENTE", -1 }, // akk
        { "GRUND(LEX)", -1 }, // Fiskus, Problem, Zuschauer, ...
        { "PRP", -1 }, // VOR
        { "GRUND(SYN)", -1 }, // DAT
        { "BSP", -1 }, // 1
        { "PRÄDIKATSTYP", -1 }, // V, V-m .. PRD(SYN) für XML-Artikel
        { "COSMAS-SIGLE", -1 }, // R97/MAR.16447...
        { "MUSTERTYP", -1 }, // asm, akm
        { "PRÄDIKATSKERN", -1 }, // warnen, sich_drücken...
        { "QUELLE", -1 }, // Frankfurter Rundschau,...
        { "JAHR", -1 }, // 2007... 2012 ...
        { "REL(GENERALISIERT)", -1 },
        { "SCHLAGWORTE", -1 }, // FORMEL????: adversativ, final, kausativ
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
