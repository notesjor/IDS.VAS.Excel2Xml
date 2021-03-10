using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace IDS.VAS.Excel2Xml.Forms.Abstract
{
  public partial class AbstractForm : RadForm
  {
    public AbstractForm()
    {
      try
      {
        ThemeResolutionService.ApplicationThemeName = "Office2019Light";
      }
      catch
      {
        // ignore
      }
      InitializeComponent();
    }
  }
}
