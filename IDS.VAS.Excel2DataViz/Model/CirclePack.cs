using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2DataViz.Model
{
  public class CirclePack
  {
    [JsonIgnore]
    private Dictionary<string, CirclePack> _data = new Dictionary<string, CirclePack>();

    public void Add(params string[] subKeys)
    {
      var keys = subKeys?.ToList();
      if (keys == null || keys.Count == 0)
        Value++;
      else
      {
        var key = keys[0];
        keys.RemoveAt(0);

        if (_data.ContainsKey(key))
          _data[key].Add(keys.ToArray());
        else
        {
          var cp = new CirclePack { Name = key, Path = $"{Path}/{key}" };
          cp.Add(keys.ToArray());
          _data.Add(key, cp);
          Children = _data.Values.ToArray();
        }
      }
    }

    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("path")]
    public string Path { get; set; }
    [JsonProperty("children")]
    public CirclePack[] Children { get; set; }

    [JsonProperty("value", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int Value { get; set; } = 0;

    public void CalculateValue()
    {
      if (Children == null || Children.Length == 0)
      {
        Value = 1;
        return;
      }

      foreach (var child in Children)
      {
        child.CalculateValue();
        Value += child.Value;
      }
    }
  }
}
