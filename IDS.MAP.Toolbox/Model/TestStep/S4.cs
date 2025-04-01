using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S4 : AbstractTestStep
  {
    public override Dictionary<string, AbstractTestCase> TestCases { get; } = new Dictionary<string, AbstractTestCase> { { "ARTICLE", new T21() }, { "SYNTAX", new T22() }, { "XREF", new T23() }, { "LINK", new T24() }, { "CITE", new T25() } };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }
}