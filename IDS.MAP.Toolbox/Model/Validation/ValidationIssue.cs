using System;

namespace IDS.MAP.Toolbox.Model.Validation
{
  public enum ValidationSeverity
  {
    Error = 0,
    Warning = 1,
    Info = 2
  }

  public class ValidationIssue
  {
    public ValidationSeverity Severity { get; set; } = ValidationSeverity.Error;
    public string FileName { get; set; }
    public int? Line { get; set; }
    public string Context { get; set; }
    public string TechnicalMessage { get; set; }
    public string UserMessage { get; set; }
    public string Suggestion { get; set; }

    public string GroupingKey
    {
      get
      {
        return string.Join("|", new[]
        {
          Severity.ToString(),
          FileName ?? string.Empty,
          Line?.ToString() ?? string.Empty,
          UserMessage ?? string.Empty,
          Suggestion ?? string.Empty
        });
      }
    }

    public string DisplayLocation
    {
      get
      {
        var location = FileName ?? "(unbekannt)";
        if (Line.HasValue)
          location += ":" + Line.Value;
        return location;
      }
    }

    public override string ToString()
    {
      return string.Format("[{0}] {1} - {2}", Severity, DisplayLocation, UserMessage);
    }
  }
}
