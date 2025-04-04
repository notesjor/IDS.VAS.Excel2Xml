using System.Collections.Generic;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.MAP.Toolbox.Model.TestCase;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.TestStep.Abstract;

namespace IDS.MAP.Toolbox.Model.TestStep
{
  public class S4 : AbstractTestStep
  {
    public override Dictionary<string, AbstractTestCase> TestCases { get; } = new Dictionary<string, AbstractTestCase> { { "ARTICLE", new TS41() }, { "SYNTAX", new TS42() }, { "XREF", new TS43() }, { "LINK", new TS44() }, { "CITE", new TS45() } };
    public override Dictionary<string, AbstractAction> Actions { get; }
  }
}