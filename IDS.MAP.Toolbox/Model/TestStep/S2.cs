using System;
using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S1 : AbstractTestStep
  {
    public override AbstractTestCase[] TestCases { get; } = Array.Empty<AbstractTestCase>();
    public override Dictionary<string, AbstractAction> Actions { get; }
  }

  public class S2 : AbstractTestStep
  {
    public override AbstractTestCase[] TestCases { get; } = { new T01(), new T02() };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }

  public class S3 : AbstractTestStep
  {
    public override AbstractTestCase[] TestCases { get; } = { new T11(), new T12(), new T13() };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }

  public class S4 : AbstractTestStep
  {
    public override AbstractTestCase[] TestCases { get; } = { new T21(), new T22(), new T23(), new T24(), new T25() };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }

  public class S5 : AbstractTestStep
  {
    public override AbstractTestCase[] TestCases { get; } = Array.Empty<AbstractTestCase>();
    public override Dictionary<string, AbstractAction> Actions { get; }
  }
}
