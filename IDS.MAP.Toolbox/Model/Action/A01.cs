using System.IO;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class A01 : AbstractAction
  {
    public override bool AutoRun { get; } = true;
    public override void Execute(ref MapConfiguration config) 
      => File.Copy(Path.Combine(config.MapPath, "excel", "data.xlsx"), Path.Combine(config.TmpPath, "excel", "data.xlsx"), true);
  }
}