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
    public IEnumerable<int> GroundHead { get; set; }
    public IEnumerable<int> GroundCase { get; set; }

    public string DisplayPrd { get; set; }
    public string DisplayTrigger { get; set; }
    public string DisplayFigure { get; set; }
    public string DisplayGroundHead { get; set; }
    public string DisplayGroundCase { get; set; }

    public HashSet<int> KwicIds { get; set; } = new HashSet<int>();
    public HashSet<int> ArticleIds { get; set; } = new HashSet<int>();

    [JsonIgnore]
    public string Key => $"{DisplayPrd}/{DisplayTrigger}/{DisplayFigure}/{DisplayGroundHead}+{DisplayGroundCase}";
  }
}