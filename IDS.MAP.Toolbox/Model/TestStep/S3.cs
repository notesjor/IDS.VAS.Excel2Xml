using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S3 : AbstractTestStep
  {
    public override Dictionary<string, AbstractTestCase> TestCases { get; } = new Dictionary<string, AbstractTestCase> { { "STRUCT", new TS31() }, { "ID", new TS32() } };
    public override Dictionary<string, AbstractAction> Actions { get; } = new Dictionary<string, AbstractAction> { { "MAP", new AS31() } };
  }
}