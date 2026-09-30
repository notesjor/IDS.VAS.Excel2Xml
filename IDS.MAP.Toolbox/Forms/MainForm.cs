using IDS.MAP.Toolbox.Model;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using System;
using System.Linq;
using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class MainForm : Form
  {

    private MapConfiguration _config = new MapConfiguration();
    private TestController _controller = new TestController();
    private ValidationProcess _process = new ValidationProcess();

    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      SettingsLoad();
      txt_path.Text = _config.MapPath;
      RunTests();
    }

    private void btn_path_Click(object sender, EventArgs e)
    {
      _controller.Steps[0].Actions["CONNECT"].Execute(ref _config);
      txt_path.Text = _config.MapPath;
      SettingsSave();
      RunTests();
    }

    private void SettingsLoad()
      => _config = MapConfiguration.Load();

    private void SettingsSave()
      => _config.SettingsSave();

    private void RunTests()
    {
      WindowState = FormWindowState.Minimized;
      Hide();
      _process.Show();

      backgroundWorker.RunWorkerAsync();
    }

    private void btn_update_Click(object sender, EventArgs e)
      => RunTests();

    private void btn_build_Click(object sender, EventArgs e)
      => _controller.Steps[4].Actions["BUILD"].Execute(ref _config);

    private void btn_2_excelColumns_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[1].TestCases["MISSED"]);

    private void btn_2_searchPreview_Click(object sender, EventArgs e)
      => _controller.Steps[1].Actions["SEARCH"].Execute(ref _config);

    private void btn_2_createXml_Click(object sender, EventArgs e)
      => _controller.Steps[1].Actions["XML"].Execute(ref _config);

    private void btn_3_nostructure_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[2].TestCases["STRUCT"]);

    private void btn_3_wrongId_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[2].TestCases["ID"]);

    private void btn_4_xmlSyntax_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["SYNTAX"]);

    private void btn_4_xref_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["XREF"]);

    private void bnt_4_link_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["LINK"]);

    private void btn_4_missingXml_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["ARTICLE"]);

    private void DisplayError(AbstractTestCase testCase)
    {
      if (testCase == null)
        return;

      var message = testCase.DetailErrorReport;
      if (string.IsNullOrWhiteSpace(message))
        return;

      var split = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      if (split.Length > 1 || (testCase.DetailIssues != null && testCase.DetailIssues.Count > 0))
        new ErrorReportForm(message, testCase.DetailIssues).ShowDialog();
      else
        MessageBox.Show(message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
      => SettingsSave();

    private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
      _controller.Execute(ref _config);
    }

    private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
      var level = _controller.Level;
      SettingsSave();
      grp_1.Visible = level >= 0;
      grp_2.Visible = level >= 1;
      btn_2_excelColumns.Visible = !_controller.Steps[1].TestCases["MISSED"].Valid;
      //btn_2_searchPreview.Visible = _controller.Steps[1].TestCases.All(x => x.Value.Valid);
      btn_2_createXml.Visible = _controller.Steps[1].TestCases.All(x => x.Value.Valid);

      grp_3.Visible = level >= 2;
      btn_3_nostructure.Visible = !_controller.Steps[2].TestCases["STRUCT"].Valid;
      btn_3_wrongId.Visible = !_controller.Steps[2].TestCases["ID"].Valid;

      grp_4.Visible = level >= 3;
      btn_4_missingXml.Visible = !_controller.Steps[3].TestCases["ARTICLE"].Valid;
      btn_4_xmlSyntax.Visible = !_controller.Steps[3].TestCases["SYNTAX"].Valid;
      btn_4_xref.Visible = !_controller.Steps[3].TestCases["XREF"].Valid;
      bnt_4_link.Visible = !_controller.Steps[3].TestCases["LINK"].Valid;

      grp_5.Visible = level >= 5;

      _process.Hide();
      Show();
      WindowState = FormWindowState.Normal;
    }

    private bool _cheat = false;

    private void pictureBox1_DoubleClick(object sender, EventArgs e)
    {
      _cheat = true;
    }

    private void pictureBox1_Click(object sender, EventArgs e)
    {
      if(!_cheat)
        return;
      _config.ForcePublish = true;
      _cheat = false;
    }
  }
}
