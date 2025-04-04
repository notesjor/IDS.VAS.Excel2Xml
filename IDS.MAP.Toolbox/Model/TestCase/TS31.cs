using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS31 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      config.WorkStructFiles = new List<string>();
      config.Links = new HashSet<string>();

      var dirs = Directory.GetDirectories(config.MapArticlePath).Select(Path.GetFileName).ToArray();
      foreach (var dir in dirs)
      {
        var outDir = Path.Combine(config.WorkArticlePath, dir);
        if (!Directory.Exists(outDir))
          Directory.CreateDirectory(outDir);

        var input = Path.Combine(config.MapArticlePath, dir, "_struktur.xml");
        var output = Path.Combine(config.WorkArticlePath, dir, "_struktur.xml");

        if (File.Exists(input))
        {
          File.Copy(input, output, true);
          config.WorkStructFiles.Add(output);
        }
        else
          errors.Add(dir);
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.Count == 0
        ? string.Empty
        : errors.BuildErrorMessage("Für die folgenden PRP fehlen die Strukturdateien (_struktur.xml):");
    }

    public override bool BreakExecution { get; } = false;
  }
}