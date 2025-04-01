using IDS.MAP.Toolbox.Model;
using System;
using System.Linq;
using System.Windows.Forms;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class MainForm : Form
  {

    private MapConfiguration _config = new MapConfiguration();
    private TestController _controller = new TestController();

    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      SettingsLoad();
      RunTests();
    }

    private void btn_path_Click(object sender, EventArgs e)
      => _controller.Steps[0].Actions["EXCEL"].Execute(ref _config);

    private void SettingsLoad()
      => _config = MapConfiguration.Load();

    private void SettingsSave()
      => _config.SettingsSave();

    private void RunTests()
    {
      var level = _controller.Execute(ref _config);
      grp_1.Visible = level >= 1;
      grp_2.Visible = level >= 2;
      btn_2_noExcel.Visible = !_controller.Steps[1].TestCases["MISSED"].Valid;
      btn_2_excelColumns.Visible = !_controller.Steps[1].TestCases["COLUMNS"].Valid;
      btn_2_searchPreview.Visible = !_controller.Steps[1].TestCases.Any(x => x.Value.Valid);
      btn_2_createXml.Visible = !_controller.Steps[1].TestCases.Any(x => x.Value.Valid);

      grp_3.Visible = level >= 3;
      btn_3_nostructure.Visible = !_controller.Steps[2].TestCases["STRUCT"].Valid;
      btn_3_missingArticle.Visible = !_controller.Steps[2].TestCases["ARTICLE"].Valid;
      btn_3_noStructureEntry.Visible = !_controller.Steps[2].TestCases["ENTRY"].Valid;

      grp_4.Visible = level >= 4;
      btn_4_missingXml.Visible = !_controller.Steps[3].TestCases["ARTICLE"].Valid;
      btn_4_xmlSyntax.Visible = !_controller.Steps[3].TestCases["SYNTAX"].Valid;
      btn_4_xref.Visible = !_controller.Steps[3].TestCases["XREF"].Valid;
      bnt_4_link.Visible = !_controller.Steps[3].TestCases["LINK"].Valid;
      btn_4_cite.Visible = !_controller.Steps[3].TestCases["CITE"].Valid;

      grp_5.Visible = level >= 5;
    }

    private void btn_update_Click(object sender, EventArgs e)
      => RunTests();

    private void btn_build_Click(object sender, EventArgs e)
      => _controller.Build();

    private void btn_2_noExcel_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[1].TestCases["EXCEL"].DetailErrorReport);

    private void btn_2_excelColumns_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[1].TestCases["COLUMNS"].DetailErrorReport);

    private void btn_2_searchPreview_Click(object sender, EventArgs e)
      => _controller.Steps[1].Actions["SEARCH"].Execute(ref _config);

    private void btn_2_createXml_Click(object sender, EventArgs e)
      => _controller.Steps[1].Actions["XML"].Execute(ref _config);

    private void btn_3_nostructure_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[2].TestCases["STRUCT"].DetailErrorReport);

    private void btn_3_missingArticle_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[2].TestCases["ARTICLE"].DetailErrorReport);

    private void btn_3_noStructureEntry_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[2].TestCases["ENTRY"].DetailErrorReport);

    private void btn_4_xmlSyntax_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["SYNTAX"].DetailErrorReport);

    private void btn_4_xref_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["XREF"].DetailErrorReport);

    private void bnt_4_link_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["LINK"].DetailErrorReport);

    private void btn_4_cite_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["CITE"].DetailErrorReport);

    private void btn_4_missingXml_Click(object sender, EventArgs e)
      => DisplayError(_controller.Steps[3].TestCases["ARTICLE"].DetailErrorReport);

    private void DisplayError(string message)
    {
      var split = message.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
      if (split.Length > 1)
        new ErrorReportForm(message).ShowDialog();
      else
        MessageBox.Show(message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
      => SettingsSave();
  }
}
