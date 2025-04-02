using System.IO;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class T00 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      if (string.IsNullOrEmpty(config.MapPath) || !Directory.Exists(config.MapPath))
        Valid = false;
      else
        Valid = File.Exists(Path.Combine(config.MapPath, "excel", "data.xlsx"));
    }

    public override bool BreakExecution { get; } = true;
  }
}