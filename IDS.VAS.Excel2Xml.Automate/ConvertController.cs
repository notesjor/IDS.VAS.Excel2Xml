using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using IDS.VAS.Excel2Xml.Automate.Model;
using IDS.VAS.Excel2Xml.Automate.Properties;
using IDS.VAS.Excel2Xml.Convert.Model;
using IDS.VAS.Excel2Xml.Model;
using IDS.Vas.ExcelReader;

namespace IDS.VAS.Excel2Xml.Automate
{
  public class ConvertController
  {
    public void Convert(string input, string output)
      => Convert(VasExcelReader.ReadExcel(input), output);

    private void Convert(DataSet excel, string output)
    {
      foreach (DataTable sheet in excel.Tables)
      {
        var mapper = new ExcelColumnMapper();
        if (mapper.Map(sheet))
          continue;

        var outputDir = Path.Combine(output, sheet.TableName);
        if (!Directory.Exists(outputDir))
          Directory.CreateDirectory(outputDir);

        var patterns = new HashSet<string>(from DataRow row in sheet.Rows select row.ItemArray[mapper.Mapping["MUSTER"]].ToString());
        foreach (var pattern in patterns)
        {
          if (string.IsNullOrWhiteSpace(pattern))
            continue;

          var items = sheet.Rows.Cast<DataRow>()
                           .Where(row => row.ItemArray[mapper.Mapping["MUSTER"]].ToString() == pattern)
                           .Where(row => row.ItemArray[mapper.Mapping["EINGANG"]].ToString().Trim() == "1" || row.ItemArray[mapper.Mapping["EINGANG"]].ToString().Trim() == "2")
                           .ToArray();

          if (items.Length == 0)
            continue;

          var patternFixed = PatternNameFix(pattern);
          IdGenerator.Init(patternFixed);

          var templateJ = Resources.TEMPLATE.Replace("$$$MUSTER$$$", patternFixed);
          templateJ = templateJ.Replace("$$$GROUND_HEAD$$$", GetGroundHead(items, mapper));
          templateJ = templateJ.Replace("$$$KEYWORDS$$$", GetKeywords(items, mapper));
          templateJ = templateJ.Replace("$$$ARTICLE_TYPE$$$", GetArticleType(items, mapper));
          templateJ = templateJ.Replace("$$$SAMPLES$$$", GetSampleContainer(items, mapper));
          templateJ = templateJ.Replace("$$$FORMS$$$", GetForms(items, mapper));
          templateJ = templateJ.Replace("$$$MAIN_PATTERN$$$", _mainPattern); // Muss nach FORMS ausgeführt werden, da dort _mainPattern ermittelt wird
          templateJ = templateJ.Replace("$$$PREDICATES$$$", GetPredicates(items, mapper));
          File.WriteAllText(Path.Combine(outputDir, $"{FileNameFix(pattern)}.xml"), templateJ, Encoding.UTF8);
        }
      }
    }

    private string GetSampleContainer(DataRow[] items, ExcelColumnMapper mapper)
    {
      return $"<samples>\r\n{string.Join("\r\n", GetSamples(items, mapper))}\r\n\t\t</samples>\r\n\t\t<!--\r\n\t\tTODO: \r\n\t\tFür Beispiele im Texte <xref>-Elemente kopieren\r\n\t\t\t<examples>\r\n\t\t\t\t<xref href=\"s_1072\"/>\r\n\t\t\t</examples>\r\n\r\n\t\tAufeinander folgende Beispiele in EINEM <examples>-Element bündeln\r\n\t\t  \t<examples>\r\n\t\t\t\t<xref href=\"s_1072\"/>\r\n\t\t\t\t<xref href=\"s_4075\"/>\r\n\t\t\t</examples>\r\n\r\n\t\tAuf diese Weise referenzierte Beispiele MÜSSEN oben im <samples>-Block ausgezeichnet werden! -->\r\n";
    }

    private List<string> GetSamples(DataRow[] items, ExcelColumnMapper mapper)
    {
      return items.Select(row => KwicFix(KwicHighlight(row, new Kwic
                   {
                     Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
                     Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
                     Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
                     Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString()
                   })))
                  .Select(kwic => $"\t\t\t<sample id=\"s_{kwic.Id}\" cosmas=\"{kwic.Sigle}\">{kwic.Text}</sample>")
                  .ToList();
    }

