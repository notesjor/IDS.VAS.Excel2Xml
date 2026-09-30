using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS42 : AbstractTestCase
  {
    private static readonly Regex LinePattern = new Regex(@"line\s+(\d+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override void Execute(ref MapConfiguration config)
    {
      var issues = new List<ValidationIssue>();
      var files = config.WorkXmlFiles;

      var styleOrig = Path.Combine(config.AppPath, "XMAP", "map.xsl");
      var styleWork = Path.Combine(config.WorkEtcFilePath, "map.xsl");
      File.Copy(styleOrig, styleWork, true);
      File.Copy(Path.Combine(config.AppPath, "XMAP", "map.dtd"), Path.Combine(config.WorkEtcFilePath, "map.dtd"), true);

      var transform = Path.Combine(config.AppPath, "XDependencies", "Transform.exe");

      foreach (var file in files)
      {
        try
        {
          var output = file.Replace(".xml", ".html");
          if (File.Exists(output))
            File.Delete(output);

          var workDir = Path.GetDirectoryName(file);
          Directory.SetCurrentDirectory(workDir);
          Environment.CurrentDirectory = workDir;

          var arguments = string.Join(" ", new[]
          {
            $"-s:\"{file}\"",
            $"-xsl:\"{styleWork}\"",
            $"-o:\"{output}\"",
            $"DIR=\"{workDir.Replace("\\", "/")}/\"",
            "APP=\"ja\"",
          });

          var process = Process.Start(new ProcessStartInfo
          {
            FileName = transform,
            Arguments = arguments,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = workDir
          });

          var error = process.StandardError.ReadToEnd();
          process.WaitForExit();

          if (!string.IsNullOrWhiteSpace(error))
            issues.AddRange(ParseIssues(file, error));

          if (!File.Exists(output))
            continue;

          var html = File.ReadAllText(output, Encoding.UTF8);
          html = html.Replace("<!DOCTYPE HTML>", "");
          html = html.Replace("<html>", "<template>");
          html = html.Replace("</html>", "</template>");
          output = file.Replace(".xml", ".vue");
          File.WriteAllText(output, html, Encoding.UTF8);
        }
        catch (Exception ex)
        {
          issues.Add(new ValidationIssue
          {
            Severity = ValidationSeverity.Error,
            FileName = Path.GetFileName(file),
            Line = 1,
            TechnicalMessage = ex.Message,
            UserMessage = "Bei der XML-Validierung ist ein unerwarteter Verarbeitungsfehler aufgetreten.",
            Suggestion = "Prüfen Sie Datei und Validierungsumgebung. Falls der Fehler bleibt, technische Meldung an das Entwicklungsteam weitergeben."
          });
        }
      }

      DetailIssues = issues
        .GroupBy(x => x.GroupingKey)
        .Select(x => x.First())
        .ToList();

      Valid = DetailIssues.Count == 0;
      DetailErrorReport = DetailIssues.BuildValidationMessage("Folgende XML-Dateien sind nicht valide (Schema-Validierung):");
    }

    public override bool BreakExecution { get; } = false;

    private static IEnumerable<ValidationIssue> ParseIssues(string filePath, string errorOutput)
    {
      var fileName = Path.GetFileName(filePath);
      var lines = errorOutput
        .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
        .Select(x => x.Trim())
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .ToList();

      if (lines.Count == 0)
      {
        return new[]
        {
          new ValidationIssue
          {
            Severity = ValidationSeverity.Error,
            FileName = fileName,
            Line = 1,
            TechnicalMessage = "Unbekannter Validierungsfehler.",
            UserMessage = "Die XML-Datei enthält mindestens einen Validierungsfehler.",
            Suggestion = "Öffnen Sie die Datei im XML-Editor und prüfen Sie die Struktur gegen die MAP-Spezifikation."
          }
        };
      }

      var issues = new List<ValidationIssue>();
      foreach (var line in lines)
      {
        int lineNo;
        ExtractLocation(line, out lineNo);

        var normalizedLine = lineNo > 0 ? lineNo : 1;

        issues.Add(new ValidationIssue
        {
          Severity = ValidationSeverity.Error,
          FileName = fileName,
          Line = normalizedLine,
          Context = ValidationReportBuilder.GetContextLine(filePath, normalizedLine),
          TechnicalMessage = line,
          UserMessage = ValidationReportBuilder.TranslateToUserMessage(line),
          Suggestion = ValidationReportBuilder.BuildSuggestion(line)
        });
      }

      return issues;
    }

    private static void ExtractLocation(string text, out int line)
    {
      line = 0;

      var match = LinePattern.Match(text ?? string.Empty);
      if (!match.Success)
        return;

      int.TryParse(match.Groups[1].Value, out line);
    }
  }
}
