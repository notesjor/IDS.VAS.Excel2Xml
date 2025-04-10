using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Forms;

namespace IDS.MAP.Toolbox
{
  static class Program
  {
    private static bool _close = false;

    /// <summary>
    /// Der Haupteinstiegspunkt für die Anwendung.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      var form = new MainForm();
      form.FormClosing += (s, e) => { _close = true; };
      form.Show();

      while(!_close)
      {
        Application.DoEvents();
        Task.Delay(100).Wait();
      }
    }
  }
}
