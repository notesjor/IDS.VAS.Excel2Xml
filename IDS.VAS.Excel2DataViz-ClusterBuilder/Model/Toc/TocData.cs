using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IDS.VAS.Excel2DataViz.ClusterBuilder.Model.Toc
{
  public class Entry
  {

    [JsonProperty("Label")]
    public string Label { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }
  }

  public class SubGroup
  {

    [JsonProperty("Label")]
    public string Label { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Entries")]
    public IList<Entry> Entries { get; set; }
  }

  public class TocData
  {

    [JsonProperty("Label")]
    public string Label { get; set; }

    [JsonProperty("Id")]
    public string Id { get; set; }

    [JsonProperty("Entries")]
    public IList<Entry> Entries { get; set; }

    [JsonProperty("SubGroups")]
    public IList<SubGroup> SubGroups { get; set; }
  }

}

