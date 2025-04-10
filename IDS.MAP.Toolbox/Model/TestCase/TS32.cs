using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using IDS.MAP.Toolbox.Helper;
using IDS.MAP.Toolbox.Model.Struktur;
using IDS.MAP.Toolbox.Model.TestCase.Abstract;

namespace IDS.MAP.Toolbox.Model.TestCase
{
  public class TS32 : AbstractTestCase
  {
    private Regex _test = new Regex(@"^[a-z0-9_]+$");

    public override void Execute(ref MapConfiguration config)
    {
      var errors = new List<string>();
      config.Pages = new HashSet<string>();
      config.PatternIds = new HashSet<string>();

      foreach (var f in config.WorkStructFiles)
      {
        try
        {
          var dir = Path.GetDirectoryName(f);
          var name = Path.GetFileName(dir);                    

          if (!File.Exists(Path.Combine(config.MapArticlePath, name, "index.xml")))
            errors.Add($"{name} - Die Datei index.xml muss für die PRP erstellt werden.");
          else
          {
            config.Pages.Add($"{name}/index");
            File.Copy(Path.Combine(config.MapArticlePath, name, "index.xml"), Path.Combine(config.WorkArticlePath, name, "index.xml"), true);
          }

          var serializer = new XmlSerializer(typeof(overview));

          overview overview;
          using (var stream = new FileStream(f, FileMode.Open, FileAccess.Read))
          {
            overview = (overview)serializer.Deserialize(stream);
            if (overview == null)
              throw new Exception();
          }

          var ids = new HashSet<string>();
          foreach (var entry in overview.pattern)
            ValidateId(name, ref errors, ref ids, entry.id);
          foreach (var family in overview.family)
          {
            ValidateId(name, ref errors, ref ids, family.id);
            foreach (var entry in family.pattern)
              ValidateId(name, ref errors, ref ids, entry.id);
          }
          foreach(var x in ids)
          {
            config.PatternIds.Add(x);
            config.Pages.Add($"{name}/{x}");
          }

          foreach (var id in ids)
          {
            var xTest = Path.Combine(config.MapArticlePath, name, $"{id}.xml");

            if (File.Exists(xTest))
              File.Copy(xTest, Path.Combine(config.WorkArticlePath, name, $"{id}.xml"), true);
            else
              errors.Add($"{name} - Für die ID {id} ist keine XML-Datei vorhanden oder ID/Dateiname stimmen nicht überein (_struktur.xml).");

            // TODO: PDF
            //var pTest = Path.Combine(config.MapArticlePath, name, $"{id}.pdf");
            //if (File.Exists(pTest))
            //  File.Copy(pTest, Path.Combine(config.WorkArticlePath, name, $"{id}.pdf"), true);
            //else
            //  errors.Add($"{name} - Für die ID {id} ist keine PDF-Datei vorhanden oder ID/Dateiname stimmen nicht überein (_struktur.xml).");
          }

          foreach (var fLook in Directory.GetFiles(dir, "*.xml", SearchOption.TopDirectoryOnly))
          { 
            var xTest = Path.GetFileName(fLook);
            if (xTest == "index.xml" || xTest == "_struktur.xml")
              continue;
            if (!ids.Contains(Path.GetFileNameWithoutExtension(fLook)))
              errors.Add($"{name} - Die Datei {xTest} ist nicht in der _struktur.xml aufgeführt."); 
          }
        }
        catch
        {
          errors.Add($"{f} - entspricht nicht dem Schema (_struktur.xml)");
          Valid = false;
        }
      }

      Valid = errors.Count == 0;
      DetailErrorReport = errors.Count == 0
        ? null
        : errors.BuildErrorMessage("Folgende PRP enthalten Fehler in der Stuktur (_struktur.xml)");
    }

    private void ValidateId(string name, ref List<string> errors, ref HashSet<string> ids, string id)
    {
      if (ids.Contains(id))
        errors.Add($"{name} - ID {id} ist mehrfach vergeben (_struktur.xml).");

      if (!_test.IsMatch(id))
        errors.Add($"{name} - ID {id} ist kein gülter ID [nur Kleinbuchstaben, Zahlen, Unterstriche - keine Umlaute oder Leerzeichen] (_struktur.xml).");

      ids.Add(id);
    }

    public override bool BreakExecution { get; } = false;
  }
}