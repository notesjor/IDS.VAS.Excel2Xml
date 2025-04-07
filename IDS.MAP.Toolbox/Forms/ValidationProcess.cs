using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class ValidationProcess : Form
  {
    public ValidationProcess()
    {
      InitializeComponent();
      progressBar1.MarqueeAnimationSpeed = 100;
      progressBar1.Style = ProgressBarStyle.Marquee;
    }
  }
}
