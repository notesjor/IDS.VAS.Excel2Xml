using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase.Abstract
{
  public abstract class AbstractTestCase
  {
    public string Input { get; set; }
    public string Output { get; set; }

    public abstract void Execute(ref MapConfiguration config);

    public bool Valid { get; set; }
    public string DetailErrorReport { get; set; }
    public IList<ValidationIssue> DetailIssues { get; set; }
    public abstract bool BreakExecution { get; }
  }
}
