using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Model.Action.Abstract
{
  public abstract class AbstractAction
  {
    public abstract bool AutoRun { get; }
    public abstract void Execute(ref MapConfiguration config);

    protected static void ShowActionError(string actionName, Exception ex)
    {
      var line = TryGetLineNumber(ex);
      var lineText = line.HasValue ? $" Zeile: {line.Value}." : string.Empty;
      var message = $"Aktion '{actionName}' konnte nicht ausgeführt werden.{lineText}\r\n{ex.Message}";
      MessageBox.Show(message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    protected static int? TryGetLineNumber(Exception ex)
    {
      if (ex == null)
        return null;

      var trace = new StackTrace(ex, true);
      var frames = trace.GetFrames();
      if (frames == null)
        return null;

      foreach (var frame in frames)
      {
        var line = frame.GetFileLineNumber();
        if (line > 0)
          return line;
      }

      return null;
    }
  }
}
