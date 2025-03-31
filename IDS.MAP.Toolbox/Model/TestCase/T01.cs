using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDS.MAP.Toolbox.Model.TestCase
{
    public class T01 : AbstractTestCase
    {
      public override bool Execute(ref MapConfiguration config)
      {
        throw new NotImplementedException();
      }

      public override bool BreakExecution { get; } = true;
    }
}
