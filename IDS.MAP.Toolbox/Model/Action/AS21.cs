using System.IO;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS21 : AbstractAction
  {
    public override bool AutoRun { get; } = false;
    public override void Execute(ref MapConfiguration config)
    {
      try
      {
        MessageBox.Show("TODO"); // TODO
      }
      catch (System.Exception ex)
      {
        ShowActionError(nameof(AS21), ex);
      }
    }
  }
}