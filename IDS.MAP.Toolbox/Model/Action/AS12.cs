using System.IO;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS12 : AbstractAction
  {
    public override bool AutoRun { get; } = true;
    public override void Execute(ref MapConfiguration config)
    {
      File.Copy(Path.Combine(config.MapPath, "excel", "data.xlsx"), 
        Path.Combine(config.TmpPath, "excel", "data.xlsx"),
        true);
      File.Copy(Path.Combine(config.AppPath, "XMap", "map.dtd"),
        Path.Combine(config.MapEtcFilePath, "map.dtd"),
        true);
      File.Copy(Path.Combine(config.AppPath, "XMap", "map.xsl"),
        Path.Combine(config.MapEtcFilePath, "map.xsl"),
        true);
      File.Copy(Path.Combine(config.AppPath, "XMap", "MAP.xpr"),
        Path.Combine(config.MapPath, "MAP.xpr"),
        true);
    }
  }
}