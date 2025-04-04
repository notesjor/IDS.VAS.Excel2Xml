using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S2 : AbstractTestStep
  {
    public override Dictionary<string, AbstractTestCase> TestCases { get; } = new Dictionary<string, AbstractTestCase> { { "MISSED", new TS21() } };
    public override Dictionary<string, AbstractAction> Actions { get; } = new Dictionary<string, AbstractAction> { { "SEARCH", new AS21() }, { "XML", new AS22() } };
  }
}
