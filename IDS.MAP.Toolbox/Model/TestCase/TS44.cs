using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;
using IDS.MAP.Toolbox.Model.Validation;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS44 : AbstractTestCase
  {
    private class LinkReference
    {
      public LinkReference(string href, string sourcePath, int line)
      {
        Href = href;
        SourcePath = sourcePath;
        Line = line;
      }

      public string Href { get; }

      public string SourcePath { get; }

      public int Line { get; }
    }

    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      var issues = new List<ValidationIssue>();
      var targets = new HashSet<string>();
      var links = new Queue<LinkReference>();

      // First Run (Build)
      foreach (var page in config.Pages)
      {
        try
        {
          if (page.Contains("/"))
            SearchLinks(ref targets, ref links, ref errors, ref issues, page, Path.Combine(config.WorkArticlePath, $"{page}.xml"));
          else
            SearchLinks(ref targets, ref links, ref errors, ref issues, page, Path.Combine(config.WorkExtraFilePath, "pages", $"{page}.xml"));
        }
        catch (Exception ex)
        {
          errors.Add($"Ein Fehler in der Verabeitung ist aufgetreten - bitte Jan darüber informieren: {ex.Message}");
          issues.Add(new ValidationIssue
          {
            FileName = page + ".xml",
            Line = 1,
            UserMessage = ex.Message
          });
        }
      }

      // Second Run (Check)
      var last = "";
      foreach (var link in links)
      {
        if (link.Href.StartsWith("http"))
          continue;

        if (!targets.Contains(link.Href))
        {
          if (last != link.Href)
          {
            errors.Add($"\n{Path.GetFileNameWithoutExtension(link.SourcePath)} verlinkt falsch auf:");
            last = link.Href;
          }
          errors.Add(link.Href);
          issues.Add(new ValidationIssue
          {
            FileName = Path.GetFileName(link.SourcePath),
            Line = link.Line,
            UserMessage = $"Ungültiger Link: {link.Href}"
          });
        }
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.BuildErrorMessage("Bei der Überprüfung von Links, sind folgende Fehler aufgetreten:");
      DetailIssues = issues;
    }

    private void SearchLinks(ref HashSet<string> targets, ref Queue<LinkReference> links, ref List<string> errors, ref List<ValidationIssue> issues, string page, string path)
    {
      targets.Add(page);

      var html = new HtmlAgilityPack.HtmlDocument();
      html.Load(path, Encoding.UTF8);

      // , new[] {"#overview", "#scenario", "#meaning", "#forms", "#predicates", "#references"}

      var body = html.DocumentNode.SelectSingleNode("//body");
      if (body == null)
      {
        errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält keinen <body>-Tag.");
        issues.Add(new ValidationIssue
        {
          FileName = Path.GetFileName(path),
          Line = 1,
          UserMessage = "Datei enthält keinen <body>-Tag."
        });
      }

      var bodySections = html.DocumentNode.SelectNodes("//body/section");
      if (bodySections != null)
        foreach (var x in bodySections)
        {
          var label = x.GetAttributeValue("label", "");
          if (label == "")
            continue;

          targets.Add($"{page}#{label}");
          targets.Add($"#{label}");
        }

      SearchLinksInTag(ref targets, page, html, "overview");
      SearchLinksInTag(ref targets, page, html, "meaning");
      SearchLinksInTag(ref targets, page, html, "forms");
      SearchLinksInTag(ref targets, page, html, "predicates");
      SearchLinksInTag(ref targets, page, html, "references");

      var dlinks = html.DocumentNode.SelectNodes("//link");
      if (dlinks == null || dlinks.Count == 0)
        return;

      foreach (var link in dlinks)
      {
        var href = link.GetAttributeValue("href", "");
        if (href == "")
        {
          errors.Add($"Die Datei {Path.GetFileNameWithoutExtension(path)} enthält <link>-Einträge, ohne href.");
          issues.Add(new ValidationIssue
          {
            FileName = Path.GetFileName(path),
            Line = link.Line > 0 ? link.Line : 1,
            UserMessage = "<link>-Eintrag ohne href."
          });
        }
        else
          links.Enqueue(new LinkReference(href, path, link.Line > 0 ? link.Line : 1));
      }
    }

    private static void SearchLinksInTag(ref HashSet<string> links, string page, HtmlDocument html, string tag)
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

        links.Add($"{page}#{tag}/{label}");
      }
    }

    public override bool BreakExecution { get; } = false;
  }
}
