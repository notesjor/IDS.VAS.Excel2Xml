using System.Collections.Generic;

namespace IDS.VAS.Excel2Json.Model
{
  public class Article
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public HashSet<int> PatternIds { get; set; } = new HashSet<int>();
  }
}