    private Kwic KwicHighlight(DataRow row, Kwic str)
    {
      return str;
    }

    private Kwic KwicFix(Kwic kwic)
    {
      var str = kwic.Text.Replace(" , ", ", ")
                    .Replace(" : ", ": ")
                    .Replace(" ? ", "? ")
                    .Replace(" ! ", "! ")
                    .Replace(" . ", ". ")
                    .Replace(" ; ", "; ")
                    .Replace("  ", " ")
                    .Replace("&", "&amp;");
      if (str.EndsWith(" ."))
        str = str.Substring(0, str.Length - 2) + ".";
      if (str.EndsWith(" ?"))
        str = str.Substring(0, str.Length - 2) + "!";
      if (str.EndsWith(" !"))
        str = str.Substring(0, str.Length - 2) + "?";

      kwic.Text = str;

      return kwic;
    }

    private string GetKeywords(DataRow[] items, ExcelColumnMapper mapper)
      => string.Join(", ", new HashSet<string>(items.SelectMany(row => row.ItemArray[mapper.Mapping["KEYWORDS"]]
                                                                          .ToString()
                                                                          .Trim()
                                                                          .Split(new[] { ", ", "; " },
                                                                            StringSplitOptions.RemoveEmptyEntries)
                                                                          .Select(x => x.Trim())))
                      .OrderBy(x => x));

    private string GetGroundHead(DataRow[] items, ExcelColumnMapper mapper) 
      => string.Join(", ", new HashSet<string>(items.Select(row => row.ItemArray[mapper.Mapping["GRUND(KOPF)"]].ToString().Trim())));

    private string GetArticleType(DataRow[] items, ExcelColumnMapper mapper)
      => string.Join(", ", new HashSet<string>(items.Select(row => row.ItemArray[mapper.Mapping["MUSTERTYP"]].ToString().Trim())));

    private string PatternNameFix(string pattern)
    {
      return pattern.Substring(0, 1).ToUpper() + pattern.Substring(1).ToLower();
    }

    private static string FileNameFix(string pattern)
    {
      return pattern.ToLower()
                    .Replace("ä", "ae")
                    .Replace("ö", "oe")
                    .Replace("ü", "ue")
                    .Replace("ß", "ss");
    }

    private string GetForms(DataRow[] items, ExcelColumnMapper mapper)
    {
      var dict = new Dictionary<string, FormSlot>();
      foreach (var item in items)
      {
        if (item.ItemArray[mapper.Mapping["EINGANG"]]?.ToString()?.Trim() != "1")
          continue;

        var slot = new FormSlot(mapper, item);
        var key = slot.GetXml(0, false);
        if (dict.ContainsKey(key)) // MERGE Multi-KWICs
        {
          var pair = slot.Kwics.First();
          dict[key].Kwics.Add(pair.Key, pair.Value);
          continue;
        }
        dict.Add(key, slot);
      }

      // Detect _mainPattern
      var mp = dict.OrderByDescending(x => x.Value.Kwics.Count).First().Value;
      _mainPattern = mp.GetXml(3, false);
      _mainPattern = _mainPattern.Replace("<prototype>", "").Replace("</prototype>", "");
      _mainPattern = $"<!-- Muster-Häufigkeit (Eingang = 1): {mp.Kwics.Count} -->{_mainPattern}";

      var stb = new StringBuilder();
      stb.Append("<forms>\r\n");

      var types = new HashSet<string>(dict.Select(x => x.Value.Ground));
      foreach (var type in types.OrderBy(x => x))
      {
        var ndict = dict.Where(x => x.Value.Ground == type).ToDictionary(x => x.Key, x => x.Value);
        BuildFormGroup(stb, ndict, type);
      }

      stb.Append("\t\t<!-- TODO: ggf. löschen -->\r\n\t\t\t<section label=\"Besonderheiten\">\r\n\t\t\t\t<!-- \r\n\t\t\t\t\tText und Beispiele hierher \r\n\t\t\t\t\t<p></p>\r\n\t\t\t\t\t<examples></examples>\r\n\t\t\t\t-->\r\n\t\t\t\t<p/>\r\n\t\t\t</section>\r\n");
      stb.Append("\t\t</forms>\r\n");
      return stb.ToString();
    }

