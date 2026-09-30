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
      try
      {
        var pages = Path.Combine(config.MapExtraFilePath, "pages");
        var files = Directory.GetFiles(pages, "*.xml", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray();
        foreach (var file in files)
        {
          File.Copy(Path.Combine(pages, file), Path.Combine(config.WorkExtraFilePath, "pages", file), true);
          config.Pages.Add(Path.GetFileNameWithoutExtension(file));
        }

        if (!Directory.Exists(config.WorkEtcFilePath))
          Directory.CreateDirectory(config.WorkEtcFilePath);
        File.Copy(Path.Combine(config.MapExtraFilePath, "literatur.xml"), Path.Combine(config.WorkExtraFilePath, "literatur.xml"), true);
      }
      catch (System.Exception ex)
      {
        ShowActionError(nameof(AS31), ex);
      }
    }
  }
}