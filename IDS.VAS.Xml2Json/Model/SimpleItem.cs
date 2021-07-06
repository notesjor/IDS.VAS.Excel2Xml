using System.Collections.Generic;
using Newtonsoft.Json;

namespace IDS.VAS.Xml2Json.Model
{
  public class SimpleItem
  {
    private string _name;

    [JsonProperty("name")]
    public string Name
    {
      get => _name;
      set
      {
        _name = value;
        FileName = FileNameFix(value);
      }
    }

    [JsonProperty("filename")]
    public string FileName { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("value")]
    public int Value { get; set; }
    [JsonProperty("children", NullValueHandling = NullValueHandling.Ignore)]
    public List<SimpleItem> Children { get; set; } = new List<SimpleItem>();

    public int Finalize()
    {
      if (Children.Count == 0)
      {
        Children = null;
        Value = 1;
      }
      else
      {
        Value = 0;
        foreach (var child in Children)
          Value += child.Finalize();
      }

      return Value;
    }

    private static string FileNameFix(string pattern)
    {
      return
        "/article/" +
        pattern.ToLower()
                    .Replace("ä", "ae")
                    .Replace("ö", "oe")
                    .Replace("ü", "ue")
                    .Replace("ß", "ss");
    }
  }
}
