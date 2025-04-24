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
      var targets = new HashSet<string>();
      var links = new Queue<KeyValuePair<string, string>>();

      // First Run (Build)
      foreach (var page in config.Pages)
      {
        try
        {
          if (page.Contains("/"))
            SearchLinks(ref targets, ref links, ref errors, page, Path.Combine(config.WorkArticlePath, $"{page}.xml"));
          else
            SearchLinks(ref targets, ref links, ref errors, page, Path.Combine(config.WorkExtraFilePath, "pages", $"{page}.xml"));
        }
        catch (Exception ex)
        {
          errors.Add($"Ein Fehler in der Verabeitung ist aufgetreten - bitte Jan darüber informieren: {ex.Message}");
        }
      }

      // Second Run (Check)
      var last = "";
      foreach(var link in links)
      { 
        if(link.Key.StartsWith("http"))
          continue;

        if(!targets.Contains(link.Key))
        {
          if (last != link.Key)
          {
            errors.Add($"\n{Path.GetFileNameWithoutExtension(link.Value)} verlinkt falsch auf:");
            last = link.Key;
          }
          errors.Add(link.Key);
        }
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.BuildErrorMessage("Bei der Überprüfung von Links, sind folgende Fehler aufgetreten:");
    }

    private void SearchLinks(ref HashSet<string> targets, ref Queue<KeyValuePair<string, string>> links, ref List<string> errors, string page, string path)
    {
      targets.Add(page);

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

          targets.Add($"{page}#{label}");
          targets.Add($"#{label}");
        }

      SearchLinksInTag(ref targets, ref errors, page, path, html, "overview");
      SearchLinksInTag(ref targets, ref errors, page, path, html, "meaning");
      SearchLinksInTag(ref targets, ref errors, page, path, html, "forms");
      SearchLinksInTag(ref targets, ref errors, page, path, html, "predicates");
      SearchLinksInTag(ref targets, ref errors, page, path, html, "references");

      foreach(var link in html.DocumentNode.SelectNodes("//link"))
      {
        var href = link.GetAttributeValue("href", "");
        if (href == "")
          errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält <link>-Einträge, ohne href.");
        else
          links.Enqueue(new KeyValuePair<string, string>(href, path));
      }
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