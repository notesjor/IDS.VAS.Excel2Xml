using System.Collections.Generic;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2Json.Model
{
  public class Pattern
  {
    public int Id { get; set; }
    public IEnumerable<int> Prd { get; set; }
    public IEnumerable<int> Trigger { get; set; }
    public IEnumerable<int> Figure { get; set; }
    public IEnumerable<int> Ground { get; set; }

    public string DisplayPrd { get; set; }
    public string DisplayTrigger { get; set; }
    public string DisplayFigure { get; set; }
    public string DisplayGround { get; set; }

    public HashSet<int> KwicIds { get; set; } = new HashSet<int>();
    public HashSet<int> ArticleIds { get; set; } = new HashSet<int>();

    [JsonIgnore]
    public string Key => $"{DisplayPrd}/{DisplayTrigger}/{DisplayFigure}/{DisplayGround}";
  }
}