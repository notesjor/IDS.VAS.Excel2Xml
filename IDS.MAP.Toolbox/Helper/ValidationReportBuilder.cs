using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Helper
{
  public static class ValidationReportBuilder
  {
    public static string BuildValidationMessage(this IEnumerable<ValidationIssue> issues, string prefix, string postfix = null)
    {
      var normalized = (issues ?? Enumerable.Empty<ValidationIssue>())
        .Where(x => x != null)
        .GroupBy(x => x.GroupingKey)
        .Select(x => x.First())
        .OrderBy(x => x.Severity)
        .ThenBy(x => x.FileName)
        .ThenBy(x => x.Line ?? int.MaxValue)
        .ToList();

      if (normalized.Count == 0)
        return null;

      var fileCount = normalized
        .Where(x => !string.IsNullOrWhiteSpace(x.FileName))
        .Select(x => x.FileName)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Count();

      var errors = normalized.Count(x => x.Severity == ValidationSeverity.Error);
      var warnings = normalized.Count(x => x.Severity == ValidationSeverity.Warning);
      var infos = normalized.Count(x => x.Severity == ValidationSeverity.Info);

      var builder = new StringBuilder();
      builder.AppendLine(prefix);
      builder.AppendLine(string.Format("Zusammenfassung: {0} Fehler in {1} Datei(en), {2} Warnung(en), {3} Hinweis(e).", errors, fileCount, warnings, infos));
      builder.AppendLine();

      foreach (var group in normalized.GroupBy(x => string.IsNullOrWhiteSpace(x.FileName) ? "(unbekannt)" : x.FileName))
      {
        builder.AppendLine("Datei: " + group.Key);
        foreach (var issue in group)
        {
          var lineInfo = issue.Line.HasValue ? "Zeile " + issue.Line.Value : "Zeile unbekannt";
          builder.AppendLine(string.Format(" - [{0}] {1}: {2}", GetSeverityLabel(issue.Severity), lineInfo, issue.UserMessage));

          if (!string.IsNullOrWhiteSpace(issue.Context))
            builder.AppendLine("   Kontext: " + issue.Context.Trim());

          if (!string.IsNullOrWhiteSpace(issue.Suggestion))
            builder.AppendLine("   Vorschlag: " + issue.Suggestion);
        }

        builder.AppendLine();
      }

      if (!string.IsNullOrWhiteSpace(postfix))
        builder.AppendLine(postfix);

      return builder.ToString().Trim();
    }

    public static string TranslateToUserMessage(string technicalMessage)
    {
      var message = (technicalMessage ?? string.Empty).ToLowerInvariant();

      if (message.Contains("end tag") && message.Contains("must match"))
        return "Ein schließendes Tag passt nicht zum öffnenden Tag.";
      if (message.Contains("attribute") && message.Contains("not allowed"))
        return "Ein Attribut ist an dieser Stelle nicht erlaubt.";
      if (message.Contains("required") && message.Contains("attribute"))
        return "Ein erforderliches Attribut fehlt.";
      if (message.Contains("undeclared") && message.Contains("element"))
        return "Ein Element ist im Schema nicht definiert.";
      if (message.Contains("content") && message.Contains("invalid"))
        return "Die Reihenfolge oder der Inhalt von Elementen entspricht nicht dem Schema.";
      if (message.Contains("unexpected end") || message.Contains("premature end"))
        return "Das Dokument endet unerwartet. Vermutlich fehlt ein schließendes Tag.";

      return "Ungültige XML-Struktur gefunden.";
    }

    public static string BuildSuggestion(string technicalMessage)
    {
      var message = (technicalMessage ?? string.Empty).ToLowerInvariant();

      if (message.Contains("end tag") && message.Contains("must match"))
        return "Prüfen Sie die Verschachtelung und schließen Sie das korrekte Tag.";
      if (message.Contains("attribute") && message.Contains("not allowed"))
        return "Entfernen Sie das Attribut oder verwenden Sie ein im Schema erlaubtes Attribut.";
      if (message.Contains("required") && message.Contains("attribute"))
        return "Ergänzen Sie das fehlende Pflichtattribut gemäß map.dtd/map.xsl-Spezifikation.";
      if (message.Contains("undeclared") && message.Contains("element"))
        return "Verwenden Sie nur Elemente, die in der MAP-Spezifikation vorgesehen sind.";
      if (message.Contains("content") && message.Contains("invalid"))
        return "Prüfen Sie die erlaubte Elementreihenfolge und Pflichtknoten in der betroffenen Struktur.";
      if (message.Contains("unexpected end") || message.Contains("premature end"))
        return "Suchen Sie nach nicht geschlossenen Tags vor der genannten Position.";

      return "Prüfen Sie die angegebene Zeile im XML und vergleichen Sie sie mit der MAP-Spezifikation.";
    }

    public static string GetContextLine(string path, int? line)
    {
      if (!line.HasValue || line.Value <= 0)
        return null;

      try
      {
        var contentLine = File.ReadLines(path).Skip(line.Value - 1).FirstOrDefault();
        return contentLine;
      }
      catch
      {
        return null;
      }
    }

    private static string GetSeverityLabel(ValidationSeverity severity)
    {
      switch (severity)
      {
        case ValidationSeverity.Warning:
          return "Warnung";
        case ValidationSeverity.Info:
          return "Hinweis";
        default:
          return "Fehler";
      }
    }
  }
}
