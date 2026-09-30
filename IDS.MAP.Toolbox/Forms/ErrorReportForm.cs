using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Forms
{
  public partial class ErrorReportForm : Form
  {
    private readonly IList<ValidationIssue> _allIssues;
    private readonly string _rawReport;

    public ErrorReportForm(string report, IList<ValidationIssue> issues = null)
    {
      InitializeComponent();

      _rawReport = report ?? string.Empty;
      _allIssues = (issues ?? new List<ValidationIssue>())
        .Where(x => x != null)
        .ToList();

      if (_allIssues.Count == 0 && !string.IsNullOrWhiteSpace(_rawReport))
        _allIssues = ParseRawReport(_rawReport);

      textBox1.Text = _rawReport;
      ConfigureGrid();
      WireEvents();
      ApplyFilter();
    }

    private void ConfigureGrid()
    {
      dataGridView1.AutoGenerateColumns = false;
    }

    private void WireEvents()
    {
      txtFilter.TextChanged += (sender, args) => ApplyFilter();
      dataGridView1.SelectionChanged += (sender, args) => ShowSelectedDetails();
    }

    private void ApplyFilter()
    {
      var query = (txtFilter.Text ?? string.Empty).Trim();

      IEnumerable<ValidationIssue> filtered = _allIssues
        .Where(IsStructuredIssue);

      if (!string.IsNullOrWhiteSpace(query))
      {
        filtered = filtered.Where(x =>
          Contains(x.FileName, query) ||
          Contains(x.UserMessage, query) ||
          Contains(x.TechnicalMessage, query) ||
          Contains(x.Suggestion, query));
      }

      var list = filtered
        .OrderBy(x => x.FileName)
        .ThenBy(x => x.Line ?? int.MaxValue)
        .Select(x => new ValidationIssueView(x))
        .ToList();

      dataGridView1.DataSource = list;

      var fileCount = list.Select(x => x.FileName).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).Count();
      lblSummary.Text = string.Format("{0} Treffer in {1} Datei(en)", list.Count, fileCount);

      tabStructured.Enabled = list.Count > 0;
      if (list.Count == 0)
        tabControl1.SelectedTab = tabText;

      ShowSelectedDetails();
    }

    private static bool IsStructuredIssue(ValidationIssue issue)
    {
      return issue != null
        && !string.IsNullOrWhiteSpace(issue.FileName)
        && issue.Line.HasValue
        && issue.Line.Value > 0
        && !string.IsNullOrWhiteSpace(issue.UserMessage);
    }

    private void ShowSelectedDetails()
    {
      var selected = dataGridView1.CurrentRow?.DataBoundItem as ValidationIssueView;
      if (selected == null)
      {
        txtDetails.Text = string.Empty;
        return;
      }

      var builder = new StringBuilder();
      builder.AppendLine("Datei: " + selected.FileName);
      builder.AppendLine("Position: Zeile " + selected.Line.Value);

      if (!string.IsNullOrWhiteSpace(selected.Context))
        builder.AppendLine("Kontext: " + selected.Context.Trim());

      if (!string.IsNullOrWhiteSpace(selected.TechnicalMessage))
        builder.AppendLine("Technische Meldung: " + selected.TechnicalMessage);

      if (!string.IsNullOrWhiteSpace(selected.Suggestion))
        builder.AppendLine("Vorschlag: " + selected.Suggestion);

      txtDetails.Text = builder.ToString().Trim();
    }

    private void CopyVisibleEntries()
    {
      if (dataGridView1.DataSource is List<ValidationIssueView> list && list.Count > 0)
      {
        var lines = list.Select(x => string.Format("{0}:{1} - {2}", x.FileName, x.Line, x.UserMessage));
        Clipboard.SetText(string.Join(Environment.NewLine, lines));
        return;
      }

      if (!string.IsNullOrWhiteSpace(_rawReport))
        Clipboard.SetText(_rawReport);
    }

    private void SaveReport()
    {
      var dialog = new SaveFileDialog
      {
        Filter = "Textdatei (*.txt)|*.txt",
        FileName = "Fehlerbericht.txt",
        AddExtension = true,
        DefaultExt = "txt"
      };

      if (dialog.ShowDialog() != DialogResult.OK)
        return;

      File.WriteAllText(dialog.FileName, _rawReport, Encoding.UTF8);
    }

    private static bool Contains(string value, string query)
      => !string.IsNullOrEmpty(value) && value.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;

    private static List<ValidationIssue> ParseRawReport(string report)
    {
      var result = new List<ValidationIssue>();
      var lines = (report ?? string.Empty)
        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

      string currentFile = null;

      foreach (var raw in lines)
      {
        var line = (raw ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(line))
          continue;

        if (line.StartsWith("Datei:", StringComparison.OrdinalIgnoreCase))
        {
          currentFile = line.Substring(6).Trim();
          continue;
        }

        if (line.EndsWith(":"))
        {
          var candidate = line.TrimEnd(':').Trim();
          if (candidate.IndexOf(".xml", StringComparison.OrdinalIgnoreCase) >= 0)
          {
            currentFile = candidate;
            continue;
          }
        }

        var structuredMatch = Regex.Match(line, @"^-\s*(?:\[(?<sev>[^\]]+)\]\s*)?Zeile\s*(?<line>\d+)\s*:\s*(?<msg>.+)$", RegexOptions.IgnoreCase);
        if (structuredMatch.Success)
        {
          int parsedLine;
          int.TryParse(structuredMatch.Groups["line"].Value, out parsedLine);

          result.Add(new ValidationIssue
          {
            Severity = ParseSeverity(structuredMatch.Groups["sev"].Value),
            FileName = currentFile,
            Line = parsedLine > 0 ? (int?)parsedLine : null,
            UserMessage = structuredMatch.Groups["msg"].Value,
            TechnicalMessage = structuredMatch.Groups["msg"].Value
          });
          continue;
        }

        var oldLineMatch = Regex.Match(line, @"^Zeile\s*\((?<line>\d+)\):\s*(?<msg>.+)$", RegexOptions.IgnoreCase);
        if (oldLineMatch.Success)
        {
          int parsedLine;
          int.TryParse(oldLineMatch.Groups["line"].Value, out parsedLine);

          result.Add(new ValidationIssue
          {
            Severity = ValidationSeverity.Error,
            FileName = currentFile,
            Line = parsedLine > 0 ? (int?)parsedLine : null,
            UserMessage = oldLineMatch.Groups["msg"].Value,
            TechnicalMessage = oldLineMatch.Groups["msg"].Value
          });
          continue;
        }

        var lineInTextMatch = Regex.Match(line, @"Zeile\s+(?<line>\d+)", RegexOptions.IgnoreCase);
        if (!string.IsNullOrWhiteSpace(currentFile) && lineInTextMatch.Success)
        {
          int parsedLine;
          int.TryParse(lineInTextMatch.Groups["line"].Value, out parsedLine);

          result.Add(new ValidationIssue
          {
            Severity = ValidationSeverity.Error,
            FileName = currentFile,
            Line = parsedLine > 0 ? (int?)parsedLine : null,
            UserMessage = line,
            TechnicalMessage = line
          });
          continue;
        }

        var genericFileMatch = Regex.Match(line, @"Die Datei\s+(?<file>[^\s]+)", RegexOptions.IgnoreCase);
        if (genericFileMatch.Success)
          currentFile = genericFileMatch.Groups["file"].Value.Trim();
      }

      return result;
    }

    private static ValidationSeverity ParseSeverity(string value)
    {
      var text = (value ?? string.Empty).Trim().ToLowerInvariant();
      if (text == "warnung")
        return ValidationSeverity.Warning;
      if (text == "hinweis")
        return ValidationSeverity.Info;
      return ValidationSeverity.Error;
    }

    private class ValidationIssueView
    {
      public ValidationIssueView(ValidationIssue issue)
      {
        FileName = issue.FileName;
        Line = issue.Line;
        UserMessage = issue.UserMessage;
        TechnicalMessage = issue.TechnicalMessage;
        Suggestion = issue.Suggestion;
        Context = issue.Context;
      }

      public string FileName { get; }
      public int? Line { get; }
      public string UserMessage { get; }
      public string TechnicalMessage { get; }
      public string Suggestion { get; }
      public string Context { get; }
    }
  }
}
