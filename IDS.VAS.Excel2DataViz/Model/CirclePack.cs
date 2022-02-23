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
        Size++;
      else
      {
        var key = keys[0];
        keys.RemoveAt(0);

        if (_data.ContainsKey(key))
          _data[key].Add(keys.ToArray());
        else
        {
          var cp = new CirclePack{Name = key};
          cp.Add(keys.ToArray());
          _data.Add(key, cp);
          Children = _data.Values.ToArray();
        }
      }
    }

    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("children")]
    public CirclePack[] Children { get; set; }

    [JsonProperty("size", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public int Size { get; set; } = 0;
  }
}
