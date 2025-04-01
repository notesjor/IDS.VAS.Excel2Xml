using System.IO;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Action.Abstract;

namespace IDS.MAP.Toolbox.Model.Action
{
    public class A00 : AbstractAction
    {
      public override bool AutoRun { get; } = true;
      public override void Execute(ref MapConfiguration config)
      {
        var openFileDialog = new OpenFileDialog
        {
          Filter = "MAP-Excel (data.xlsx)|data.xlsx",
          Title = "MAP-Excel (data.xlsx) auswählen",
          InitialDirectory = Path.Combine(config.MapDirectory, "excel"),
          CheckFileExists = true,
          CheckPathExists = true,
          Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK) 
          return;

        config.MapDirectory = Path.GetDirectoryName(Path.GetDirectoryName(openFileDialog.FileName));
      }
    }

    public class A20 : AbstractAction
    {
      public override bool AutoRun { get; } = true;
      public override void Execute(ref MapConfiguration config)
      {
        var openFileDialog = new OpenFileDialog
        {
          Filter = "MAP-Excel (data.xlsx)|data.xlsx",
          Title = "MAP-Excel (data.xlsx) auswählen",
          InitialDirectory = Path.Combine(config.MapDirectory, "excel"),
          CheckFileExists = true,
          CheckPathExists = true,
          Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK) 
          return;

        config.MapDirectory = Path.GetDirectoryName(Path.GetDirectoryName(openFileDialog.FileName));
      }
    }

    public class A21 : AbstractAction
    {
      public override bool AutoRun { get; } = true;
      public override void Execute(ref MapConfiguration config)
      {
        var openFileDialog = new OpenFileDialog
        {
          Filter = "MAP-Excel (data.xlsx)|data.xlsx",
          Title = "MAP-Excel (data.xlsx) auswählen",
          InitialDirectory = Path.Combine(config.MapDirectory, "excel"),
          CheckFileExists = true,
          CheckPathExists = true,
          Multiselect = false
        };

        if (openFileDialog.ShowDialog() != DialogResult.OK) 
          return;

        config.MapDirectory = Path.GetDirectoryName(Path.GetDirectoryName(openFileDialog.FileName));
      }
    }
}
