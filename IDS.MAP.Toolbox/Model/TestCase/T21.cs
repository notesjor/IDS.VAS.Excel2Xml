using System;
using System.Collections.Generic;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class T21 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      foreach (var x in config.PatternNames)
        if(!config.PatternIds.Contains(x))
          errors.Add(x);

      Valid = errors.Count == 0;
      DetailErrorReport = errors.Count == 0
        ? string.Empty
        : errors.BuildErrorMessage("Die folgenden IDs sind in der Excel vorhanden, es gibt jedoch keine XML-Datei:");
    }

    public override bool BreakExecution { get; } = false;
  }
}