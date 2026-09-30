using System;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;
using IDS.VAS.Excel2Xml.Automate;

namespace IDS.MAP.Toolbox.Model.Action
{
  public class AS22 : AbstractAction
  {
    public override bool AutoRun { get; } = false;
    public override void Execute(ref MapConfiguration config)
    {
      try
      {
        var saveFileDialog = new SaveFileDialog
        {
          Filter = "MAP-XML (*.zip)|*.zip",
          Title = "XML-Dateien speichern",
          InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
          FileName = $"{DateTime.Now:yyyy-MM-dd}_MAP.zip",
        };

        if (saveFileDialog.ShowDialog() != DialogResult.OK)
          return;

        if (File.Exists(saveFileDialog.FileName))
          File.Delete(saveFileDialog.FileName);

        var tmpDir = Path.Combine(Path.GetTempPath(), "MAP-XML");

        var controller = new ConvertController();
        controller.Convert(config.WorkExcelPath, tmpDir);

        ZipFile.CreateFromDirectory(tmpDir, saveFileDialog.FileName);
        try
        {
          Directory.Delete(tmpDir, true);
        }
        catch
        {
          //ignore
        }
      }
      catch (Exception ex)
      {
        ShowActionError(nameof(AS22), ex);
      }
    }
  }
}