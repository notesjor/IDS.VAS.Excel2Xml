using System.Collections.Generic;

namespace IDS.VAS.Excel2Xml.Model
{
  public static class NameDiscoveryHelper
  {
    private static Dictionary<string, string> _getPredicateNames = new Dictionary<string, string>
    {
      { "V", "Verbalprädikate" },
      { "V-m", "Mediale Verbalprädikate" },
      { "PG", "Prädikatsgefüge" },
      { "PG-m", "Mediales Prädikatsgefüge" },
    };

    public static string GetPredicateName(string code)
      => _getPredicateNames.ContainsKey(code) ? _getPredicateNames[code] : code;

    private static Dictionary<string, string> _getDiatheseXmlValues = new Dictionary<string, string>
    {
      { "a", "akt" },
      { "k", "kon" },
      { "p", "pass" },
      { "ambig", "ambig" }
    };

    public static string GetDiatheseXmlValues(string code)
      => _getDiatheseXmlValues.ContainsKey(code) ? _getDiatheseXmlValues[code] : code;

    private static Dictionary<string, string> _getDiatheseNames = new Dictionary<string, string>
    {
      { "a", "Aktiv" },
      { "k", "Konvers" },
      { "p", "Passiv" },
      { "ambig", "Ambig" }
    };

    public static string GetDiatheseNames(string code)
      => _getDiatheseNames.ContainsKey(code) ? _getDiatheseNames[code] : code;

    public static string FixSourcesName(string code)
    {
      code = code.Trim();
      if (code[1] == ' ')
        code = code.Substring(2);
      return code;
    }

    private static Dictionary<string, string> _getDiatheseSubtypeNames = new Dictionary<string, string>
    {
      { "bpa", "bekommen-Passiv" },
      { "spa", "sein-Passiv" },
      { "wpa", "werden-Passiv" },
      { "gko", "gehören-Konverse" },
      { "hko", "haben-Konverse" },
      { "mko", "Modale Infinitkonverse" },
      { "sko", "sein-Konverse" }
    };

    public static string GetDiatheseSubtypeNames(string code)
      => _getDiatheseSubtypeNames.ContainsKey(code) ? _getDiatheseSubtypeNames[code] : code;

    private static Dictionary<string, string> _getSpecialFormNames = new Dictionary<string, string>
    {
      { "aci, zinf", "Akkusativ mit Infinitiv und satzwertigem zu-Infinitiv" },
      { "aci", "Akkusativ mit Infinitiv" },
      { "acp", "Akkusativ mit Partizip" },
      { "imp", "Imperativ" },
      { "lmed", "lassen-Medium" },
      { "med", "Medialkonstruktion" },
      { "prt", "Satzwertige Partizipialkonstruktion" },
      { "inf", "Satzwertiger reiner Infinitiv" },
      { "zinf", "Satzwertiger zu-Infinitiv" },
      { "zsp", "Zustandsperiphrase" }
    };

    public static string GetSpecialFormNames(string code)
      => _getSpecialFormNames.ContainsKey(code) ? _getSpecialFormNames[code] : code;
  }
}
