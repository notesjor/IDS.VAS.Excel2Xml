using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDS.VAS.Excel2DataViz.ClusterBuilder.Model.Flare
{
  public class Child
  {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("children")]
    public IList<Child> Children { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }
  }

  public class FlareData
  {

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("children")]
    public IList<Child> Children { get; set; }
  }
}


