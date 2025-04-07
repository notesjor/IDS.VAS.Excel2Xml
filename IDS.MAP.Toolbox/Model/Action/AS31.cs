using System.IO;
using System.Linq;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS31 : AbstractAction
  {
    public override bool AutoRun { get; } = true;
    public override void Execute(ref MapConfiguration config)
    {
      var files = Directory.GetFiles(config.MapExtraFilePath, "*.xml", SearchOption.AllDirectories).Select(Path.GetFileName).ToArray();
      foreach (var file in files)
        File.Copy(Path.Combine(config.MapExtraFilePath, file), Path.Combine(config.WorkExtraFilePath, file), true);

      if(!Directory.Exists(config.WorkEtcFilePath))
        Directory.CreateDirectory(config.WorkEtcFilePath);
      File.Copy(Path.Combine(config.MapEtcFilePath, "map.dtd"), Path.Combine(config.WorkEtcFilePath, "map.dtd"), true);
    }
  }
}