    private static void BuildFormGroup(StringBuilder stb, Dictionary<string, FormSlot> dict, string formGroupType)
    {
      stb.Append($"\t\t\t<form-grp label=\"{formGroupType}\">\r\n");

      var akts = dict.Where(x => x.Value.Type == "akt").OrderByDescending(x => x.Value.Kwics.Count).ToArray();
      if (akts.Length > 0)
      {
        stb.Append("\t\t\t\t<akt>\r\n");
        foreach (var pair in akts)
          stb.Append("\t\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t\t</akt>\r\n");
      }

      var meds = dict.Where(x => x.Value.Type == "kon").OrderByDescending(x => x.Value.Kwics.Count).ToArray();
      if (meds.Length > 0)
      {
        stb.Append("\t\t\t\t<kon>\r\n");
        foreach (var pair in meds)
          stb.Append("\t\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t\t</kon>\r\n");
      }

      var passs = dict.Where(x => x.Value.Type == "pass").OrderByDescending(x => x.Value.Kwics.Count).ToArray();
      if (passs.Length > 0)
      {
        stb.Append("\t\t\t\t<pass>\r\n");
        foreach (var pair in passs)
          stb.Append("\t\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t\t</pass>\r\n");
      }

      var ambigs = dict.Where(x => x.Value.Type == "ambig").OrderByDescending(x => x.Value.Kwics.Count).ToArray();
      if (ambigs.Length > 0)
      {
        stb.Append("\t\t\t\t<ambig>\r\n");
        foreach (var pair in ambigs)
          stb.Append("\t\t\t\t" + pair.Value.GetXml(1, true));
        stb.Append("\t\t\t\t</ambig>\r\n");
      }

      stb.Append("\t\t\t</form-grp>\r\n");
    }

    private string _mainPattern;

    private string GetPredicates(DataRow[] items, ExcelColumnMapper mapper)
    {
      var stb = new StringBuilder();
      var first = true;

      var gpns = new HashSet<string>(items.Select(row => row.ItemArray[mapper.Mapping["PRÄDIKATSTYP"]].ToString()));

      foreach (var gpn in gpns)
      {
        var pn = NameDiscoveryHelper.GetPredicateName(gpn);

        if (first)
          first = false;
        else
          stb.Append("\r\n\t\t\t\t");

        var complex = new Dictionary<string, List<string>>();

        foreach (var row in items)
        {
          if (row.ItemArray[mapper.Mapping["PRÄDIKATSTYP"]].ToString() != gpn)
            continue;

          var kwic = KwicFix(new Kwic
          {
            Id = row.ItemArray[mapper.Mapping["#"]].ToString(),
            Text = row.ItemArray[mapper.Mapping["BELEG"]].ToString(),
            Sigle = row.ItemArray[mapper.Mapping["COSMAS-SIGLE"]].ToString(),
            Priority = row.ItemArray[mapper.Mapping["BSP"]].ToString(),
          });
          var co = row.ItemArray[mapper.Mapping["PRÄDIKATSKERN"]].ToString().Trim().Replace("_", " ");

          if (complex.ContainsKey(co))
            complex[co].Add($"\t\t\t\t\t\t\t<!-- <xref href=\"s_{kwic.Id}\"/> --> <!-- {kwic.Text} -->");
          else
            complex.Add(co, new List<string> { $"\t\t\t\t\t\t\t<xref href=\"s_{kwic.Id}\"/> <!-- {kwic.Text} -->" });
        }

        stb.Append($"<predicate-list label=\"{pn}\">\r\n{GetPredicateItems(complex)}\r\n\t\t\t\t</predicate-list>");
      }

      return stb.ToString();
    }

    private string GetPredicateItems(Dictionary<string, List<string>> items)
    {
      return string.Join("\r\n", items.OrderBy(x => x.Key).Select(x => GetPredicateItems(x.Key, x.Value, null)));
    }

    private string GetPredicateItems(string key, IEnumerable<string> values, string alt)
      => $"\t\t\t\t\t<predicate value=\"{key}\" {(string.IsNullOrEmpty(alt) ? "alt=\"\"" : $"alt=\"{alt}\"")}>\r\n\t\t\t\t\t\t<examples>\r\n{string.Join("\r\n", values)}\r\n\t\t\t\t\t\t</examples>\r\n\t\t\t\t\t</predicate>";
  }
}
