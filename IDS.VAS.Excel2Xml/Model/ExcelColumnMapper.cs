using System;
using System.Collections.Generic;
using System.Data;

namespace IDS.VAS.Excel2Xml.Model
{
  public class ExcelColumnMapper
  {
    public Dictionary<string, int> Mapping { get; set; } = new Dictionary<string, int>();

    public ExcelColumnMapper()
    {
      Mapping.Add("#", -1); // 1, 2, 3, ...
      Mapping.Add("BELEG", -1); // Text
      Mapping.Add("EINGANG", -1); // 1
      Mapping.Add("MUSTER", -1); // VERBERGEN, BEZEUGEN, ...
      Mapping.Add("MUSTERTYP", -1); // asm, prp, ...
      Mapping.Add("DIATHESE", -1); // a, m, ...
      Mapping.Add("DIATHESE:SUBTYP", -1); // spa, wpa, gko, ...
      Mapping.Add("AUSLÖSER(SYN)", -1); // SUB
      Mapping.Add("AUSLÖSER(LEX)", -1); // Amerikaner, Kultur
      //{ "AUSLÖSER:ELEMENTE", -1 ); // NP -- Gelöscht 2023-10 >> "AUSLÖSER(LEX)"
      Mapping.Add("FIGUR(SYN)", -1); // AKK, S, SUB
      Mapping.Add("FIGUR(LEX)", -1);
      //{ "FIGUR:ELEMENTE", -1 ); // NP, sub, V, akk ... -- Gelöscht 2023-10 >> "FIGUR(LEX)"
      Mapping.Add("PG:ELEMENTE", -1); // akk
      Mapping.Add("GRUND(LEX)", -1); // Fiskus, Problem, Zuschauer, ...
      Mapping.Add("PRP", -1); // VOR
      Mapping.Add("GRUND(SYN)", -1); // DAT
      Mapping.Add("BSP", -1); // 1
      Mapping.Add("PRÄDIKATSTYP", -1); // V, V-m .. PRD(SYN) für XML-Artikel
      Mapping.Add("COSMAS-SIGLE", -1); // R97/MAR.16447...
      Mapping.Add("PRÄDIKATSKERN", -1); // warnen, sich_drücken...
      Mapping.Add("QUELLE", -1); // Frankfurter Rundschau,...
      Mapping.Add("JAHR", -1); // 2007... 2012 ...
      Mapping.Add("REL(GENERALISIERT)", -1); // PRD+, PRD
      Mapping.Add("REL+:SYN", -1); // prp-auf, sub, akk, dat
      Mapping.Add("SCHLAGWORTE", -1); // FORMEL????: adversativ, final, kausativ
      Mapping.Add("SONDERFORMEN", -1);
      Mapping.Add("ARG-STR-LEX", -1); // Revisionsspalte: sich_drücken | (NAME) (–) vor (Arbeit)
      Mapping.Add("STELLIGKEIT(TYP)", -1); // 2, 3
    }

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
