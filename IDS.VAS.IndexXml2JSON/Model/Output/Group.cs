using System.Collections.Generic;

namespace IDS.VAS.IndexXml2JSON.Model.Output
{
  public class Group
  {
    public string Label { get; set; }
    public string Id { get; set; }
    public List<Entry> Entries { get; set; } = new List<Entry>();
    public List<Group> SubGroups { get; set; } = new List<Group>();
  }
}
