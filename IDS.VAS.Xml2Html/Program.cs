using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace IDS.VAS.Xml2Html
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args == null)
        return;

      var app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var nuxt = Path.Combine(app, "style.xsl");
      var transform = Path.Combine(app, "XDependencies\\Transform.exe");

      foreach (var file in args)
      {
        try
        {
          var output = file.Replace(".xml", ".html");
          if (File.Exists(output))
            File.Delete(output);

          var arguments = string.Join(" ", new[]
          {
            $"-s:\"{file}\"",
            $"-xsl:\"{nuxt}\"",
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
            File.WriteAllText(file + ".log", error);

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

      Console.WriteLine("!END!");
      Console.ReadLine();
    }
  }
}