using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using Newtonsoft.Json.Converters;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS44 : AbstractTestCase
  {
    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      var links = new HashSet<string>();

      // First Run (Build)
      foreach (var page in config.Pages)
      {
        try
        {
          if (page.Contains("/"))
            SearchLinks(ref links, ref errors, page, Path.Combine(config.WorkArticlePath, $"{page}.xml"));
          else
            SearchLinks(ref links, ref errors, page, Path.Combine(config.WorkExtraFilePath, $"{page}.xml"));
        }
        catch (Exception ex)
        {
          errors.Add($"Ein Fehler in der Verabeitung ist aufgetreten - bitte Jan darüber informieren: {ex.Message}");
        }
      }

      // Index Fix

      // Second Run (Check)

      Valid = errors.Count == 0;
      DetailErrorReport = errors.BuildErrorMessage("Bei der Überprüfung von Links, sind folgende Fehler aufgetreten:");
    }

    private void SearchLinks(ref HashSet<string> links, ref List<string> errors, string page, string path)
    {
      links.Add(page);

      var html = new HtmlAgilityPack.HtmlDocument();
      html.Load(path, Encoding.UTF8);

      // , new[] {"#overview", "#scenario", "#meaning", "#forms", "#predicates", "#references"}

      var body = html.DocumentNode.SelectSingleNode("//body");
      if (body == null)
        errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält keinen <body>-Tag.");

      var bodySections = html.DocumentNode.SelectNodes("//body/section");
      if (bodySections != null)
        foreach (var x in bodySections)
        {
          var label = x.GetAttributeValue("label", "");
          if (label == "")
            continue;
            //errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält <section>-Einträge, ohne label.");

          links.Add($"{page}#{label}");
        }

      SearchLinksInTag(ref links, ref errors, page, path, html, "overview");
      SearchLinksInTag(ref links, ref errors, page, path, html, "meaning");
      SearchLinksInTag(ref links, ref errors, page, path, html, "forms");
      SearchLinksInTag(ref links, ref errors, page, path, html, "predicates");
      SearchLinksInTag(ref links, ref errors, page, path, html, "references");
    }

    private static void SearchLinksInTag(ref HashSet<string> links, ref List<string> errors, string page, string path, HtmlDocument html, string tag)
    {
      var overview = html.DocumentNode.SelectNodes($"//body/{tag}")?.FirstOrDefault();
      if (overview == null) 
        return;

      links.Add($"{page}#{tag}");
      links.Add($"#{tag}");
      var sections = html.DocumentNode.SelectNodes($"//body/{tag}/section");
      if (sections == null) 
        return;

      foreach (var x in sections)
      {
        var label = x.GetAttributeValue("label", "");
        if (label == "")
          continue;
          //errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält in <{tag}> ein/mehrere <section>-Einträge, ohne label.");
        links.Add($"{page}#{tag}/{label}");
      }
    }

    public override bool BreakExecution { get; } = false;
  }
}