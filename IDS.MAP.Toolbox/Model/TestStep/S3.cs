using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S3 : AbstractTestStep
  {
    public override Dictionary<string, AbstractTestCase> TestCases { get; } = new Dictionary<string, AbstractTestCase> { { "STRUCT", new T11() }, { "ID", new T12() } };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }
}