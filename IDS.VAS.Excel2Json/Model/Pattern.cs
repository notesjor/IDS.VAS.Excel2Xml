using System.Collections.Generic;

namespace IDS.VAS.Excel2Json.Model
{
  public class Pattern
  {
    public int Id { get; set; }
    public string Prd { get; set; }
    public string Trigger { get; set; }
    public string Figure { get; set; }
    public string Ground { get; set; }

    public List<int> KwicIds { get; set; } = new List<int>();
    public HashSet<int> ArticleIds { get; set; } = new HashSet<int>();

    public string Key => $"{Prd} / {Trigger} / {Figure} / {Ground}";
  }
}