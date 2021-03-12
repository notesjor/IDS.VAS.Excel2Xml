using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExcelDataReader;
using IDS.VAS.Excel2Xml.Forms.Abstract;
using IDS.VAS.Excel2Xml.Model;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;
using Telerik.WinForms.Controls.SyntaxEditor.Taggers;
using Telerik.WinForms.SyntaxEditor.Core.Text;

namespace IDS.VAS.Excel2Xml.Forms
{
  public partial class MainForm : AbstractForm
  {
    private DataSet _workbook;
    private DataTable _worksheet;
    private ExcelColumnMapper _mapper;
    private string _clipboardForm;
    private string _clipboardSamples;
    private string _clipboardKwic;

    public MainForm()
    {
      InitializeComponent();

      var form_tagger = new XmlTagger(this.snippet_form.SyntaxEditorElement);
      snippet_form.TaggersRegistry.RegisterTagger(form_tagger);
      var form_foldingtagger = new XmlFoldingTagger(this.snippet_form.SyntaxEditorElement);
      snippet_form.TaggersRegistry.RegisterTagger(form_foldingtagger);

      var kwic_tagger = new XmlTagger(this.snippet_kwic.SyntaxEditorElement);
      snippet_kwic.TaggersRegistry.RegisterTagger(kwic_tagger);
      var kwic_foldingtagger = new XmlFoldingTagger(this.snippet_kwic.SyntaxEditorElement);
      snippet_kwic.TaggersRegistry.RegisterTagger(kwic_foldingtagger);

      var sample_tagger = new XmlTagger(this.snippet_sample.SyntaxEditorElement);
      snippet_sample.TaggersRegistry.RegisterTagger(sample_tagger);
      var sample_foldingtagger = new XmlFoldingTagger(this.snippet_sample.SyntaxEditorElement);
      snippet_sample.TaggersRegistry.RegisterTagger(sample_foldingtagger);
    }

    private void btn_openFile_Click(object sender, EventArgs e)
    {
      var ofd = new OpenFileDialog { Filter = "Excel-Arbeitsmappe (*.xslx)|*.xlsx" };
      if (ofd.ShowDialog() != DialogResult.OK)
        return;

      ReadExcel(ofd.FileName);

      cmb_spreadsheet.Items.Clear();
      foreach (DataTable t in _workbook.Tables)
        cmb_spreadsheet.Items.Add(t.TableName);
      cmb_spreadsheet.SelectedIndex = 0;
    }

    private void cmb_spreadsheet_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
    {
      if (cmb_spreadsheet.SelectedIndex == -1)
        return;

      _worksheet = _workbook.Tables[cmb_spreadsheet.SelectedIndex];
      _mapper = new ExcelColumnMapper();
      if (_mapper.Map(_worksheet))
      {
        MessageBox.Show("Die Excel-Datei enspricht nicht der Vorlage.");
        return;
      }

      cmb_pattern.Items.Clear();
      var idx = _mapper.Mapping["MUSTER"];
      var hashSet = new HashSet<string>(from DataRow row in _worksheet.Rows select row.ItemArray[idx].ToString());
      cmb_pattern.Items.AddRange(hashSet);
      cmb_pattern.SelectedIndex = 0;
    }

    private void ReadExcel(string path)
    {
      using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
      {
        var reader = ExcelReaderFactory.CreateReader(fs, new ExcelReaderConfiguration()
        {
          // Gets or sets the encoding to use when the input XLS lacks a CodePage
          // record, or when the input CSV lacks a BOM and does not parse as UTF8. 
          // Default: cp1252 (XLS BIFF2-5 and CSV only)
          FallbackEncoding = Encoding.GetEncoding(1252),

          // Gets or sets a value indicating whether to leave the stream open after
          // the IExcelDataReader object is disposed. Default: false
          LeaveOpen = false
        });

        _workbook = reader.AsDataSet(new ExcelDataSetConfiguration()
        {
          // Gets or sets a value indicating whether to set the DataColumn.DataType 
          // property in a second pass.
          UseColumnDataType = true,

          // Gets or sets a callback to determine whether to include the current sheet
          // in the DataSet. Called once per sheet before ConfigureDataTable.
          FilterSheet = (tableReader, sheetIndex) => sheetIndex == 0,

          // Gets or sets a callback to obtain configuration options for a DataTable. 
          ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
          {
            // Gets or sets a value indicating the prefix of generated column names.
            EmptyColumnNamePrefix = "Column",

            // Gets or sets a value indicating whether to use a row from the 
            // data as column names.
            UseHeaderRow = true,

            // Gets or sets a callback to determine whether to include the 
            // current row in the DataTable.
            FilterRow = (rowReader) => { return true; },

            // Gets or sets a callback to determine whether to include the specific
            // column in the DataTable. Called once per column after reading the 
            // headers.
            FilterColumn = (rowReader, columnIndex) => { return true; }
          }
        });
      }
    }

    private void cmb_pattern_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
    {
      RefreshSnippets();
    }

    private void RefreshSnippets()
    {
      if (cmb_pattern.SelectedIndex == -1)
        return;

      var items = _worksheet.Rows.Cast<DataRow>()
                            .Where(row => row.ItemArray[_mapper.Mapping["MUSTER"]].ToString() ==
                                          cmb_pattern.SelectedItem.Text).ToArray();

      BuildSamples(items);
      BuildForm(items);
      BuildSampleSearch(items);
    }

