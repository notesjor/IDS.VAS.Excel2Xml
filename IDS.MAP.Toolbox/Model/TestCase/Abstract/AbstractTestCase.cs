namespace IDS.MAP.Toolbox.Model.TestCase.Abstract
{
  public abstract class AbstractTestCase
  {
    public string Input { get; set; }
    public string Output { get; set; }

    public abstract bool Execute(ref MapConfiguration config);

    public string DetailErrorReport { get; set; }
    public abstract bool BreakExecution { get; }
  }
}
