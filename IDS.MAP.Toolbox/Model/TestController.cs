using IDS.MAP.Toolbox.Model.TestStep.Abstract;
using IDS.MAP.Toolbox.Model.TestStep;
using System.IO;

namespace IDS.MAP.Toolbox.Model
{
  public class TestController
  {
    public AbstractTestStep[] Steps { get; } = { new S1(), new S2(), new S3(), new S4(), new S5() };
    public void Execute(ref MapConfiguration config)
    {
      if (!Directory.Exists(config.TmpPath))
        Directory.CreateDirectory(config.TmpPath);
      try
      {
        var file = Directory.GetFiles(config.TmpPath, "*", SearchOption.AllDirectories);
        foreach (var f in file)
          File.Delete(f);
      }
      catch
      {
        // ignore
      }
      Directory.CreateDirectory(Path.Combine(config.TmpPath, "excel"));
      Directory.CreateDirectory(Path.Combine(config.TmpPath, "artikel"));
      Directory.CreateDirectory(Path.Combine(config.TmpPath, "map"));
      Directory.CreateDirectory(Path.Combine(config.TmpPath, "map", "pages"));

      for (var i = 0; i < Steps.Length; i++)
      {
        Steps[i].Execute(ref config);
        if (Steps[i].Valid) 
          continue;

        if(config.ForcePublish)
        {
          config.ForcePublish = false;
          Level = Steps.Length;
          return;
        }

        Level = i;
        return;
      }
      Level = Steps.Length;
    }
    public int Level { get; private set; }
  }
}
