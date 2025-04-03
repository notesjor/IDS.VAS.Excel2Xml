using System.IO;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class A20 : AbstractAction
  {
    public override bool AutoRun { get; } = false;
    public override void Execute(ref MapConfiguration config)
    {
      MessageBox.Show("TODO"); // TODO
    }
  }
}