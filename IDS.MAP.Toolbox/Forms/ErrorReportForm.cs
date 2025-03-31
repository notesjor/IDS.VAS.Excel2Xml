using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class ErrorReportForm : Form
  {
    public ErrorReportForm(string report)
    {
      InitializeComponent();
      textBox1.Text = report;
    }
  }
}
