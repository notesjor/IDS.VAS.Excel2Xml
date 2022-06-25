using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace IDS.VAS.Xml2Html
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args == null)
        return;

      var app = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      var nuxt = Path.Combine(app, "vas-nuxt.xsl");
      var transform = Path.Combine(app, "XDependencies\\Transform.exe");

      foreach (var file in args)
      {
        try
        {
          var arguments = string.Join(" ", new[]
          {
            $"-s:\"{file}\"",
            $"-xsl:\"{nuxt}\"",
            $"-o:\"{file.Replace(".xml", ".html")}\""
          });

          var process = Process.Start(new ProcessStartInfo
          {
            FileName = transform,
            Arguments = arguments,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            RedirectStandardError = true,
            UseShellExecute = false
          });
          var error = process.StandardError.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(error))
            File.WriteAllText(file + ".log", error);

          process.WaitForExit();
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