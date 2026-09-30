using System.IO;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS11 : AbstractAction
  {
    public override bool AutoRun { get; } = false;
    public override void Execute(ref MapConfiguration config)
    {
      try
      {
        var openFileDialog = new OpenFileDialog
        {
          Filter = "MAP-Excel (data.xlsx)|data.xlsx",
          Title = "MAP-Excel (data.xlsx) auswählen",
          CheckFileExists = true,
          CheckPathExists = true,
          Multiselect = false
        };
        if (!string.IsNullOrEmpty(config.MapPath))
          openFileDialog.InitialDirectory = Path.Combine(config.MapPath, "excel");

        if (openFileDialog.ShowDialog() != DialogResult.OK)
          return;

        config.MapPath = Path.GetDirectoryName(Path.GetDirectoryName(openFileDialog.FileName));
      }
      catch (System.Exception ex)
      {
        ShowActionError(nameof(AS11), ex);
      }
    }
  }
}
