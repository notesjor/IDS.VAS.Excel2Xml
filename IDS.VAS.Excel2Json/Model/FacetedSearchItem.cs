using System.Collections.Generic;

namespace IDS.VAS.Excel2Json.Model
{
  public class FacetedSearchItem
  {
    public HashSet<string> Quellen { get; set; } = new HashSet<string>();
    public HashSet<string> Jahre { get; set; } = new HashSet<string>();
    public string Mustertyp { get; set; } = "";
    public HashSet<string> Prädikatskerne { get; set; } = new HashSet<string>();
    public HashSet<string> Prädikate { get; set; } = new HashSet<string>();
    public HashSet<string> Diathesen { get; set; } = new HashSet<string>();
    public HashSet<string> KTypen { get; set; } = new HashSet<string>();
    public HashSet<string> PrdMusterslot { get; set; } = new HashSet<string>();
    public HashSet<string> Auslöser { get; set; } = new HashSet<string>();
    public HashSet<string> Figuren { get; set; } = new HashSet<string>();
    public HashSet<string> GrundLex { get; set; } = new HashSet<string>();
    public HashSet<string> GrundSyn { get; set; } = new HashSet<string>();
    //public HashSet<string> Belege { get; set; } = new HashSet<string>();
    public string Name { get; set; }
  }
}
