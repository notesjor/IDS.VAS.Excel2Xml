using System.Collections.Generic;

namespace IDS.VAS.Excel2Xml.Automate.Model
{
  public static class IdGenerator
  {
    private static string _doc;
    private static Dictionary<string, int> _id;

    public static void Init(string doc)
    {
      _doc = doc;
      _id = new Dictionary<string, int>();
    }

    public static string GetId(string idName)
    {
      if(!_id.ContainsKey(idName))
        _id.Add(idName, 0);

      _id[idName]++;
      return $"{_doc}_{idName}{_id[idName]:D3}";
    }
  }
}
