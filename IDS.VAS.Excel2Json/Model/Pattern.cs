using System.Collections.Generic;

namespace IDS.VAS.Excel2Json.Model
{
  public class Pattern
  {
    public int Id { get; set; }
    public int Prd { get; set; }
    public int Trigger { get; set; }
    public int Figure { get; set; }
    public int Ground { get; set; }

    public List<int> KwicIds { get; set; } = new List<int>();
    public HashSet<int> ArticleIds { get; set; } = new HashSet<int>();

    public string Key => $"{Prd} / {Trigger} / {Figure} / {Ground}";
  }
}