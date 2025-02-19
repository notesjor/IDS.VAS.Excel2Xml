using System.Collections.Generic;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2DataViz.ClusterBuilder.Model.Flare
{
  public class CirclePackSimple
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("path")]
    public string Path { get; set; }

    [JsonProperty("children")]
    public IList<CirclePackSimple> Children { get; set; }

    [JsonProperty("value")]
    public int Value { get; set; }
  }
}