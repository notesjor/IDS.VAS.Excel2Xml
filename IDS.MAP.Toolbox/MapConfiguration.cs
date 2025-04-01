using System.IO;
using System;
using Newtonsoft.Json;
using System.Text;

namespace IDS.MAP.Toolbox
{
  public class MapConfiguration
  {
    private static string _configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "IDS", "MAP", "config.json");
    public string MapDirectory { get; set; } = null;

    public void SettingsSave()
      => File.WriteAllText(_configPath, JsonConvert.SerializeObject(this), Encoding.UTF8);

    public static MapConfiguration Load()
    {
      var dir = Path.GetDirectoryName(_configPath);
      if (!Directory.Exists(dir))
        Directory.CreateDirectory(dir);

      return File.Exists(_configPath) ? JsonConvert.DeserializeObject<MapConfiguration>(File.ReadAllText(_configPath, Encoding.UTF8)) : new MapConfiguration();
    }
  }
}
