using System.Collections.Generic;

namespace IDS.MAP.Toolbox.Helper
{
  public static class ErrorMessageHelper
  {
    public static string BuildErrorMessage(this IEnumerable<string> errors, string prefix, string postfix = null)
    {
      var res = new List<string> { prefix };
      res.AddRange(errors);
      if(!string.IsNullOrEmpty(postfix))
        res.Add(postfix);
      return string.Join("\r\n", res);
    }
  }
}
