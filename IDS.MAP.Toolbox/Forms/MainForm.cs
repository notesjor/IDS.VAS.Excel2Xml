using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class MainForm : Form
  {
    private string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IDS", "MAP", "config.json");
    private MapConfiguration _config = null;

    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {

    }

    private void btn_path_Click(object sender, EventArgs e)
    {

    }

    private void SettingsLoad()
    {
      var dir = Path.GetDirectoryName(_configPath);
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);
      
      _config = File.Exists(_configPath) ? JsonConvert.DeserializeObject<MapConfiguration>(File.ReadAllText(_configPath, Encoding.UTF8)) : new MapConfiguration();
    }

    private void SettingsSave() 
      => File.WriteAllText(_configPath, JsonConvert.SerializeObject(_config), Encoding.UTF8);

    private void RunTests()
    {
      if (!string.IsNullOrEmpty(_mapDir))
        return;
    }
  }
}
