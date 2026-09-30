using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS51 : AbstractAction
  {
    public override bool AutoRun { get; } = false;
    public override void Execute(ref MapConfiguration config)
    {
      try
      {
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        var saveFileDialog = new SaveFileDialog
        {
          Filter = $"MAP-Publikation (MAP_{date}.zip)|MAP_{date}.zip",
          Title = "MAP-Publikation erstellen",
          CheckPathExists = true,
          FileName = $"MAP_{date}.zip",
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        };
        if (saveFileDialog.ShowDialog() != DialogResult.OK)
          return;

        ZipFile.CreateFromDirectory(config.TmpPath, saveFileDialog.FileName);
      }
      catch (Exception ex)
      {
        ShowActionError(nameof(AS51), ex);
      }
    }
  }
}