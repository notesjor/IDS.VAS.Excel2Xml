using System.Collections.Generic;
using Newtonsoft.Json;

namespace IDS.VAS.Excel2Json.Model
{
  public class Pattern
  {
    public int Id { get; set; }
    public int Prd { get; set; }
    public int Trigger { get; set; }
    public int Figure { get; set; }
    public int Ground { get; set; }
    public int Stelligkeit { get; internal set; }

    public IEnumerable<int> ElementsPrd { get; set; }
    public IEnumerable<int> ElementsTrigger { get; set; }
    public IEnumerable<int> ElementsFigure { get; set; }
    public IEnumerable<int> ElementsGround { get; set; }
    public IEnumerable<int> Keywords { get; set; }

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