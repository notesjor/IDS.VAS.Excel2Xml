using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS42 : AbstractTestCase
  {
    private HashSet<string> _error;
    private string _current;

    public override void Execute(ref MapConfiguration config)
    {
      _error = new HashSet<string>();
      var files = GetFiles(config);

      var styleOrig = Path.Combine(config.AppPath, "XMAP", "map.xsl");
      var styleWork = Path.Combine(config.WorkEtcFilePath, "map.xsl");
      File.Copy(styleOrig, styleWork, true);

      var transform = Path.Combine(config.AppPath, "XDependencies", "Transform.exe");

      foreach (var file in files)
      {
        try
        {
          var output = file.Replace(".xml", ".html");
          if (File.Exists(output))
            File.Delete(output);

          var arguments = string.Join(" ", new[]
          {
            $"-s:\"{file}\"",
            $"-xsl:\"{styleWork}\"",
            $"-o:\"{output}\""
          });

          var workDir = Path.GetDirectoryName(file);
          Directory.SetCurrentDirectory(workDir);
          Environment.CurrentDirectory = workDir;

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
          if (!string.IsNullOrWhiteSpace(error))
          {
            //_error.Add($"Die Datei {Path.GetFileName(file)} enthält mindestens einen Fehler - bitte Fehler mittels Oxygen beheben.");
            _error.Add($"Die Datei {Path.GetFileName(file)} enthält mindestens einen Fehler - bitte Fehler mittels Oxygen beheben. Der/die Fehler:");
            _error.Add(error);
          }

          process.WaitForExit();
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
          Console.WriteLine(ex.Message);
          Console.WriteLine(ex.StackTrace);
        }
      }

      Valid = _error.Count == 0;
      DetailErrorReport = _error.Count == 0 ?
        null:
        _error.BuildErrorMessage("Folgende XML-Dateien sind nicht valide (Schema-Validierung):");
    }

    private List<string> GetFiles(MapConfiguration config)
    {
      var res = new List<string>();
      res.AddRange(Directory.GetFiles(config.WorkArticlePath, "*.xml", SearchOption.AllDirectories).Where(x => !Path.GetFileName(x).StartsWith("_")));
      res.AddRange(Directory.GetFiles(config.WorkExtraFilePath, "*.xml", SearchOption.TopDirectoryOnly).Where(x => !x.EndsWith("literatur.xml")));
      return res;
    }

    private void ValidationCallBack(object sender, ValidationEventArgs e)
    {
      _error.Add($"{_current} - {e.Message}");
    }

    public override bool BreakExecution { get; } = false;
  }
}