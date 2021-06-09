using Newtonsoft.Json;

namespace IDS.VAS.Excel2Json
{
  public static class GlobalJsonConfig
  {
    public static JsonSerializerSettings Get()
    {
      return new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore
      };
    }
  }
}
