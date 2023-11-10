namespace IDS.VAS.Excel2Xml.Convert.Model
{
  public class Kwic
  {
    public string Id { get; set; }
    public string Text { get; set; }
    public string Priority { get; set; }
    public int PriorityIndex => int.TryParse(Priority, out var res) ? res : 0;
    public string Sigle { get; set; }
    public string MType { get; set; }
    public string ArgStr { get; internal set; }

    public override string ToString() => Text;
  }
}
