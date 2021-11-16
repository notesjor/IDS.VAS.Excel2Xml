using System.IO;
using IDS.VAS.Excel2Xml.Automate;

namespace IDS.VAS.Excel2Xml.Convert
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length == 0)
        return;

      var controller = new ConvertController();
      controller.Convert(args[0], Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0])));
    }
  }
}
