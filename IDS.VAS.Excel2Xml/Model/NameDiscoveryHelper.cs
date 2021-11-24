using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.VAS.Excel2Xml.Model
{
  public static class NameDiscoveryHelper
  {
    private static Dictionary<string, string> _getPredicateNames = new Dictionary<string, string>
    {
      { "V", "Verbalprädikate" },
      { "V-m", "Mediales Verbalprädikate" },
      { "PG", "Prädikatsgefüge" },
      { "PG-m", "Mediales Prädikatsgefüge" },
    };

    public static string GetPredicateName(string code)
      => _getPredicateNames.ContainsKey(code) ? _getPredicateNames[code] : code;
  }
}
