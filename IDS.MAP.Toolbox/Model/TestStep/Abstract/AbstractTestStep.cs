using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using System.Collections.Generic;
using System.Linq;

namespace IDS.MAP.Toolbox.Model.TestStep.Abstract
{
  public abstract class AbstractTestStep
  {
    public abstract Dictionary<string, AbstractTestCase> TestCases { get; }
    public abstract Dictionary<string, AbstractAction> Actions { get; }
    public bool Valid { get; set; }

    public void Execute(ref MapConfiguration config)
    {
      var valid = true;
      foreach (var x in TestCases)
      {
        if (x.Value.Execute(ref config))
          continue;

        valid = false;
        if (x.Value.BreakExecution)
          break;
      }
      Valid = valid;

      if (!valid) return;
      if (Actions == null) return;

      foreach (var x in Actions.Where(x => x.Value.AutoRun))
        x.Value.Execute(ref config);
    }
  }
}
