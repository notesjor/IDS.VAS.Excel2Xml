using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using System.Text;

namespace IDS.MAP.Toolbox
{
  public class MapConfiguration
  {
    private static string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IDS", "MAP", "config.json");
    public string MapPath { get; set; } = null;
    [JsonIgnore]
    public string TmpPath { get; set; } = null;
    [JsonIgnore]
    public string WorkExcelPath => Path.Combine(TmpPath, "excel", "data.xlsx");
    [JsonIgnore]
    public string WorkArticlePath => Path.Combine(TmpPath, "artikel");
    [JsonIgnore]
    public string MapArticlePath => Path.Combine(MapPath, "artikel");
    [JsonIgnore]
    public string WorkExtraFilePath => Path.Combine(TmpPath, "map");
    [JsonIgnore]
    public string MapExtraFilePath => Path.Combine(MapPath, "map");
    [JsonIgnore]
    public string WorkEtcFilePath => Path.Combine(TmpPath, "etc");
    [JsonIgnore]
    public string MapEtcFilePath => Path.Combine(MapPath, "etc");
    [JsonIgnore]
    public List<string> WorkStructFiles { get; set; }
    [JsonIgnore]
    public HashSet<string> Pages { get; set; }
    [JsonIgnore]
    public HashSet<string> PatternNames { get; set; }
    [JsonIgnore]
    public HashSet<string> PatternIds { get; set; }
    [JsonIgnore]
    public string AppPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    [JsonIgnore]
    public string[] WorkXmlFiles
    {
      get
      {
        var res = new List<string>();
        res.AddRange(Directory.GetFiles(WorkArticlePath, "*.xml", SearchOption.AllDirectories).Where(x => !Path.GetFileName(x).StartsWith("_")));
        res.AddRange(Directory.GetFiles(WorkExtraFilePath, "*.xml", SearchOption.TopDirectoryOnly).Where(x => !x.EndsWith("literatur.xml")));
        return res.ToArray();
      }
    }

    public void SettingsSave()
      => File.WriteAllText(_configPath, JsonConvert.SerializeObject(this), Encoding.UTF8);

    public static MapConfiguration Load()
    {
      var dir = Path.GetDirectoryName(_configPath);
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      return File.Exists(_configPath) ? JsonConvert.DeserializeObject<MapConfiguration>(File.ReadAllText(_configPath, Encoding.UTF8)) : new MapConfiguration();
    }

    public MapConfiguration()
    {
      TmpPath = Path.Combine(Path.GetTempPath(), "MAP");
      if (Directory.Exists(TmpPath))
        Directory.Delete(TmpPath, true);
      Directory.CreateDirectory(TmpPath);

      Directory.CreateDirectory(Path.Combine(TmpPath, "excel"));
      Directory.CreateDirectory(Path.Combine(TmpPath, "artikel"));
      Directory.CreateDirectory(Path.Combine(TmpPath, "map"));
    }
  }
}