    private void BuildSampleSearch(IEnumerable<DataRow> items)
    {
      chk_kwic.Items.Clear();
      foreach (var row in items)
      {
        chk_kwic.Items.Add(new ListViewDataItem(row.ItemArray[_mapper.Mapping["BELEG"]].ToString())
        {
          Tag = $"\t<xref href=\"s_{ row.ItemArray[_mapper.Mapping["#"]]}\"/> <!-- { row.ItemArray[_mapper.Mapping["BELEG"]]} -->\r\n",
          CheckState = ToggleState.Off
        });
      }
    }

    private void BuildSamples(IEnumerable<DataRow> items)
    {
      var stb = new StringBuilder();
      stb.Append("<samples>\r\n");
      foreach (DataRow row in items)
        stb.Append($"\t<sample id=\"s_{row.ItemArray[_mapper.Mapping["#"]]}\">{KwicFix(row.ItemArray[_mapper.Mapping["BELEG"]].ToString())}</sample>\r\n");
      stb.Append("</samples>");

      _clipboardSamples = stb.ToString();
      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_clipboardSamples)))
      {
        using (StreamReader reader = new StreamReader(ms))
        {
          snippet_sample.Document = new TextDocument(reader);
        }
      }
    }

    private string KwicFix(string str)
    {
      str = str.Replace(" , ", ", ")
                .Replace(" : ", ": ")
                .Replace(" ? ", "? ")
                .Replace(" ! ", "! ")
                .Replace(" . ", ". ")
                .Replace(" ; ", "; ")
                .Replace("  ", " ");
      if (str.EndsWith(" ."))
        str = str.Substring(0, str.Length - 2) + ".";
      if (str.EndsWith(" ?"))
        str = str.Substring(0, str.Length - 2) + "!";
      if (str.EndsWith(" !"))
        str = str.Substring(0, str.Length - 2) + "?";
      return str;
    }

    private void BuildForm(IEnumerable<DataRow> items)
    {
      var dict = new Dictionary<string, FormSlot>();
      foreach (var item in items)
      {
        var slot = new FormSlot(_mapper, item, txt_form_defaultVsem.Text);
        var key = slot.GetXml(false);
        if (dict.ContainsKey(key))
        {
          if (chk_form_addSamples.Checked)
          {
            var pair = slot.Kwics.First();
            dict[key].Kwics.Add(pair.Key, pair.Value);
          }
          continue;
        }
        dict.Add(key, slot);
      }

      var stb = new StringBuilder();
      stb.Append("<forms>\r\n");
      stb.Append("\t<akt>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "akt"))
        stb.Append(pair.Value.GetXml(chk_form_addSamples.Checked));
      stb.Append("\t</akt>\r\n");
      stb.Append("\t<med>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "med"))
        stb.Append(pair.Value.GetXml(chk_form_addSamples.Checked));
      stb.Append("\t</med>\r\n");
      stb.Append("\t<pass>\r\n");
      foreach (var pair in dict.Where(x => x.Value.Type == "pass"))
        stb.Append(pair.Value.GetXml(chk_form_addSamples.Checked));
      stb.Append("\t</pass>\r\n");
      stb.Append("</forms>");

      _clipboardForm = stb.ToString();
      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_clipboardForm)))
      {
        using (StreamReader reader = new StreamReader(ms))
        {
          snippet_form.Document = new TextDocument(reader);
        }
      }
    }

    private void btn_form_clipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(_clipboardForm);
    }

    private void btn_kwic_filterDelete_Click(object sender, EventArgs e)
    {
      txt_kwic_filter.Text = "";
    }

    private void btn_kwic_clipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(_clipboardKwic);
    }

    private void txt_form_defaultVsem_TextChanged(object sender, EventArgs e)
    {
      RefreshSnippets();
    }

    private void chk_form_addSamples_ToggleStateChanged(object sender, StateChangedEventArgs args)
    {
      RefreshSnippets();
    }

    private void btn_kwic_unselectAll_Click(object sender, EventArgs e)
    {
      foreach (var item in chk_kwic.Items)
        item.CheckState = ToggleState.Off;
    }

    private void btn_kiwc_selectAll_Click(object sender, EventArgs e)
    {
      foreach (var item in chk_kwic.Items)
        item.CheckState = ToggleState.On;
    }

    private void chk_kwic_ItemCheckedChanged(object sender, ListViewItemEventArgs e)
    {
      var stb = new StringBuilder();
      stb.Append("<examples>\r\n");
      foreach (var item in chk_kwic.Items)
        if(item.CheckState == ToggleState.On)
          stb.Append(item.Tag);
      stb.Append("</examples>");

      _clipboardKwic = stb.ToString();
      using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(_clipboardKwic)))
      {
        using (StreamReader reader = new StreamReader(ms))
        {
          snippet_kwic.Document = new TextDocument(reader);
        }
      }
    }

    private void txt_kwic_filter_TextChanged(object sender, EventArgs e)
    {
      chk_kwic.EnableFiltering = true;
      chk_kwic.FilterDescriptors.Clear();
      chk_kwic.FilterDescriptors.Add(new FilterDescriptor("Value", FilterOperator.Contains, txt_kwic_filter.Text));
    }

    private void btn_samples_clipboard_Click(object sender, EventArgs e)
    {
      Clipboard.SetText(_clipboardSamples);
    }
  }
}
