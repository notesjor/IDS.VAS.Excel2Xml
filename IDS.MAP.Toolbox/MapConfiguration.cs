using System.IO;
using System;
using Newtonsoft.Json;
using System.Text;

namespace IDS.MAP.Toolbox
{
  public class MapConfiguration
  {
    private static string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IDS", "MAP", "config.json");
    public string MapPath { get; set; } = null;
    [JsonIgnore]
    public string TmpPath;

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
      if(Directory.Exists(TmpPath))
        Directory.Delete(TmpPath, true);
      Directory.CreateDirectory(TmpPath);

      Directory.CreateDirectory(Path.Combine(TmpPath, "excel"));
      Directory.CreateDirectory(Path.Combine(TmpPath, "artikel"));
      Directory.CreateDirectory(Path.Combine(TmpPath, "vas"));
    }
  }
}
